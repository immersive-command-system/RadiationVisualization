using System;
using UnityEngine;

/* 
 * This file takes care of parsing messages about the PointCloud that are received by the DataServer.
 * The data currently comes in a format of "x, y, z, rgb". The RGB is currently arbitrary.
 */
public class CloudDataSubscriber : PointCloudVisualizer2, DataServer.DataSubscriber
{
    // Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`.
    public DataServer server;
    
    // Setting this to true will give a horizontal view of the data - This should be equal throughout all subscribers.
    public bool flipYZ = false;

    // Counts the number of messages this subscriber receives.
    public int msgCount = 0;

    public RadiationPointSubscriber radiationDataSource = null;

    void Start ()
    {
        Initialize();

        SetShader("Particles/Standard Unlit");

        SetColor(new Color(1, 1, 1, 1));
        SetEmissionColor(new Color(0.8f, 0.8f, 0.8f, 0.8f));

        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SetRenderMethod(ParticleSystemRenderMode.Mesh, temp.GetComponent<MeshFilter>().mesh);
        Destroy(temp);

        // Called to attach as a subscriber to DataServer.
        server.RegisterDataSubscriber("Cloud", this);
    }

    /* Parses and checks if message is corrupted. Stores data ready for visualization. */
    public void OnReceiveMessage(float timestamp, string message)
    {
        string[] parts = message.Split(',');
        float x, y, z;
        msgCount++;
        if (parts.Length >= 3 && float.TryParse(parts[0], out x) && 
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z))
        {
            // ParticleSystem is used to visualize PointCloud data.
            ParticleSystem.Particle p = new ParticleSystem.Particle();
            p.position = (flipYZ) ? new Vector3(x, z, y) : new Vector3(x, y, z);
            p.startSize = 0.1f;

            if (radiationDataSource != null)
            {
                p.startColor = radiationDataSource.GetRadiationColor(p.position);
            } else
            {
                p.startColor = new Color(1, 1, 1, 0.8f);
            }

            AddParticle(p);
        }
    }    
    
}
