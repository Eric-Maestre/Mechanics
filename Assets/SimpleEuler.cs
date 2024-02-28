using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEuler : MonoBehaviour
{
    Vector3 velocidad;
    private void Update()
    {
        velocidad += Physics.gravity * Time.deltaTime;
        transform.position += velocidad * Time.deltaTime;
    }
}
