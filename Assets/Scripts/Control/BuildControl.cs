/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
	x,
	y, 
	z,
}

public class BuildControl : MonoBehaviour
{
	public static BuildControl main;

	public bool building;
	public Axis a;
	public GameObject x;
	public GameObject y;
	public GameObject z;
	public Transform ghostFollower;
	public LayerMask avoidOverlap;//don't put buildings here though
	public LayerMask blockSight;


	// Start is called before the first frame update
	void Awake()
    {
		if (main != null) Debug.LogError("two BuildControls");
		main = this;
	}

	void SetAxis(Axis axis)
	{
		a = axis;
		DisableAxes();

		if (a == Axis.x)
		{
			x.SetActive(true);
		}
		else if (a == Axis.y)
		{
			y.SetActive(true);
		}
		else if (a == Axis.z)
		{
			z.SetActive(true);
		}
	}

	private void DisableAxes()
	{
		x.SetActive(false);
		y.SetActive(false);
		z.SetActive(false);
	}

	// Update is called once per frame
	void Update()
    {
		if (building)
		{
			SetAxis(a);//TODO: this is wasteful
			if (Input.GetKeyDown(KeyCode.T))
			{
				if (a == Axis.x)
				{
					SetAxis(Axis.y);
				}
				else if (a == Axis.y)
				{
					SetAxis(Axis.z);
				}
				else if (a == Axis.z)
				{
					SetAxis(Axis.x);
				}
			}
		}
		else
		{
			DisableAxes();
		}
		
	}
}
