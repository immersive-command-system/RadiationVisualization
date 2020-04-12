using ROSBridgeLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScanningConnection : MonoBehaviour {

    private ROSBridgeWebSocketConnection ros = null;

    // Use this for initialization
    void Start () {
        // TODO: Understand this line
        Debug.Log("Starting Scanning Connection...");
        //ros = new ROSBridgeWebSocketConnection("ws://128.32.43.94", 9090);
        //ros = new ROSBridgeWebSocketConnection("ws://192.168.1.102", 9090);
        Debug.Log(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable());
        Debug.Log(IPManager.GetLocalIPAddress());
        ros = new ROSBridgeWebSocketConnection("ws://192.168.107.113", 9090);
        ros.AddSubscriber(typeof(PointCloud2Subscriber));

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

    public static class IPManager
    {
        public static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
