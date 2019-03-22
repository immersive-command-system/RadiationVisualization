using PointCloud;
using UnityEngine;

public class LidarVisualizer : PointCloudVisualizer2 {

    public bool flipYZ = false;

    // Use this for initialization
    void Start () {
        Initialize();

        SetColor(new Color(1, 0, 0, 0.5f));
        SetEmissionColor(new Color(0.25f, 0, 0, 0.5f));
    }

    public void SetPointCloud(PointCloud<PointXYZIntensity> newCloud)
    {
        if (particles.Length < newCloud.Size)
        {
            int oldSize = particles.Length;
            particles = new ParticleSystem.Particle[newCloud.Size];
            cloud.GetParticles(particles);
            for (int i = oldSize; i < particles.Length; i++)
            {
                particles[i] = new ParticleSystem.Particle();
            }
        }
        int ind = 0;
        foreach (PointXYZIntensity point in newCloud.Points)
        {
            particles[ind].position = (flipYZ) ? new Vector3(point.X, point.Z, point.Y) : new Vector3(point.X, point.Y, point.Z);
            particles[ind].startSize = 0.1f;
            ind += 1;
        }
        particle_count = newCloud.Size;
        cloud.SetParticles(particles, particle_count);
        OnParticlesUpdated();
    }
    
}
