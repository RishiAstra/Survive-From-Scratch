/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
	public Equip me;
	public float spawnDist;//don't overlap, spawn this distance away
	public float velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (RegionSettings.main.allowCombat && Input.GetMouseButtonDown(1) && Cursor.lockState == CursorLockMode.Locked)
		{
			//print(me.bob.inventory[me.bob.invSel].amount >= 1);
			if (me.bob.inv.items[me.bob.invSel].amount >= 1)
			{
				me.bob.RemoveItem(me.bob.invSel, 1);
				GameObject g = Instantiate(GameControl.itemTypes[me.myID.id].prefab, transform.position + me.bob.cam.forward * spawnDist, transform.rotation);
				SkillObject so = g.GetComponent<SkillObject>();
				if (so != null) so.parent = me.bob.GetComponent<Abilities>();
				Rigidbody rig = g.GetComponent<Rigidbody>();
				if (rig != null)
				{
					rig.AddForce(me.bob.rig.velocity + me.bob.cam.forward * velocity, ForceMode.VelocityChange);
				}
			}
		}
	}
}
