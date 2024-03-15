using System;
using System.Collections.Generic;
using TreeEditor;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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

        //Peta

        //CustomDebug.Print(cascade, new Vector3C(255f, 0f, 0f), false);

        //for (int i = 0; i < settingsCollision.spheres.Length; i++)
        //    CustomDebug.Print(settingsCollision.spheres[i], new Vector3C(0f, 0f, 255f));

        if (time == 0)
        {
            cascade = new LineC(settingsCascade.PointA, settingsCascade.PointB);
            particles = new Particle[settings.poolCapacity];
            for(int i = 0; i < particles.Length; i++)
            {
                particles[i].alive = false;
                particles[i].size = 0.05f;
            }
        }

        UpdateCascade();
        UpdateCannon();

        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i].AddForce(settings.gravity * dt);
            particles[i].lastPosition = particles[i].position;
            particles[i].position += particles[i].acceleration * dt;
            if (particles[i].life <= time - particles[i].timeOfCreation)
            {
                particles[i].alive = false;
            }

            for (int j = 0; j < settingsCollision.planes.Length; ++j)
            {
                Vector3C distanceVector = particles[i].position - settingsCollision.planes[j].NearestPoint(particles[i].position);
                float distance = distanceVector.magnitude;
                float factor = particles[i].size;
                bool collision = false;
                bool passed = false;
                if (distance <= factor*2)
                    passed = true;
                if (distance <= particles[i].size + factor)
                    collision = true;
                if (collision)
                {
                    int counter = 2;
                    while (passed)
                    {
                        particles[i].position = particles[i].lastPosition;
                        particles[i].position += particles[i].acceleration * dt/counter;
                        distanceVector = particles[i].position - settingsCollision.planes[j].NearestPoint(particles[i].position);
                        distance = distanceVector.magnitude;

                        if (distance > 0)
                            passed = false;
                        else
                            counter *= 2;
                    }
                    //Calcular componente normal
                    float vnMagnitude = Vector3C.Dot(particles[i].acceleration, settingsCollision.planes[j].normal);
                    Vector3C vn = settingsCollision.planes[j].normal * vnMagnitude;
                    //Calcular componente tangencial
                    Vector3C vt = particles[i].acceleration - vn;
                    //Calcular nueva velocidad
                    Vector3C newVelocity = -vn + vt;

                    particles[i].AddForce(-(particles[i].acceleration));
                    particles[i].AddForce(newVelocity * settings.bounce);


                    collision = false;

                }
            }
            //SPHERES
            for (int k = 0; k < settingsCollision.spheres.Length; ++k)
            {
                bool collision = false;
                float factor = particles[i].size;
                bool passed = false;
                Vector3C nearestPoint = settingsCollision.spheres[k].NearestPoint(particles[i].position);
                Vector3C distanceVector = particles[i].position - settingsCollision.spheres[k].position;
                
                float distance = distanceVector.magnitude;
                if (distance <= settingsCollision.spheres[k].radius + factor)
                    passed = true;
                if (distance <= settingsCollision.spheres[k].radius + particles[i].size + factor)
                {
                    UnityEngine.Debug.Log("SphereCollision");
                    collision = true;
                    int counter = 2;
                    while (passed)
                    {
                        particles[i].position = particles[i].lastPosition;
                        particles[i].position += particles[i].acceleration * dt / counter;
                        distanceVector = particles[i].position - settingsCollision.spheres[k].position;
                        distance = distanceVector.magnitude;

                        if (distance <= settingsCollision.spheres[k].radius + particles[i].size + factor)
                            passed = false;
                        else
                            counter *= 2;
                    }

                    //NearestPoint circulo a la partícula
                    //Usando la distancia de este punto al centro hago un vector que me servira como normal
                    //Con un punto y una normal hago un plano
                    //Repito el rebote visto en el plano

                    Vector3C normal = settingsCollision.spheres[k].NearestPoint(particles[i].position) - settingsCollision.spheres[k].position;
                    //Calcular componente normal
                    float vnMagnitude = Vector3C.Dot(normal, particles[i].acceleration);
                    Vector3C vn = normal * vnMagnitude;
                    //Calcular componente tangencial
                    Vector3C vt = particles[i].acceleration - vn;
                    //Calcular nueva velocidad
                    Vector3C newVelocity = -vn + vt;
                    //particles[i].acceleration *= 0;
                    particles[i].AddForce(-(particles[i].acceleration));
                    particles[i].AddForce(newVelocity * settings.bounce);
                }
         
            }
            //CAPSULES
            //Vector v del punto A a la partícula
            //Vector w del punto A al punto B
            //Proyección del vector v en el vector w
            //Crear vector entre el final de la proyección y la partícula
            for (int l = 0; l < settingsCollision.capsules.Length; ++l)
            {
                float factor = particles[i].size;
                Vector3C particulaPuntoA = particles[i].position - settingsCollision.capsules[l].positionA;
                Vector3C BA = settingsCollision.capsules[l].positionB - settingsCollision.capsules[l].positionA;
                float proyeccionVenWValue = (Vector3C.Dot(particulaPuntoA, BA));
                Vector3C proyeccionVenW = BA.normalized * proyeccionVenWValue;
                Vector3C particleNearestCapsulePoint = particles[i].position - proyeccionVenW;
                bool passed = false;

                float dist = particleNearestCapsulePoint.magnitude;

                if (dist <= settingsCollision.capsules[l].radius + factor)
                {
                    passed = true;
                }

                if (dist <= settingsCollision.spheres[l].radius + particles[i].size + factor)
                {
                    UnityEngine.Debug.Log("CapsuleCollision");

                    int counter = 2;
                    while (passed)
                    {
                        particles[i].position = particles[i].lastPosition;
                        particles[i].position += particles[i].acceleration * dt / counter;

                        particulaPuntoA = particles[i].position - settingsCollision.capsules[l].positionA;
                        BA = settingsCollision.capsules[l].positionB - settingsCollision.capsules[l].positionA;
                        proyeccionVenWValue = (Vector3C.Dot(particulaPuntoA, BA));
                        proyeccionVenW = BA.normalized * proyeccionVenWValue;
                        particleNearestCapsulePoint = particles[i].position - proyeccionVenW;
                        dist = particleNearestCapsulePoint.magnitude;

                        if (dist <= settingsCollision.capsules[l].radius + particles[i].size + factor)
                            passed = false;
                        else
                            counter *= 2;
                    }

                    //NearestPoint circulo a la partícula
                    //Usando la distancia de este punto al centro hago un vector que me servira como normal
                    //Con un punto y una normal hago un plano
                    //Repito el rebote visto en el plano


                    //No se si esta bien

                    Vector3C normal = proyeccionVenW;
                    //Calcular componente normal
                    float vnMagnitude = Vector3C.Dot(normal, particles[i].acceleration);
                    Vector3C vn = normal * vnMagnitude;
                    //Calcular componente tangencial
                    Vector3C vt = particles[i].acceleration - vn;
                    //Calcular nueva velocidad
                    Vector3C newVelocity = -vn + vt;

                    particles[i].AddForce(-(particles[i].acceleration));
                    particles[i].AddForce(newVelocity * settings.bounce);
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

                double randomAlpha = rnd.NextDouble();

                particles[i].position = InterpolatePoint(settingsCascade.PointA, settingsCascade.PointB, randomAlpha);

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
                particles[i].life = RandomMinMaxFloat(randomLife, 0, 1, settingsCascade.minimumParticlesLife, settingsCascade.maximumParticlesLife);
               

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

    private Vector3C InterpolatePoint(Vector3C pointA, Vector3C pointB, double t)
    {
        float x = (float)((1 - t) * pointA.x + t * pointB.x);
        float y = (float)((1 - t) * pointA.y + t * pointB.y);
        float z = (float)((1 - t) * pointA.z + t * pointB.z);

        return new Vector3C(x, y, z);

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
