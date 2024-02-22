using System;
using System.Net.NetworkInformation;
using System.Numerics;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public struct PlaneC
{
    #region FIELDS
    public Vector3C position;
    public Vector3C normal;
    #endregion

    #region PROPIERTIES
    public static PlaneC right
    {
        get { return new PlaneC(new Vector3C(0, 0, 0), new Vector3C(1, 0, 0)); }
    }
    public static PlaneC up
    {
        get { return new PlaneC(new Vector3C(0, 0, 0), new Vector3C(0, 1, 0)); }
    }
    public static PlaneC forward
    {
        get { return new PlaneC(new Vector3C(0, 0, 0), new Vector3C(0, 0, 1)); }
    }

    #endregion

    #region CONSTRUCTORS
    public PlaneC(Vector3C position, Vector3C normal)
    {
        this.position = position;
        this.normal = normal;
    }
    public PlaneC(Vector3C n, float D)
    {
        this.position = new Vector3C();
        this.normal = new Vector3C();
    }
    public PlaneC(Vector3C pointA, Vector3C pointB, Vector3C pointC)
    {
        Vector3C vectorAB = pointB - pointA;
        Vector3C vectorAC = pointC - pointA;
        Vector3C normal = Vector3C.Cross(vectorAB, vectorAC);
        this.normal = normal;
        this.position = pointA;
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public (float A, float B, float C, float D) ToEquation()
    {
        return (0, 0, 0, 0);
    }
    //Esta habrá que comprobarla
    public Vector3C NearestPoint(Vector3C point)
    {
        Vector3C vector = point - this.position;
        float dot = Vector3C.Dot(vector, this.normal.normalized);
        Vector3C nearestPoint = point - this.normal.normalized*dot;
        return nearestPoint;
    }
    public Vector3C IntersectionWithLine(LineC line)
    {
        //Give the equation of the plane with normal vector (10, 8, 3) that contains the point (10, 5, 5):
        //    Recall that the scalar form of the equation of a plane with a normal vector n = (a, b, c),
        //    that contains the point(x0, y0, z0), is a(x-x0) + b(y - y0) + c(z - z0) = 0.
        //Thus, substituting the values for the given normal vector(10, 8, 3) and point(10, 5, 5), we have
        //    10(x - 10) + 8(y - 5) + 3(z - 5) = 0
        //    10x - 100 + 8y - 40 + 3z - 15 = 0
        //    10x + 8y + 3z - 155 = 0
        //    Thus, the general form of the equation of the plane with normal vector(10, 8, 3) that contains
        //    the point(10, 5, 5) is 10x + 8y + 3z - 155 = 0

        //PlaneC.normal.x(x - PlaneC.position.x) + PlaneC.normal.y(y - PlaneC.position.y) + PlaneC.normal.z(z - PlaneC.position.z) = 0
        //PlaneC.normal.x*x + PlaneC.normal.y*y + PlaneC.normal.z*z
        //- PlaneC.normal.x*PlaneC.position.x - PlaneC.normal.y*PlaneC.position.y - PlaneC.normal.z*PlaneC.position.z = 0
        return new Vector3C();
    }
    public bool Equals(PlaneC other)
    {
        if(this.normal ==  other.normal) 
            return true;
        else
            return false;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}