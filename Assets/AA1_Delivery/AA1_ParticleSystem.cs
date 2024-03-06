using System;
using TreeEditor;

[System.Serializable]
public class AA1_ParticleSystem
{

    

    [System.Serializable]
    public struct Settings
    {
        public uint poolCapacity;
        public Vector3C gravity;
        public float bounce;
    }
    public Settings settings;

    [System.Serializable]
    public struct SettingsCascade
    {
        public Vector3C PointA;
        public Vector3C PointB;
        public Vector3C direction;
        public bool randomDirection;
        public float minimumForce;
        public float maximumForce;
        public float minimumParticlesPerSecond;
        public float maximumParticlesPerSecond;
        public float minimumParticlesLife;
        public float maximumParticlesLife;
    }
    public SettingsCascade settingsCascade;

    [System.Serializable]
    public struct SettingsCannon
    {
        public Vector3C Start;
        public Vector3C Direction;
        public float angle;
        public float maximumForce;
        public float minimumForce;
        public float minimumParticlesPerSecond;
        public float maximumParticlesPerSecond;
        public float minimumParticlesLife;
        public float maximumParticlesLife;
    }
    public SettingsCannon settingsCannon;

    [System.Serializable]
    public struct SettingsCollision
    {
        public PlaneC[] planes;
        public SphereC[] spheres;
        public CapsuleC[] capsules;
    }
    public SettingsCollision settingsCollision;

    public struct Particle
    {
        public Vector3C position;
        public Vector3C lastPosition;
        public Vector3C acceleration;
        public float size;
        public float life;

        private float timeOfCreation;

        public bool alive;
        public void AddForce(Vector3C force)
        {
            acceleration += force;
        }
    }
    Random rnd = new Random();
    Particle[] particles = null;
    Particle[] aliveParticles = null;
    private float time = 0;

    private LineC cascade;
    public Particle[] Update(float dt)
    {
        if(time == 0)
        {
            cascade = new LineC(settingsCascade.PointA, settingsCascade.PointB);
            particles = new Particle[settings.poolCapacity];
            for(int i = 0; i < particles.Length; i++)
            {
                particles[i].alive = false;
            }
        }

        UpdateCascade(dt);

        for (int i = 0; i < particles.Length; ++i)
        {
            if (particles[i].alive)
            particles[i].position += settings.gravity * dt;
        }
            time += dt;
        return particles;
    }

    public int UpdateCascade(float dt)
    {
        double this = rnd.NextDouble();

        return 0;
    }

    public void Activeparticles(int particles)
    {

    }

    public void Debug()
    {
        foreach (var item in settingsCollision.planes)
        {
            item.Print(Vector3C.red);
        }
        foreach (var item in settingsCollision.capsules)
        {
            item.Print(Vector3C.green);
        }
        foreach (var item in settingsCollision.spheres)
        {
            item.Print(Vector3C.blue);
        }
    }
}
