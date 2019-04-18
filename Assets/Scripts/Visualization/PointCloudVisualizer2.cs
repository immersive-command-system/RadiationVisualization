using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudVisualizer2 : MonoBehaviour
{
    public int initialParticleCount = 20000;

    protected ParticleSystem cloud;
    protected ParticleSystemRenderer cloud_renderer;
    protected ParticleSystem.Particle[] particles;
    protected int particle_count = 0;

    public enum UpdateMode
    {
        TIMED,
        ON_REFRESH
    };
    private UpdateMode update_mode = UpdateMode.ON_REFRESH;
    private float updateTimer = 0;
    public float updateInterval = 2;
    protected bool hasChanged = false;

    private bool initialized = false;

    private string shader = "Standard";

    protected void Initialize()
    {
        cloud = GetComponent<ParticleSystem>();
        if (cloud == null)
        {
            cloud = gameObject.AddComponent<ParticleSystem>();
        }
        cloud_renderer = GetComponent<ParticleSystemRenderer>();
        InitializeParticleSystem(cloud, cloud_renderer, initialParticleCount);
        SetRenderMethod(ParticleSystemRenderMode.Billboard);

        particles = new ParticleSystem.Particle[initialParticleCount];

        initialized = true;
    }

    void Update()
    {
        bool should_update = false;
        if (update_mode == UpdateMode.TIMED)
        {
            if (updateTimer >= updateInterval)
            {
                should_update = true;
                updateTimer = 0;
            }
            updateTimer += Time.deltaTime;
        } else if (update_mode == UpdateMode.ON_REFRESH)
        {
            should_update = hasChanged;
            hasChanged = false;
        }
        if (should_update)
        {
            cloud.SetParticles(particles, particle_count, 0);
        }
    }

    public void SetUpdateMode(UpdateMode mode)
    {
        if (mode == UpdateMode.TIMED)
        {
            updateTimer = updateInterval;
        }
        update_mode = mode;
    }

    protected void AddParticle(ParticleSystem.Particle p)
    {
        if (!initialized)
        {
            Initialize();
        }

        ParticleSystem.Particle[] new_particles = particles;
        while (particle_count >= new_particles.Length)
        {
            new_particles = new ParticleSystem.Particle[new_particles.Length * 2];
        }
        if (new_particles != particles)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                new_particles[i] = particles[i];
            }
            particle_count = particles.Length;
            particles = new_particles;
        }
        particles[particle_count] = p;
        particle_count++;

        hasChanged = true;
    }

    protected void OnParticlesUpdated()
    {
        hasChanged = true;
    }

    private List<Color> results = new List<Color>();
    public void ApplyColorSpace(BoundsOctree<Color> colorMap)
    {
        results.Clear();
        for (int i = 0; i < particle_count; i++)
        {
            colorMap.GetColliding(results, new Bounds(particles[i].position, particles[i].startSize * Vector3.one));
            if (results.Count > 0)
            {
                Color c = results[0];
                for (int j = 1; j < results.Count; j++)
                {
                    c += results[j];
                }
                particles[i].startColor = c;
            }
        }
        OnParticlesUpdated();
    }

    public void ApplyColorSpace(Color[, ,] colorMap, Vector3 origin, Vector3 voxelDimensions)
    {
        for (int i = 0; i < particle_count; i++)
        {
            Vector3 particlePos = particles[i].position;
            if (particlePos.x < origin.x ||
                particlePos.y < origin.y || 
                particlePos.z < origin.z)
            {
                continue;
            }
            int ind_x = Mathf.RoundToInt((particlePos.x - origin.x) / voxelDimensions.x);
            int ind_y = Mathf.RoundToInt((particlePos.y - origin.y) / voxelDimensions.y);
            int ind_z = Mathf.RoundToInt((particlePos.z - origin.z) / voxelDimensions.z);

            if (ind_x >= colorMap.GetLength(0) || 
                ind_y >= colorMap.GetLength(1) || 
                ind_z >= colorMap.GetLength(2))
            {
                continue;
            }

            particles[i].startColor = colorMap[ind_x, ind_y, ind_z];
        }
        OnParticlesUpdated();
    }



    protected void SetEmissionColor(Color c)
    {
        Material particleMaterial = cloud_renderer.material;
        particleMaterial.SetColor("_EmissionColor", c);
    }
    protected void SetColor(Color c)
    {
        Material particleMaterial = cloud_renderer.material;
        particleMaterial.SetColor("_Color", c);
    }

    protected void SetFollowCamera(bool follow)
    {
        cloud_renderer.alignment = (follow) ? ParticleSystemRenderSpace.Facing : ParticleSystemRenderSpace.World;
    }

    protected void SetRenderMethod(ParticleSystemRenderMode mode, Mesh mesh = null)
    {
        cloud_renderer.renderMode = mode;
        if (mode == ParticleSystemRenderMode.Mesh)
        {
            SetFollowCamera(false);
            cloud_renderer.mesh = mesh;
            cloud_renderer.enableGPUInstancing = true;
        } else
        {
            SetFollowCamera(true);
        }
    }

    protected void SetShader(string shader_name)
    {
        shader = shader_name;
        Material particleMaterial = new Material(Shader.Find(shader));
        cloud_renderer.material = particleMaterial;

        PrepareMaterial(particleMaterial);
    }

    protected static void PrepareMaterial(Material particleMaterial)
    {
        // Make it transparent
        particleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        particleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.DstAlpha);
        particleMaterial.SetInt("_ZWrite", 0);
        particleMaterial.DisableKeyword("_ALPHATEST_ON");
        particleMaterial.EnableKeyword("_ALPHABLEND_ON");
        particleMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        particleMaterial.renderQueue = 3000;
    }

    protected void InitializeParticleSystem(ParticleSystem ps, ParticleSystemRenderer renderer, int max_particles)
    {
        ParticleSystem.MainModule main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.maxParticles = max_particles;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1.0f, 1.0f, 1.0f, 0.0f));

        renderer.sortMode = ParticleSystemSortMode.Distance;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.enableGPUInstancing = true;
        
        Material particleMaterial = new Material(Shader.Find(shader));
        renderer.material = particleMaterial;

        PrepareMaterial(particleMaterial);

        particleMaterial.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        particleMaterial.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.8f, 1.0f));

        ParticleSystem.EmissionModule em = ps.emission;
        em.enabled = false;
        ParticleSystem.ShapeModule sh = ps.shape;
        sh.enabled = false;
        ParticleSystem.VelocityOverLifetimeModule vol = ps.velocityOverLifetime;
        vol.enabled = false;
        ParticleSystem.LimitVelocityOverLifetimeModule lvol = ps.limitVelocityOverLifetime;
        lvol.enabled = false;
        ParticleSystem.InheritVelocityModule ivm = ps.inheritVelocity;
        ivm.enabled = false;
        ParticleSystem.ForceOverLifetimeModule fol = ps.forceOverLifetime;
        fol.enabled = false;
        ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = false;
        ParticleSystem.ColorBySpeedModule cbs = ps.colorBySpeed;
        cbs.enabled = false;
        ParticleSystem.SizeOverLifetimeModule sol = ps.sizeOverLifetime;
        sol.enabled = false;
        ParticleSystem.SizeBySpeedModule sbs = ps.sizeBySpeed;
        sbs.enabled = false;
        ParticleSystem.RotationOverLifetimeModule rol = ps.rotationOverLifetime;
        rol.enabled = false;
        ParticleSystem.RotationBySpeedModule rbs = ps.rotationBySpeed;
        rbs.enabled = false;
        ParticleSystem.ExternalForcesModule extf = ps.externalForces;
        extf.enabled = false;
        ParticleSystem.NoiseModule noise = ps.noise;
        noise.enabled = false;
        ParticleSystem.CollisionModule collision = ps.collision;
        collision.enabled = false;
        ParticleSystem.TriggerModule triggers = ps.trigger;
        triggers.enabled = false;
        ParticleSystem.SubEmittersModule subem = ps.subEmitters;
        subem.enabled = false;
        ParticleSystem.TextureSheetAnimationModule tsa = ps.textureSheetAnimation;
        tsa.enabled = false;
        ParticleSystem.LightsModule lights = ps.lights;
        lights.enabled = false;
        ParticleSystem.CustomDataModule cd = ps.customData;
        cd.enabled = false;
    }
}
