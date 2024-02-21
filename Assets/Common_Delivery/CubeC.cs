using System;

[System.Serializable]
public struct CubeC
{
    #region FIELDS
    Vector3C position;
    float scale, rotation, radius;
    #endregion

    #region PROPIERTIES
    #endregion

    #region CONSTRUCTORS
    #endregion

    #region OPERATORS
    //public static bool operator >(Vector3C a, Vector3C b)
    //{
    //    return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
    //}
    #endregion

    #region METHODS
    public bool isInside()
    {
        return false;
    }
    public Vector3C NearestPoint(Vector3C point)
    {
        return new Vector3C();
    }
    public Vector3C IntersectionWithLine(LineC line)
    {
        return new Vector3C();
    }
    public bool Equals(CubeC cube)
    {
        if(this.scale == cube.scale && this.rotation == cube.rotation && this.radius == cube.radius)
            return true;
        else
            return false;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}