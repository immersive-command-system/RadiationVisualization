using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationPointSubscriber : MonoBehaviour, DataServer.DataSubscriber
{
    public DataServer server;

    private ParticleSystem cloud_viewer = null;

    // Start is called before the first frame update
    void Start()
    {
        cloud_viewer = GetComponent<ParticleSystem>();
        if (cloud_viewer == null)
        {
            cloud_viewer = gameObject.AddComponent<ParticleSystem>();
        }

        server.RegisterDataSubscriber("Radiation", this);
    }

    public void OnReceiveMessage(float timestamp, string message)
    {
        string[] parts = message.Split(',');
        float x, y, z, intensity;
        if (parts.Length >= 4 && float.TryParse(parts[0], out x) && 
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z) && 
            float.TryParse(parts[3], out intensity))
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
