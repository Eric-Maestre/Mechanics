using System;
using System.Collections.Generic;
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
        //esto es velocidad
        public Vector3C acceleration;
        public float size;
        public float life;

        public float timeOfCreation;

        public bool alive;
        public void AddForce(Vector3C force)
        {
            acceleration += force;
        }
    }
    Random rnd = new Random();
    Particle[] particles = null;
    List<Particle> aliveParticles = new List<Particle>();
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
                particles[i].size = 0.1f;
            }
        }

        UpdateCascade(dt);

        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i].position += settings.gravity * dt;
            particles[i].position += particles[i].acceleration * dt;
            if (particles[i].life <= time - particles[i].timeOfCreation)
            {
                particles[i].alive = false;
            }
            for (int j = 0; j < settingsCollision.planes.Length; ++j)
            {
                Vector3C distanceVector = particles[i].position - settingsCollision.planes[j].NearestPoint(particles[i].position);
                float distance = distanceVector.magnitude;
                if (distance < particles[i].size )
                {
                    UnityEngine.Debug.Log("Collision");
                    Vector3C particleDirection = particles[i].position - particles[i].lastPosition;
                    Vector3C bounceDirection = new Vector3C(particleDirection.x * -1, particleDirection.y * -1, particleDirection.z * -1);
                    //bounceDirection.Normalize();
                    particles[i].AddForce(bounceDirection*0.5f/**particles[i].acceleration * settings.bounce*/);
                }
            }
        }
            time += dt;

        AddAliveParticles();

        return aliveParticles.ToArray();
    }

    public void UpdateCascade(float dt)
    {
        double something = rnd.NextDouble();
        int particlesThisSecond = RandomMinMaxInt(something, 0, 1, settingsCascade.minimumParticlesPerSecond, settingsCascade.maximumParticlesPerSecond);

        int counter = 0;

        for(int i = 0; i < particles.Length || counter >= particlesThisSecond; i++)
        {

            if (!particles[i].alive)
            {
                particles[i].alive = true;

                double x = rnd.NextDouble();
                double y = rnd.NextDouble();
                double z = rnd.NextDouble();

                particles[i].position.x = RandomMinMaxFloat(x, 0, 1, settingsCascade.PointA.x, settingsCascade.PointB.x);
                particles[i].position.y = RandomMinMaxFloat(y, 0, 1, settingsCascade.PointA.y, settingsCascade.PointB.y);
                particles[i].position.z = RandomMinMaxFloat(z, 0, 1, settingsCascade.PointA.z, settingsCascade.PointB.z);

                double randomForce = rnd.NextDouble();
                float force = RandomMinMaxFloat(randomForce, 0, 1, settingsCascade.minimumForce, settingsCascade.maximumForce);

                Vector3C forceDirection = settingsCascade.direction.normalized;

                if (settingsCascade.randomDirection)
                {
                    double randDir = rnd.NextDouble();
                    forceDirection.x = forceDirection.x * (float)randDir;
                    forceDirection.y = forceDirection.y * (float)randDir;
                    forceDirection.z = forceDirection.z * (float)randDir;
                }
                particles[i].acceleration = forceDirection * force;

                double randomLife = rnd.NextDouble();
                particles[i].life = RandomMinMaxFloat(randomLife, 0, 1, settingsCascade.minimumParticlesLife, settingsCascade.maximumParticlesLife);

                particles[i].timeOfCreation = time;

                counter++;
            }

        }
    }

    private void AddAliveParticles()
    {
        aliveParticles.Clear();

        for(int i = 0; i < particles.Length; i++)
        {
            if (particles[i].alive)
            {
                aliveParticles.Add(particles[i]);
            }
        }
    }

    private int RandomMinMaxInt (double inputD, float min, float max, float minExpected, float maxExpected)
    {
        float result = minExpected + ((float)inputD - min) * (maxExpected - minExpected) / (max - min);
        return (int)result;
    }

    private float RandomMinMaxFloat(double inputD, float min, float max, float minExpected, float maxExpected)
    {
        return minExpected + ((float)inputD - min) * (maxExpected - minExpected) / (max - min);
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
