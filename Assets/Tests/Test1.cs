using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[ExecuteAlways]
public class Test1 : MonoBehaviour
{
   
    // Start is called before the first frame update
    
    Vector3C colorRed = new Vector3C(255f, 0f, 0f);
    [SerializeField]
    LineC lineC;
    [SerializeField]
    Vector3C point;
    [SerializeField]
    PlaneC planeC;
    private Vector3C nearestPoint;
    private Vector3C intersectionWithLine;

    // Update is called once per frame
    void Update()
    {
        //nearestPoint = lineC.NearestPointToPoint(point);
        CustomDebug.Print(point, 5);
        nearestPoint = planeC.NearestPoint(point);
        CustomDebug.Print(lineC, colorRed, false);
        intersectionWithLine = planeC.IntersectionWithLine(lineC);
        CustomDebug.Print(nearestPoint, 5);
        CustomDebug.Print(intersectionWithLine, 5);
        Debug.DrawLine(point.ToUnity(),nearestPoint.ToUnity(), Color.magenta, 5f);
        Debug.Log(nearestPoint.x);
        Debug.Log(nearestPoint.y);
        Debug.Log(nearestPoint.z);

    }
    private void OnDrawGizmos()
    {
        CustomDebug.Print(planeC, colorRed, 10);
    }
}
