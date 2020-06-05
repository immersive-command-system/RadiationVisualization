using ROSBridgeLib;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles a subscription to a topic broadcasting rntools/RNData message type.
/// The class supports both polling/callback methods of obtaining radiation data.
/// </summary>
public class RadiationConnection : MonoBehaviour, ROSTopicSubscriber
{
    /// <value> The network address of the computer running rosbrdige and cartographer_ros.</value>
    public string rosbridgeAddress = "128.32.43.94";
    /// <value> The port that rosbridge operates on.</value>
    public int rosbridgePort = 9090;
    /// <value> The name of the topic on which radiation data is published.</value>
    public string radiationTopicName = "interaction_data";

    /// <value> The web socket object directly responsible for communicating with ROS.</value>
    /// <remarks> If there is an object with ROSBridgeConnectionManager active in the scene, this field should be auto-populated.</remarks>
    private ROSBridgeWebSocketConnection connection = null;

    /// <value> The last radiation message received.</value>
    public RNDataMsg curr_radiation_msg { get; private set; }

    // The following variables allow for a callback style of receiving new radiation data.

    /// <summary>
    /// The signature for callback methods for receiving new radiation methods.
    /// </summary>
    /// <param name="msg">THe new radiation message.</param>
    public delegate void RadiationSubscriber(in RNDataMsg msg);

    /// <value> The object used as a lock for the list of radiationSubscribers.</value>
    private object subscriber_lock = new object();
    /// <value> A list of RadiationSubscribers callback methods to call upon receiving a new radiation message.</value>
    private List<RadiationSubscriber> radiationSubscribers = new List<RadiationSubscriber>();

    // Use this for initialization
    void Start()
    {
        GetConnection();
    }

    /// <summary>
    /// Attempt to obtain the ROSBridgeConnection object with ROSBridge
    /// </summary>
    void GetConnection()
    {
        if (connection == null)
        {
            // Attempt to obtain a connection object.
            ROSBridgeConnectionsManager manager = ROSBridgeConnectionsManager.Instance;
            if (manager != null)
            {
                connection = manager.GetConnection(rosbridgeAddress, rosbridgePort);
                connection.AddSubscriber(radiationTopicName, this);
            }
        }
    }

    /// <summary>
    /// Register a RadiationSubscriber callback for new radiation messages.
    /// </summary>
    /// <param name="subscriber">The callback method to register.</param>
    /// /// <remarks>If the subscriber has already been registered, calling this function has no effect.</remarks>
    /// <remarks>The subscriber will only receive image messages that arrive after it has registered.</remarks>
    public void AddSubscriber(RadiationSubscriber subscriber)
    {
        lock (subscriber_lock)
        {
            if (radiationSubscribers.Contains(subscriber))
            {
                return;
            }
            radiationSubscribers.Add(subscriber);
        }
    }

    /// <summary>
    /// Unregister a callback from receiving new radiation messages.
    /// </summary>
    /// <param name="subscriber">The callback to unregister.</param>
    /// <remarks>If a subscriber is not currently registered, calling this function will have no effect.</remarks>
    public void RemoveSubscriber(RadiationSubscriber subscriber)
    {
        lock (subscriber_lock)
        {
            radiationSubscribers.Remove(subscriber);
        }
    }

    // Update is called once per frame in Unity
    void Update()
    {
        // Call GetConnection() again just in case the call at Start() was not successful (eg. ros connection manager not active yet).
        GetConnection();
    }

    /// <summary>
    /// Callback method to handle receiving a message on a topic subscribed to.
    /// </summary>
    /// <param name="topic">The topic this message is from.</param>
    /// <param name="raw_msg">The json object representing the raw message.</param>
    /// <param name="parsed">A pre-parsed ROSBridgeMsg, if another object has already parsed it. Possibly null.</param>
    /// <remarks>In this class, this callback method handles the image messages on the topic it is subscribed to.</remarks>
    /// <returns>
    /// The parsed message (a subclass of ROSBridgeMsg). In this case it is an RNDataMsg object.
    /// This will be passed as the parsed param to other subscribers this message is given to for convenience/performance.
    /// </returns>
    public ROSBridgeMsg OnReceiveMessage(string topic, JSONNode raw_msg, ROSBridgeMsg parsed = null)
    {
        if (topic.Equals(radiationTopicName))
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
                foreach (RadiationSubscriber sub in radiationSubscribers)
                {
                    sub(curr_radiation_msg);
                }
            }
            return curr_radiation_msg;
        }
        return parsed;
    }

    /// <summary>
    /// Get the message type of each topic this class subscribes to.
    /// </summary>
    /// <param name="topic">The topic to get the type of.</param>
    /// <returns>The string representing the type of the topic.</returns>
    public string GetMessageType(string topic)
    {
        if (topic.Equals(radiationTopicName))
        {
            return "rntools/RNData";
        }
        return "";
    }
}
