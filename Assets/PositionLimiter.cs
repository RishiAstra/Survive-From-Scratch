using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLimiter : MonoBehaviour
{
    public Vector3 min, max;
    public float scaleMult;
    public Transform relativeTo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 p = transform.position;
        Vector3 d = transform.position - relativeTo.position;
        d = relativeTo.InverseTransformVector(d) * scaleMult;
        d.x = Mathf.Clamp(d.x, min.x, max.x);
        d.y = Mathf.Clamp(d.y, min.y, max.y);
        d.z = Mathf.Clamp(d.z, min.z, max.z);
        d = relativeTo.TransformVector(d) / scaleMult;
        //GetComponent<Rigidbody>().position = relativeTo.position + d;
        //print(p - relativeTo.position + d);
        transform.position = relativeTo.position + d;
    }
}
