using System;
using System.Net.NetworkInformation;

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
        float dot = Vector3C.Dot(vector, this.normal);
        Vector3C nearestPoint = point - this.normal*dot;
        return nearestPoint;
    }
    public Vector3C IntersectionWithLine(LineC line)
    {
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