/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component is meant to optimize the performance of rigidbodies, expecially those attached to items
/// Right now, it only removes the rigidbody when it sleeps
/// </summary>
public class RigPrioroty : MonoBehaviour {
	//public float simulateDist;
	//public float rigRemoveThreshold ;

	private Rigidbody rig;
	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (rig != null && rig.IsSleeping())
		{
			Destroy(rig);//remove the rigidbody since it's not needed
			gameObject.isStatic = true;//mark as static since it won't move
		}
		//rig.isKinematic = Vector3.Distance(transform.position, bobPlayer.main.transform.position) > simulateDist;
		//if ()
		//{
		//	rig.Sleep();
		//}
	}
}
