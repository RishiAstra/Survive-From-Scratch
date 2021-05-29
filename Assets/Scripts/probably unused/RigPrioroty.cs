/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigPrioroty : MonoBehaviour {
	//public float simulateDist;
	public float sleep;

	public Rigidbody rig;
	// Use this for initialization
	void Start () {
		rig.sleepThreshold = sleep;
	}
	
	// Update is called once per frame
	void Update () {
		//rig.isKinematic = Vector3.Distance(transform.position, bobPlayer.main.transform.position) > simulateDist;
		//if ()
		//{
		//	rig.Sleep();
		//}
	}
}
