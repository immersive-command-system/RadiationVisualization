using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNDataVisualizer : PointCloudVisualizer2
{
    public float pointSize = 0.1f;
    public float maxRadiationLevel = 0.0007f;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        SetShader("Particles/Standard Unlit");
    }

    public void AddRadiationPoint(Vector3 position, float radiationLevel)
    {
        ParticleSystem.Particle p = new ParticleSystem.Particle();
        p.startSize = pointSize;
        float intensity_norm = Mathf.Min(Mathf.Abs(radiationLevel), maxRadiationLevel) / maxRadiationLevel;
        Color temp = Color.HSVToRGB(0.85f * (1 - intensity_norm), 1, 1);
        p.startColor = new Color(temp.r, temp.g, temp.b, intensity_norm * intensity_norm * intensity_norm);
        AddParticle(p);
    }
}
