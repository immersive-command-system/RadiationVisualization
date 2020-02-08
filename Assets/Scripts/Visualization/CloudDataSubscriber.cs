using UnityEngine;

/// <summary>
/// This file takes care of parsing messages about the PointCloud that are received by the DataServer.
/// The data currently comes in a format of "x, y, z, rgb". The RGB is currently arbitrary.
/// </summary>
public class CloudDataSubscriber : PointCloudVisualizer2, DataServer.DataSubscriber
{
    /// <value> Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`. </value>
    public DataServer server;
    
    /// <value> Setting this to true will give a horizontal view of the data.</value>
    public bool flipYZ = false;

    /// <value> Debug field for  the number of messages this subscriber receives. </value>
    public int msgCount = 0;

    private bool finished = false;
    /// <value> The subscriber script that is the data source for radiation-based colorization.</value>
    /// <remarks> If this field is null, the points take on a default color.</remarks>
    public RadiationPointSubscriber radiationDataSource = null;

    void Start ()
    {
        Initialize();

        SetShader("Particles/Standard Unlit");

        SetColor(new Color(1, 1, 1, 1));
        SetEmissionColor(new Color(0.8f, 0.8f, 0.8f, 0.8f));

        // Need a cube object to get its mesh and to set the particle's default mesh to a cube shape.
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SetRenderMethod(ParticleSystemRenderMode.Mesh, temp.GetComponent<MeshFilter>().mesh);
        // Don't need the cube anymore after getting its mesh.
        Destroy(temp);

        // Called to attach as a subscriber to DataServer.
        server.RegisterDataSubscriber("Cloud", this);
    }

    /// <summary>
    /// The callback for receiving data on the subscribed channel.
    /// Parses and checks if message is corrupted.
    /// Stores data ready for visualization.
    /// </summary>
    /// <param name="timestamp">The timestamp of the received message.</param>
    /// <param name="message">The raw contents of the message.</param>
    public void OnReceiveMessage(float timestamp, string message)
    {
        msgCount++;
        if (string.Compare(message.ToString(), "End of Cloud") == 0)
        {
            finished = true;
        }
        string[] parts = message.Split(',');
        float x, y, z;
        if (parts.Length >= 3 && float.TryParse(parts[0], out x) && 
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z))
        {
            // ParticleSystem is used to visualize PointCloud data.
            ParticleSystem.Particle p = new ParticleSystem.Particle();
            p.position = (flipYZ) ? new Vector3(x, z, y) : new Vector3(x, y, z);
            p.startSize = 0.1f;

            // Check for radiation-based colorization.
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
