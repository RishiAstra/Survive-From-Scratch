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
	public bool building;
	public Axis a;
	public GameObject x;
	public GameObject y;
	public GameObject z;

	public GameObject placing;
    // Start is called before the first frame update
    void Start()
    {
        
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
		if (placing)
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				float toRotate = 90;
				if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					toRotate *= -1;
				}

				if(a == Axis.x)
				{
					placing.transform.Rotate(Vector3.right, toRotate);
				}else if (a == Axis.y)
				{
					placing.transform.Rotate(Vector3.up, toRotate);
				}
				else if(a == Axis.z)
				{
					placing.transform.Rotate(Vector3.forward, toRotate);
				}
			}

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
