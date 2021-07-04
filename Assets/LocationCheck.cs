using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCheck : MonoBehaviour
{
	public static List<LocationCheck> instances = new List<LocationCheck>();

	public string thisLocationName;
	// Start is called before the first frame update
	void Start()
	{
		instances.Add(this);   
	}

	private void OnDestroy()
	{
		instances.Remove(this);
	}

	public static LocationCheck GetInstance(string nameToGet)
	{
		foreach (LocationCheck d in instances)
		{
			if (d.thisLocationName == nameToGet)
			{
				return d;
			}
		}

		return null;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnTriggerEnter(Collider other)
	{
		NPCControl ctrl = other.GetComponent<NPCControl>();
		//if it's the player who entered this trigger
		if(ctrl == GameControl.main.playerControl)
		{
			ProgressTracker.main.RegisterLocationVisit(thisLocationName);
		}
	}
}
