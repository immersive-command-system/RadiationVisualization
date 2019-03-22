using ROSBridgeLib;
using UnityEngine;
using SimpleJSON;

public class SubmapConnection : MonoBehaviour
{

    public bool isUpdating = false;

    private ROSBridgeWebSocketConnection ros = null;

    // Use this for initialization
    void Start()
    {
        // TODO: Understand this line
        Debug.Log("Starting Submap Connection...");
        ros = new ROSBridgeWebSocketConnection("ws://128.32.43.94", 9090);
        //ros.AddSubscriber(typeof(SubmapListSubscriber));
        ros.AddJSONServiceResponse(typeof(SubmapServiceResponse));
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

        if (Input.GetKeyUp(KeyCode.Space) && !isUpdating)
        {
            isUpdating = true;
            ros.CallService(SubmapServiceResponse.GetServiceName(), "[0, 0, 0.5, false]");
        }
    }
}
