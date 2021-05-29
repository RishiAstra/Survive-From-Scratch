/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using UnityEngine;
using System.Collections;

public class spawnPoint : MonoBehaviour {
	public bool autoHeight = true;
	public LayerMask putOnTopOfThese;
	// Use this for initialization
	void Start () {
		RaycastHit hit;
		Physics.Raycast (transform.position + Vector3.up * 100, -Vector3.up, out hit, 200, putOnTopOfThese);
		transform.position = hit.point + Vector3.up;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, 1f);
		Gizmos.DrawLine(transform.position, transform.position - Vector3.up * 10);
	}

	// Update is called once per frame
	//	void Update () {
	//	
	//	}
}
