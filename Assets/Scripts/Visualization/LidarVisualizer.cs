using UnityEngine;

using PointCloud;

/// <summary>
/// A class for visualizing real-time point-cloud data, especially lidar data.
/// Uses the CPU-based particle system (inherited from PointCloudVisualizer2).
/// </summary>
public class LidarVisualizer : PointCloudVisualizer2 {

    /// <value> Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`.</value>
    public bool flipYZ = false;

    // Use this for initialization
    void Start () {
        Initialize();

        SetColor(new Color(1, 0, 0, 0.5f));
        SetEmissionColor(new Color(0.25f, 0, 0, 0.5f));
    }

    /// <summary>
    /// Replace the current point cloud with a new point cloud.
    /// </summary>
    /// <param name="newCloud"></param>
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
