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

    // Update is called once per frame
    void Update()
    {
        //nearestPoint = lineC.NearestPointToPoint(point);

        //CustomDebug.Print(lineC, colorRed, false);
        //CustomDebug.Print(point, 5);
        //CustomDebug.Print(nearestPoint, 5);
        //Debug.DrawLine(point.ToUnity(),nearestPoint.ToUnity(), Color.magenta, 5f);

        CustomDebug.Print(planeC, colorRed, 100);

    }
}
