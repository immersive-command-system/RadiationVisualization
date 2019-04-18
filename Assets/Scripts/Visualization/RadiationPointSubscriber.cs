using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationPointSubscriber : PointCloudVisualizer2, DataServer.DataSubscriber
{
    public DataServer server;

    public bool flipYZ = false;

    private float size = 1;
    private float max = 0.0007f;


    private BoundsOctree<float> boundsTree = null;

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
            if (boundsTree == null)
            {
                boundsTree = new BoundsOctree<float>(this.size * 100, new Vector3(0, 0, 0), this.size, 1f);
            }
            ParticleSystem.Particle p = new ParticleSystem.Particle();
            p.position = (flipYZ) ? new Vector3(x, z, y) : new Vector3(x, y, z);
            p.startSize = this.size;

            p.startColor = colorFromIntensity(intensity);

            AddParticle(p);

            // Adding the cube to the octree
            Vector3 bounds = new Vector3(this.size, this.size, this.size);
            boundsTree.Add(intensity, new Bounds(p.position, bounds));

        } else if (parts.Length == 1 && float.TryParse(parts[0], out size))
        {
            this.size = size;
        }
    }

    private List<float> radiationVals = new List<float>();
    public Color GetRadiationColor(Vector3 point)
    {
        radiationVals.Clear();
        boundsTree.GetColliding(radiationVals, new Bounds(point, new Vector3(this.size * 5, this.size * 5, this.size * 5)));
        if (radiationVals.Count == 0)
        {
            return new Color(1, 1, 1, 1);
        }
        Color c = colorFromIntensity(radiationVals[0]);
        for (int i = 1; i < radiationVals.Count; i++)
        {
            c += colorFromIntensity(radiationVals[i]);
        }
        return c / radiationVals.Count;
    }

    private Color colorFromIntensity(float intensity)
    {
        float intensity_norm = Mathf.Min(Mathf.Abs(intensity), max) / max;
        Color temp = Color.HSVToRGB(0.85f * (1 - intensity_norm), 1, 1);
        return new Color(temp.r, temp.g, temp.b, intensity_norm * intensity_norm * intensity_norm);
    }
}
