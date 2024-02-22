using System;

[System.Serializable]
public struct LineC
{
    #region FIELDS
    public Vector3C origin;
    public Vector3C direction;
    #endregion

    #region PROPIERTIES
    #endregion

    #region CONSTRUCTORS
    public LineC(Vector3C origin, Vector3C direction)
    {
        this.origin = origin;
        this.direction = direction;
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    #endregion
    public  Vector3C NearestPointToPoint(Vector3C point)
    {
        Vector3C vector = point - origin;
        float dot = Vector3C.Dot(vector, direction.normalized);
        Vector3C nearestPoint = origin + direction.normalized * dot;
        return nearestPoint;
    }

    #region FUNCTIONS
    public static LineC From(Vector3C pointA, Vector3C pointB )
    {
        Vector3C origin = pointA;
        Vector3C direction = pointB - pointA;
        LineC line = new LineC(origin, direction);
        return line;
    }

    #endregion

}