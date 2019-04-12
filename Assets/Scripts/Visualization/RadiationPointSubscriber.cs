using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationPointSubscriber : PointCloudVisualizer2, DataServer.DataSubscriber
{
    public DataServer server;

    public bool flipYZ = false;

    private float size = 1;
    private float max = 0.0007f;

    // Create an octree
    // Initial size (metres), initial centre position, minimum node size (metres), looseness
    // Also decide on the type of object within the tree, currently it's a dummy Int (because we want to check the bounds anyways)
    private BoundsOctree<int> boundsTree = new BoundsOctree<int>(15, new Vector3(0,0,0), 1, 1.25f);

    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        SetShader("Particles/Standard Unlit");

        SetColor(new Color(1, 1, 1, 0.2f));
        SetEmissionColor(new Color(0, 0, 0, 0));

        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SetRenderMethod(ParticleSystemRenderMode.Mesh, temp.GetComponent<MeshFilter>().mesh);
        Destroy(temp);

        server.RegisterDataSubscriber("Radiation", this);

    }

    public void OnReceiveMessage(float timestamp, string message)
    {
        string[] parts = message.Split(',');
        float x, y, z, intensity, size;
        if (parts.Length >= 4 && float.TryParse(parts[0], out x) &&
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z) &&
            float.TryParse(parts[3], out intensity))
        {
            ParticleSystem.Particle p = new ParticleSystem.Particle();
            p.position = (flipYZ) ? new Vector3(x, z, y) : new Vector3(x, y, z);
            p.startSize = this.size;

            float intensity_norm = Mathf.Min(Mathf.Abs(intensity), max) / max;
            Color temp = Color.HSVToRGB(0.85f * (1 - intensity_norm), 1, 1);
            p.startColor = new Color(temp.r, temp.g, temp.b, intensity_norm * intensity_norm * intensity_norm);
            AddParticle(p);

            // Adding the cube to the octree?
            // bounds is 1m x 1m x 1m
            Vector3 bounds = new Vector3(this.size, this.size, this.size);
            boundsTree.Add(1, new Bounds(p.position, bounds));

        } else if (parts.Length == 1 && float.TryParse(parts[0], out size))
        {
            this.size = size;
        }
    }
}
