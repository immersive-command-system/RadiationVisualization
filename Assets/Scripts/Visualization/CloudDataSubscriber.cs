using System;
using UnityEngine;

public class CloudDataSubscriber : MonoBehaviour, DataServer.DataSubscriber
{
    public DataServer server;
    public int initialParticleCount = 200000;
    public bool flipYZ = false;
    public int msgCount = 0;

    private ParticleSystem cloud_viewer;
    private ParticleSystem.Particle[] particles;
    private int particle_count;
    private bool hasUpdated = false;

    void Start ()
    {
        cloud_viewer = GetComponent<ParticleSystem>();
        if (cloud_viewer == null)
        {
            cloud_viewer = gameObject.AddComponent<ParticleSystem>();
        }
        InitializeParticleSystem(cloud_viewer, GetComponent<ParticleSystemRenderer>(), initialParticleCount);

        particles = new ParticleSystem.Particle[initialParticleCount];

        server.RegisterDataSubscriber("Cloud", this);
    }

    public void OnReceiveMessage(float timestamp, string message)
    {
        string[] parts = message.Split(',');
        float x, y, z;
        msgCount++;
        if (parts.Length >= 3 && float.TryParse(parts[0], out x) && 
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z))
        {
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

            ParticleSystem.Particle p = new ParticleSystem.Particle();
            p.position = (flipYZ) ? new Vector3(x, z, y) : new Vector3(x, y, z);
            p.startSize = 0.1f;
            p.startColor = new Color32(0, 0, 0, 1);

            particles[particle_count] = p;
            particle_count++;
            hasUpdated = true;
        }
    }

    void Update()
    {
        if (hasUpdated)
        {
            hasUpdated = false;
            cloud_viewer.SetParticles(particles, particle_count);
        }
    }

    private void InitializeParticleSystem(ParticleSystem ps, ParticleSystemRenderer renderer, int max_particles)
    {
        ParticleSystem.MainModule main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.maxParticles = max_particles;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1.0f, 1.0f, 1.0f, 0.0f));

        renderer.sortMode = ParticleSystemSortMode.Distance;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        Material particleMaterial = renderer.material;
        if (particleMaterial == null)
        {
            new Material(Shader.Find("Standard"));
            renderer.material = particleMaterial;
        }
        particleMaterial.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.0f));
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
