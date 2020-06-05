using ROSBridgeLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScanningConnection : MonoBehaviour {

    private ROSBridgeWebSocketConnection ros = null;

    // ROSBridge Server IP address to connect to.
    public string ip;

    // Use this for initialization
    void Start () {
        Debug.Log("Starting Scanning Connection...");

        // Debug Log to get Client's local IP.
        Debug.Log(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable());
        Debug.Log(IPManager.GetLocalIPAddress());

        // Connects to the specified IP address at port 9090.
        ros = new ROSBridgeWebSocketConnection("ws://"+ip, 9090);
        
        // Adds the LiDAR point cloud subscriber.
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

    // Tool to get your local ip address.
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
