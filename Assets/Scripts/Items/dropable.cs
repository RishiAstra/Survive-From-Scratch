/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropable : MonoBehaviour {
	public Equip me;
	//public int id;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Q))
		{
			int times = 1;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				times *= 5;
			}
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				times *= 25;
			}
			StartCoroutine(Drop(times));
		}
	}
	IEnumerator Drop (int amount)
	{
		//print(amount);
		
		for (int i = 0; i < amount; i++)
		{
			//print(me.bob.inventory[me.bob.invSel].amount >= 1);
			if (me.bob.inv.items[me.bob.invSel].amount >= 1)
			{
				me.bob.RemoveItem(me.bob.invSel, 1);
				GameObject g = Instantiate(GameControl.itemTypes[me.myID.id].prefab, transform.position, transform.rotation);
				Rigidbody rig = g.GetComponent<Rigidbody>();
				if (rig != null)
				{
					rig.AddForce(me.bob.rig.velocity, ForceMode.VelocityChange);
				}
				//print("before");
				yield return null;
				//print("after");
			}
		}
		yield return null;
	}
}
