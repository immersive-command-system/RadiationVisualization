using System;
using UnityEngine;

public class CloudDataSubscriber : MonoBehaviour, DataServer.DataSubscriber
{
    public DataServer server;

    void Start ()
    {
        server.RegisterDataSubscriber("Cloud", this);
    }

    public void OnReceiveMessage(float timestamp, string message)
    {
        string[] parts = message.Split(',');
        float x, y, z;

        if (parts.Length >= 3 && float.TryParse(parts[0], out x) && 
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z))
        {

        }
    }
}
