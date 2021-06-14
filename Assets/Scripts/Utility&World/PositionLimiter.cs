/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLimiter : MonoBehaviour
{
    public Vector3 min, max;
	[Range(0, 1)]
	public float bounce;
    public float scaleMult;
    public Transform relativeTo;
	public Rigidbody rig;

	private Vector3 p;
	private Vector3 v;
    // Start is called before the first frame update
    void Start()
    {
        
    }

	private void FixedUpdate()
	{
		v = relativeTo.position - p;
		v /= Time.fixedDeltaTime;
		p = relativeTo.position;
	}

	// Update is called once per frame
	void LateUpdate()
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
