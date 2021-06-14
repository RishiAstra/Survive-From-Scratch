/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkillObject))]
public class CollideDamage : MonoBehaviour
{

	public List<GameObject> hit;//use this to avoid hitting the same enemy twice with multiple colliders. Otherwise, the enemy might take damage twice.
	public float maxDamage;
	public float minDamage;
	public float maxVelocity;
	public float minVelocity;
	public AttackType attackType;
	public int collisionsTilldeactive = 1;//must hit 1 thing to stop dealing dmg
	public float minTimeAcive = 5f;//deal damage on collision for at leas this long
	public float bonusActiveTime = 1f;//add more time active if it hits something

	private SkillObject so;
	private int collisionCount;
	private float timeActive = 0;
    // Start is called before the first frame update
    void Start()
    {
		hit = new List<GameObject>();
		so = GetComponent<SkillObject>();
    }

    // Update is called once per frame
    void Update()
    {
		timeActive += Time.deltaTime;
		if (shouldExist())
		{
			Destroy(this);//no longer want to deal dmg on collision
		}
	}

	private bool shouldExist()
	{
		return so.parent == null || (timeActive > minTimeAcive && collisionCount >= collisionsTilldeactive);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (shouldExist()) return;
		timeActive -= bonusActiveTime;
		if (timeActive < 0) timeActive = 0;
		collisionCount++;

		TagScript ts = collision.gameObject.GetComponentInParent<TagScript>();

		if(ts != null && ts.ContainsTag(so.parent.enemyString))
		{

			float v = collision.relativeVelocity.magnitude;
			StatScript b = ts.GetComponent<StatScript>();
			if(b != null)
			{
				if (hit.Contains(collision.gameObject))
				{

				}
				else
				{
					hit.Add(collision.gameObject);
					if(v > minVelocity)
					{
						float d = (v - minVelocity) / (maxVelocity - minVelocity);//Get how far between max and min velocity v is
						if (d > 1) d = 1;//cap damage at maxDamage
						float damage = minDamage + (maxDamage - minDamage) * d;//min damage plus the extra from higher velocity
						//print("Collide damage: " + damage);
						b.Damage(so.GetDamageAmount(attackType, damage), so.parent, collision.collider, attackType, collision.contacts[0].point);
						timeActive = 0;//make it active again so it can bounce and hit more enemies
					}
				}
			
			}
		}

		
	}

	void OnCollisionExit(Collision collision)
	{
		float v = collision.relativeVelocity.magnitude;
		badguy b = collision.gameObject.GetComponent<badguy>();
		if (b != null)
		{
			hit.Remove(collision.gameObject);
			//if (v > minVelocity)
			//{
			//	float d = (v - minVelocity) / (maxVelocity - minVelocity);//Get how far between max and min velocity v is
			//	if (d > 1) d = 1;//cap damage at maxDamage
			//	float damage = minDamage + (maxDamage - minDamage) * d;//min damage plus the extra from higher velocity
				print("exit collision");//" + damage);
			//	b.Damage(damage);
			//}
		}
	}
}
