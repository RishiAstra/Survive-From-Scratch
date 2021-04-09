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
			x.SetActive(false);
		}
		else if (a == Axis.y)
		{
			y.SetActive(false);
		}
		else if (a == Axis.z)
		{
			z.SetActive(false);
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
			if (Input.GetKeyDown(KeyCode.E))
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
