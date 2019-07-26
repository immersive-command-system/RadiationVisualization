using ROSBridgeLib;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public class RadiationConnection : MonoBehaviour, ROSTopicSubscriber
{
    public string rosbridgeAddress = "128.32.43.94";
    public int rosbridgePort = 9090;
    public string topicName = "interaction_data";

    private ROSBridgeConnectionsManager ros = null;
    private ROSBridgeWebSocketConnection connection = null;

    public RNDataMsg curr_radiation_msg { get; private set; }

    public delegate void RadiationSubscriber(in RNDataMsg msg);
    private object subscriber_lock = new object();
    private List<RadiationSubscriber> subscribers = new List<RadiationSubscriber>();

    // Use this for initialization
    void Start()
    {
    }

    public void AddSubscriber(RadiationSubscriber subscriber)
    {
        lock (subscriber_lock)
        {
            if (subscribers.Contains(subscriber))
            {
                return;
            }
            subscribers.Add(subscriber);
        }
    }

    public void RemoveSubscriber(RadiationSubscriber subscriber)
    {
        lock (subscriber_lock)
        {
            subscribers.Remove(subscriber);
        }
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
            connection.AddSubscriber(topicName, this);
        }
    }

    public ROSBridgeMsg OnReceiveMessage(string topic, JSONNode raw_msg, ROSBridgeMsg parsed = null)
    {
        if (topic.Equals(topicName))
        {
            if (parsed == null)
            {
                curr_radiation_msg = new RNDataMsg(raw_msg);
            } else
            {
                curr_radiation_msg = (RNDataMsg)parsed;
            }
            lock (subscriber_lock)
            {
                foreach (RadiationSubscriber sub in subscribers)
                {
                    sub(curr_radiation_msg);
                }
            }
            return curr_radiation_msg;
        }
        return parsed;
    }
    public string GetMessageType(string topic)
    {
        if (topic.Equals(topicName))
        {
            return "rntools/RNData";
        }
        return "";
    }
}
