using PointCloud;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmapVisualizer : PointCloudVisualizer2
{
    public bool flipYZ = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        SetColor(new Color(1, 1, 1, 1));
        SetEmissionColor(new Color(0.8f, 0.8f, 0.8f, 0.8f));
    }

    void UpdateMap(PointCloud.PointCloud<PointXYZ> c)
    {
        if (particles.Length < c.Size)
        {
            particles = new ParticleSystem.Particle[c.Size];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new ParticleSystem.Particle();
            }
        }
        int ind = 0;
        foreach (PointXYZ p in c.Points)
        {
            particles[ind].position = (flipYZ) ? new Vector3(p.X, p.Z, p.Y) : new Vector3(p.X, p.Y, p.Z);
            particles[ind].startSize = 0.1f;
        }
        particle_count = c.Size;
        OnParticlesUpdated();
    }
}
