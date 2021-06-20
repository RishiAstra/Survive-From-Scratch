﻿/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: make camera zoom out on death
public class Cam : MonoBehaviour
{
	public const float ZOOM_LERP_SPEED = 0.2f;

	//public Player myPlayer;
	public Transform pivot;
	public Transform mapIcon;
	public LayerMask blockCamera;
	//public float wait;
	//public Vector2 mouseSensitivity;
	//public bool invertMouseY;
	public Vector3 offset;
	public float dist;
	public float actualDist;
	public float minDist;
	public float maxDist;
	//public float scrollSencitivity;

	public float minX;
	public float maxX;
	//private List<float> f = new List<float>();
	//private float temp = 0;
	// Use this for initialization
	void Start()
	{
		//myPlayer = GetComponentInParent<Player>();//TODO: dangerous
		dist = transform.localPosition.magnitude;
		offset = transform.localPosition / dist;
		mapIcon = GameControl.main.mapPlayerIcon;
	}

	public void AddPitch(float p)
	{
		Vector3 temp = pivot.eulerAngles;
		temp.x = Mathf.Clamp(Mathf.DeltaAngle(0, pivot.eulerAngles.x + p), minX, maxX);
		pivot.eulerAngles = temp;
	}

	public void AddDist(float d)
	{
		dist += d;
		dist = Mathf.Clamp(dist, minDist, maxDist);
	}

	private void Update()
	{
		mapIcon.position = pivot.position;
		mapIcon.localEulerAngles = pivot.localEulerAngles.y * Vector3.up;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		

		RaycastHit hit;
		//ignore trigers because they generally represent things that don't collide and shouldn't force the camera to zoom in
		if(Physics.Raycast(pivot.position, transform.TransformVector(offset), out hit, dist, blockCamera, QueryTriggerInteraction.Ignore))
		{
			//hit.distance;
			actualDist = Mathf.Clamp(dist, minDist, hit.distance);
		}
		else
		{
			actualDist = dist;
		}

		transform.localPosition = offset * Mathf.Lerp(transform.localPosition.magnitude, actualDist, ZOOM_LERP_SPEED);
	}
}
