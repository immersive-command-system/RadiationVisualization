using UnityEngine;

using PointCloud;

/// <summary>
/// A PointCloudVisulizer subclass for visualizing PointClouds, especially SLAM submaps.
/// </summary>
public class SubmapVisualizer : PointCloudVisualizer2
{
    private PointCloud<PointXYZIntensity> pending_cloud = null;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        SetShader("Particles/Standard Surface");
        SetColor(new Color(1, 1, 1, 1));
        SetEmissionColor(new Color(0.8f, 0.8f, 0.8f, 0.8f));

        if (pending_cloud != null)
        {
            UpdateMap(pending_cloud);
            pending_cloud = null;
        }

        // Set update mode to timed since sometimes it won't update if only refreshed once.
        SetUpdateMode(UpdateMode.TIMED);

        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SetRenderMethod(ParticleSystemRenderMode.Mesh, temp.GetComponent<MeshFilter>().mesh);
        Destroy(temp);
    }

    /// <summary>
    /// Display a new point cloud (replacing the old one).
    /// </summary>
    /// <param name="c"></param>
    public void UpdateMap(PointCloud<PointXYZIntensity> c)
    {
        if (particles == null)
        {
            pending_cloud = c;
            return;
        }
        // Ensure that particle array has enough capacity.
        if (particles.Length < c.Size)
        {
            particles = new ParticleSystem.Particle[c.Size];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new ParticleSystem.Particle();
            }
        }

        // Copy over the positions of the points.
        int ind = 0;
        foreach (PointXYZIntensity p in c.Points)
        {
            particles[ind].position = new Vector3(p.X, p.Y, p.Z);
            particles[ind].startColor = new Color32(255, 255, 255, (byte)(255 * p.intensity));
            particles[ind].startSize = 0.1f * p.intensity;
            ind++;
        }
        particle_count = c.Size;
        OnParticlesUpdated();
    }
}
