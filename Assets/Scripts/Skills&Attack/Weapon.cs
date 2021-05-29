/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: allow hitting multiple enemies
public class Weapon : MonoBehaviour
{
	public AttackType attackType;
	public Abilities parent;
	[Range(0, 2)]public float dmg;
	public bool canDmg;
	public bool multipleHits = true;//can it hit multiple enemies with same swing etc.
	public float attackDuriation;//StopAttack() will prevent this, so no need. Set it to large value, larger than realistic attack duriation (e.g. if sword takes 1 sec to swing, set to 2 sec)
	//public Equip eq;

	private float timeSinceAttack;
	private List<StatScript> hit;
    // Start is called before the first frame update
    void Start()
    {
		parent = GetComponentInParent<Abilities>();
    }

    // Update is called once per frame
    void Update()
    {
		timeSinceAttack += Time.deltaTime;
		if(timeSinceAttack > attackDuriation)
		{
			canDmg = false;
		}
    }

	public void Attack1()
	{
		canDmg = true;
		hit = new List<StatScript>();
		timeSinceAttack = 0;
	}

	public void StopAttack1()
	{
		canDmg = false;
	}

	//void OnTriggerEnter(Collider other)
	//{
	//	if (canDmg)
	//	{
	//		LayerMask mask = gameControll.main.weaponHit.value;
	//		if(mask == (mask | (1 << other.gameObject.layer)))//if included in the mask
	//		{
	//			Abilities bg = other.GetComponentInParent<Abilities>();
	//			if(bg != null)
	//			{
	//				bg.Damage(dmg, attackType);
	//				canDmg = false;
	//			}
	//		}
	//	}
		
	//}

	private void OnTriggerStay(Collider other)
	{
		if (canDmg)
		{
			//LayerMask mask = gameControll.main.weaponHit.value;
			//if (mask == (mask | (1 << other.gameObject.layer)))//if included in the mask
			//{

			//if this is an enemy...
			TagScript ts = other.GetComponentInParent<TagScript>();
			if (ts != null && ts.ContainsTag(parent.enemyString))
			{
				StatScript bg = ts.GetComponent<StatScript>();
				if (bg != null && bg != parent && !hit.Contains(bg))
				{
					hit.Add(bg);
					bg.Damage(parent.myStat.GetOutputDamageAmount(attackType), parent, other, attackType, other.ClosestPoint(transform.position));
					if(!multipleHits) canDmg = false;
				}
			}
				
				
			//}
		}
	}
}
