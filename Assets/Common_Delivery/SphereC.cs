using System;

[System.Serializable]
public struct SphereC
{
    #region FIELDS
    public Vector3C position;
    public float radius;
    #endregion

    #region PROPIERTIES
    public static SphereC unitary
    {
        get { return new SphereC(new Vector3C(0,0,0), 1); }
    }
    #endregion

    #region CONSTRUCTORS
    public SphereC (Vector3C position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public bool IsInside(SphereC other)
    {
        float distanceFromCenter = other.position.Magnitude() - position.Magnitude();
        return radius - distanceFromCenter > 0;
    }
    public Vector3C NearestPoint(Vector3C point)
    {
        Vector3C pointToCenterVector = point - position;
        pointToCenterVector.Normalize();
        return pointToCenterVector * radius;
    }
    public Vector3C IntersectionWithLine(LineC line)
    {
        return new Vector3C();
    }
    public bool Equals (SphereC other)
    {
        if(other.radius == radius)
            return true;
        else 
            return false;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}