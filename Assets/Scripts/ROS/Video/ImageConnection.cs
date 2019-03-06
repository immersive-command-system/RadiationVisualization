using ROSBridgeLib;
using UnityEngine;

public class ImageConnection : MonoBehaviour
{

    private ROSBridgeWebSocketConnection ros = null;

    // Use this for initialization
    void Start()
    {
        // TODO: Understand this line
        Debug.Log("Starting Image Connection...");
        ros = new ROSBridgeWebSocketConnection("ws://128.32.43.94", 9090);
        ros.AddSubscriber(typeof(RawCameraSubscriber));
        ros.Connect();
    }

    void OnApplicationQuit()
    {
        Debug.Log("Disconnecting from ROS");
        if (ros != null)
        {
            ros.Disconnect();
        }
    }

    // Update is called once per frame in Unity
    void Update()
    {
        ros.Render();
    }
}
