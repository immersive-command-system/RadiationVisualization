using ROSBridgeLib;
using ROSBridgeLib.sensor_msgs;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public class ImageConnection : MonoBehaviour, ROSTopicSubscriber
{
    public string rosbridgeAddress = "128.32.43.94";
    public int rosbridgePort = 9090;
    public string topicName = "camera/image_raw";

    private ROSBridgeConnectionsManager ros = null;
    private ROSBridgeWebSocketConnection connection = null;

    private ImageMsg curr_image_msg = null;
    public byte[] curr_image
    {
        get
        {
            return (curr_image_msg == null) ? null : curr_image_msg.GetImage();
        }
    }
    public uint width
    {
        get
        {
            return (curr_image_msg == null) ? 0 : curr_image_msg.GetWidth();
        }
    }
    public uint height
    {
        get
        {
            return (curr_image_msg == null) ? 0 : curr_image_msg.GetHeight();
        }
    }
    public float last_update_time { get; private set; } = 0;

    public delegate void ImageSubscriber(in byte[] image_data, uint width, uint height);
    private object subscriber_lock = new object();
    private List<ImageSubscriber> subscribers = new List<ImageSubscriber>();


    // Use this for initialization
    void Start()
    {
    }

    public void AddSubscriber(ImageSubscriber subscriber)
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

    public void RemoveSubscriber(ImageSubscriber subscriber)
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
        } else if (connection == null) {
            connection = ros.GetConnection(rosbridgeAddress, rosbridgePort);
            connection.AddSubscriber(topicName, this);
        }
    }

    public string GetMessageType(string topic)
    {
        if (topic.Equals(topicName))
        {
            return "sensor_msgs/Image";
        }
        return "";
    }

    public ROSBridgeMsg OnReceiveMessage(string topic, JSONNode raw_msg, ROSBridgeMsg parsed = null)
    {
        if (topic.Equals(topicName))
        {
            if (parsed == null)
            {
                curr_image_msg = new ImageMsg(raw_msg);
            } else
            {
                curr_image_msg = (ImageMsg)parsed;
            }
            last_update_time = Time.time;

            lock (subscriber_lock)
            {
                foreach (ImageSubscriber sub in subscribers)
                {
                    sub(curr_image, width, height);
                }
            }

            return curr_image_msg;
        }
        return parsed;
    }
}
