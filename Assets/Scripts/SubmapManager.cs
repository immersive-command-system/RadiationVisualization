using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib.cartographer_msgs;

public class SubmapManager : MonoBehaviour
{
    public SubmapConnection connection;

    private float threshold_probability = 0.5f;

    private Dictionary<int, int> submap_versions = new Dictionary<int, int>();
    private Dictionary<int, SubmapEntryMsg> newest_version = new Dictionary<int, SubmapEntryMsg>();
    private Dictionary<int, SubmapVisualizer> submaps = new Dictionary<int, SubmapVisualizer>();

    public SubmapEntryMsg pending_submap = null;

    public void HandleSubmapList(SubmapListMsg msg)
    {
        foreach (SubmapEntryMsg submapEntry in msg.submap_entries)
        {
            HandleSubmapEntry(submapEntry);
        }
    }

    public void HandleSubmapEntry(SubmapEntryMsg msg)
    {
        if (!newest_version.ContainsKey(msg.submap_index) || 
            msg.submap_version > newest_version[msg.submap_index].submap_version)
        {
            newest_version[msg.submap_index] = msg;
            Debug.Log("Update available for submap: " + msg.submap_index);
        }
    }

    public void HandleSubmapMessage(SubmapCloudMsg msg)
    {
        Debug.Log("Done3");
        if (pending_submap != null)
        {
            Debug.Log("Received pointcloud for submap:" + pending_submap.submap_index);
            GameObject obj;
            SubmapVisualizer vis;
            if (!submaps.ContainsKey(pending_submap.submap_index))
            {
                obj = new GameObject();
                obj.transform.parent = transform;
                vis = obj.AddComponent<SubmapVisualizer>();
                submaps.Add(pending_submap.submap_index, vis);
            } else {
                vis = submaps[pending_submap.submap_index];
                obj = vis.gameObject;
            }
            
            obj.transform.position = new Vector3(pending_submap.pose._position.GetX(), 
                pending_submap.pose._position.GetY(), 
                pending_submap.pose._position.GetZ());
            obj.transform.rotation = new Quaternion(pending_submap.pose._orientation.GetX(), 
                pending_submap.pose._orientation.GetY(), 
                pending_submap.pose._orientation.GetZ(), 
                pending_submap.pose._orientation.GetW());
            vis.UpdateMap(msg.cloud.GetCloud());
            
            submap_versions[pending_submap.submap_index] = pending_submap.submap_version;
            pending_submap = null;
        }
    }

    void Update()
    {
        if (pending_submap == null)
        {
            int minIndex = -1;
            int maxDifference = 0;
            SubmapEntryMsg needed_submap = null;
            foreach (KeyValuePair<int, SubmapEntryMsg> curr in newest_version)
            {
                if (!submaps.ContainsKey(curr.Key))
                {
                    minIndex = curr.Key;
                    needed_submap = curr.Value;
                    break;
                }
                int currDiff = curr.Value.submap_version - submap_versions[curr.Key];
                if (currDiff > maxDifference) {
                    minIndex = curr.Key;
                    maxDifference = currDiff;
                    needed_submap = curr.Value;
                }
            }
            if (minIndex >= 0) {
                pending_submap = needed_submap;
                Debug.Log("Requestiong Submap: " + pending_submap.submap_index);
                connection.ros.CallService(SubmapServiceResponse.GetServiceName(), string.Format("[0, {0}, {1}, false]", minIndex, threshold_probability));
            }
        }
    }
}
