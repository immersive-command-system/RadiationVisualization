using ROSBridgeLib;
using UnityEngine;
using SimpleJSON;
using ROSBridgeLib.cartographer_msgs;
using System.Collections.Generic;

public class SubmapConnection : MonoBehaviour, ROSTopicSubscriber
{
    public string rosbridgeAddress = "128.32.43.94";
    public int rosbridgePort = 9090;
    public string submapListTopic = "submap_list";
    public string submapServiceName = "submap_cloud_query";

    public bool flipYZ
    {
        get
        {
            return _flipYZ;
        }
        set
        {
            _flipYZ = value;
            transform.rotation = Quaternion.Euler((_flipYZ) ? -90 : 0, 0, 0);
        }
    }
    private bool _flipYZ;

    public int maxConcurrentMapRequests = 1;

    public float threshold_probability = 0.95f;

    private Dictionary<int, int> submap_versions = new Dictionary<int, int>();
    private Dictionary<int, SubmapEntryMsg> newest_version = new Dictionary<int, SubmapEntryMsg>();
    private Dictionary<int, SubmapVisualizer> submaps = new Dictionary<int, SubmapVisualizer>();

    private Dictionary<string, SubmapEntryMsg> pending_submaps = new Dictionary<string, SubmapEntryMsg>();

    private ROSBridgeConnectionsManager ros = null;
    private ROSBridgeWebSocketConnection connection = null;

    // Use this for initialization
    void Start()
    {
        flipYZ = true;
    }

    // Update is called once per frame in Unity
    void Update()
    {
        if (ros == null)
        {
            ros = ROSBridgeConnectionsManager.Instance;
        }
        else if (connection == null)
        {
            connection = ros.GetConnection(rosbridgeAddress, rosbridgePort);
            connection.AddSubscriber(submapListTopic, this);
        }

        if (pending_submaps.Count < maxConcurrentMapRequests)
        {
            int minIndex = -1;
            int maxDifference = 0;
            SubmapEntryMsg needed_submap = null;
            foreach (KeyValuePair<int, SubmapEntryMsg> curr in newest_version)
            {
                bool already_pending = false;
                foreach (SubmapEntryMsg pending in pending_submaps.Values)
                {
                    if (pending.submap_index == curr.Key)
                    {
                        already_pending = true;
                        break;
                    }
                }
                if (already_pending)
                {
                    continue;
                }

                if (!submaps.ContainsKey(curr.Key))
                {
                    minIndex = curr.Key;
                    needed_submap = curr.Value;
                    break;
                }
                int currDiff = curr.Value.submap_version - submap_versions[curr.Key];
                if (currDiff > maxDifference)
                {
                    minIndex = curr.Key;
                    maxDifference = currDiff;
                    needed_submap = curr.Value;
                }
            }
            if (minIndex >= 0)
            {
                string id = submapServiceName + " " + needed_submap.submap_index + "." + needed_submap.submap_version;
                pending_submaps[id] = needed_submap;
                //Debug.Log("Requestiong Submap: " + pending_submap.submap_index);
                connection.CallService(HandleSubmapServiceResponse, submapServiceName, id, string.Format("[{0}, {1}, {2}, false]", needed_submap.trajectory_id, minIndex, threshold_probability));
            }
        }
    }

    void HandleSubmapServiceResponse(JSONNode response)
    {
        SubmapEntryMsg targetEntry;
        if (!pending_submaps.TryGetValue(response["id"].Value, out targetEntry))
        {
            return;
        }
        pending_submaps.Remove(response["id"].Value);
        SubmapCloudMsg cloudMsg = new SubmapCloudMsg(response["values"]);
        HandleSubmapMessage(targetEntry, cloudMsg);
    }

    void HandleSubmapMessage(SubmapEntryMsg entry, SubmapCloudMsg msg)
    {
        if (submap_versions.ContainsKey(entry.submap_index) && 
            submap_versions[entry.submap_index] >= entry.submap_version)
        {
            return;
        }

        GameObject obj;
        SubmapVisualizer vis;
        if (!submaps.ContainsKey(entry.submap_index))
        {
            obj = new GameObject();
            obj.transform.parent = transform;
            vis = obj.AddComponent<SubmapVisualizer>();
            submaps.Add(entry.submap_index, vis);
        }
        else
        {
            vis = submaps[entry.submap_index];
            obj = vis.gameObject;
        }

        obj.transform.position = transform.position + (new Vector3(
            entry.pose._position.GetX(),
            entry.pose._position.GetY(),
            entry.pose._position.GetZ()));
        obj.transform.rotation = transform.rotation * (new Quaternion(
            entry.pose._orientation.GetX(),
            entry.pose._orientation.GetY(),
            entry.pose._orientation.GetZ(),
            entry.pose._orientation.GetW()));

        vis.UpdateMap(msg.cloud.GetCloud());

        submap_versions[entry.submap_index] = entry.submap_version;
    }

    void HandleSubmapList(SubmapListMsg msg)
    {
        foreach (SubmapEntryMsg submapEntry in msg.submap_entries)
        {
            HandleSubmapEntry(submapEntry);
        }
    }

    void HandleSubmapEntry(SubmapEntryMsg msg)
    {
        if (!newest_version.ContainsKey(msg.submap_index) ||
            msg.submap_version > newest_version[msg.submap_index].submap_version)
        {
            newest_version[msg.submap_index] = msg;
        }
    }

    public ROSBridgeMsg OnReceiveMessage(string topic, JSONNode raw_msg, ROSBridgeMsg parsed = null)
    {
        if (topic.Equals(submapListTopic))
        {
            SubmapListMsg submapList;
            if (parsed == null)
            {
                submapList = new SubmapListMsg(raw_msg);
            } else
            {
                submapList = (SubmapListMsg)parsed;
            }
            HandleSubmapList(submapList);
            return submapList;
        }
        return parsed;
    }

    public string GetMessageType(string topic)
    {
        if (topic.Equals(submapListTopic))
        {
            return "cartographer_ros_msgs/SubmapList";
        }
        return "";
    }
}
