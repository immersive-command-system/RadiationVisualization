using System;
using UnityEngine;

public class CloudDataSubscriber : PointCloudVisualizer2, DataServer.DataSubscriber
{
    public DataServer server;
    
    public bool flipYZ = false;
    public int msgCount = 0;

    void Start ()
    {
        Initialize();
        SetColor(new Color(1, 1, 1, 1));
        SetEmissionColor(new Color(0.8f, 0.8f, 0.8f, 0.8f));

        server.RegisterDataSubscriber("Cloud", this);
    }

    public void OnReceiveMessage(float timestamp, string message)
    {
        string[] parts = message.Split(',');
        float x, y, z;
        msgCount++;
        if (parts.Length >= 3 && float.TryParse(parts[0], out x) && 
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z))
        {
            ParticleSystem.Particle p = new ParticleSystem.Particle();
            p.position = (flipYZ) ? new Vector3(x, z, y) : new Vector3(x, y, z);
            p.startSize = 0.1f;
            AddParticle(p);
        }
    }    
}
