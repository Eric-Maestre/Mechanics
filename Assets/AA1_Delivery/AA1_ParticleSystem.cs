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

        UpdateCascade();
        UpdateCannon();

        for (int i = 0; i < particles.Length; ++i)
        {
            //particles[i].position += settings.gravity * dt;
            particles[i].AddForce(settings.gravity * dt);
            particles[i].position += particles[i].acceleration * dt;
            if (particles[i].life <= time - particles[i].timeOfCreation)
            {
                particles[i].alive = false;
            }

            for (int j = 0; j < settingsCollision.planes.Length; ++j)
            {
                Vector3C distanceVector = particles[i].position - settingsCollision.planes[j].NearestPoint(particles[i].position);
                float distance = distanceVector.magnitude;
                if (distance < particles[i].size)
                {
                    Vector3C particleDirection = particles[i].position - particles[i].lastPosition;
                    float particleLastAcceleration = particles[i].acceleration.magnitude;
                    Vector3C accelNullifier = new Vector3C(1f - (float)Math.Sqrt(Math.Pow(settingsCollision.planes[j].normal.x, 2)), 1 - (float)Math.Sqrt(Math.Pow(settingsCollision.planes[j].normal.y, 2)), 1 - (float)Math.Sqrt(Math.Pow(settingsCollision.planes[j].normal.z, 2)));
                    particles[i].acceleration *= accelNullifier;
                    particles[i].AddForce(settingsCollision.planes[j].normal * settings.bounce * particleLastAcceleration * -(particleDirection.normalized));
                }
            }
        }
            time += dt;

        AddAliveParticles();

        return aliveParticles.ToArray();
    }

    public void UpdateCascade()
    {
        double something = rnd.NextDouble();
        int particlesThisSecond = RandomMinMaxInt(something, 0, 1, settingsCascade.minimumParticlesPerSecond, settingsCascade.maximumParticlesPerSecond);

        int counter = 0;
        int i = 0;

        while (i < particles.Length && counter < particlesThisSecond)
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

                Vector3C forceDirection = settingsCannon.Direction.normalized;

                if (settingsCascade.randomDirection)
                {
                    double randDir = rnd.NextDouble();
                    forceDirection.x = forceDirection.x * (float)randDir;
                    forceDirection.y = forceDirection.y * (float)randDir;
                    forceDirection.z = forceDirection.z * (float)randDir;
                }
                particles[i].acceleration = forceDirection * force;

                double randomLife = rnd.NextDouble();
                //particles[i].life = RandomMinMaxFloat(randomLife, 0, 1, settingsCascade.minimumParticlesLife, settingsCascade.maximumParticlesLife);
                
                particles[i].life = 10;//To experiment

                particles[i].timeOfCreation = time;

                counter++;
            }
            i++;

        }
    }

    public void UpdateCannon()
    {
        double something = rnd.NextDouble();
        int particlesThisSecond = RandomMinMaxInt(something, 0, 1, settingsCascade.minimumParticlesPerSecond, settingsCascade.maximumParticlesPerSecond);

        int counter = 0;
        int i = 0;

        while (i < particles.Length && counter < particlesThisSecond)
        {
            if (!particles[i].alive)
            {
                particles[i].alive = true;

                particles[i].position = settingsCannon.Start;

                double randomForce = rnd.NextDouble();
                float force = RandomMinMaxFloat(randomForce, 0, 1, settingsCannon.minimumForce, settingsCannon.maximumForce);

                Vector3C forceDirection = settingsCannon.Direction.normalized;

                double randomAngle = rnd.NextDouble();
                float angle = RandomMinMaxFloat(randomAngle, 0, 1, 0, settingsCannon.angle);

                double randomEje = rnd.NextDouble();
                int eje = RandomMinMaxInt(randomEje, 0,1, 0, 2);

                if (eje == 0)
                    forceDirection = RotateAroundX(forceDirection, angle).normalized;
                else if(eje == 1)
                    forceDirection = RotateAroundY(forceDirection, angle).normalized;
                else
                    forceDirection = RotateAroundZ(forceDirection, angle).normalized;

                particles[i].acceleration = (forceDirection) * force;

                double randomLife = rnd.NextDouble();
                particles[i].life = RandomMinMaxFloat(randomLife, 0, 1, settingsCannon.minimumParticlesLife, settingsCannon.maximumParticlesLife);

                particles[i].timeOfCreation = time;

                counter++;


            }

            i++;
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

    Vector3C RotateAroundX(Vector3C input, float angleInDegrees)
    {
        float angleInRadians = UnityEngine.Mathf.Deg2Rad * angleInDegrees;
        float cosAngle = UnityEngine.Mathf.Cos(angleInRadians);
        float sinAngle = UnityEngine.Mathf.Sin(angleInRadians);
        return new Vector3C(
           input.x,
           cosAngle * input.y - sinAngle * input.z,
           sinAngle * input.y + cosAngle * input.z
       );
    }

    Vector3C RotateAroundY(Vector3C input, float angleInDegrees)
    {
        float angleInRadians = UnityEngine.Mathf.Deg2Rad * angleInDegrees;
        float cosAngle = UnityEngine.Mathf.Cos(angleInRadians);
        float sinAngle = UnityEngine.Mathf.Sin(angleInRadians);
        return new Vector3C(
             cosAngle * input.x + sinAngle * input.z,
            input.y,
            -sinAngle * input.x + cosAngle * input.z
       );
    }

    Vector3C RotateAroundZ(Vector3C input, float angleInDegrees)
    {
        float angleInRadians = UnityEngine.Mathf.Deg2Rad * angleInDegrees;
        float cosAngle = UnityEngine.Mathf.Cos(angleInRadians);
        float sinAngle = UnityEngine.Mathf.Sin(angleInRadians);
        return new Vector3C(
           cosAngle * input.x - sinAngle * input.y,
            sinAngle * input.x + cosAngle * input.y,
            input.z
       );
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
