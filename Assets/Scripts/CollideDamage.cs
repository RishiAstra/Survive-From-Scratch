using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideDamage : MonoBehaviour
{

	public List<GameObject> hit;//use this to avoid hitting the same enemy twice with multiple colliders. Otherwise, the enemy might take damage twice.
	public float maxDamage;
	public float minDamage;
	public float maxVelocity;
	public float minVelocity;
    // Start is called before the first frame update
    void Start()
    {
		hit = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnCollisionEnter(Collision collision)
	{
		float v = collision.relativeVelocity.magnitude;
		badguy b = collision.gameObject.GetComponent<badguy>();
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
					print("Collide damage: " + damage);
					b.Damage(damage);
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
