using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: allow hitting multiple enemies
public class Weapon : MonoBehaviour
{
	public Abilities.AttackType attackType;
	public Abilities parent;
	public float dmg;
	public bool canDmg;
	public float attackDuriation;//StopAttack() will prevent this, so no need. Set it to large value, larger than realistic attack duriation (e.g. if sword takes 1 sec to swing, set to 2 sec)
	//public Equip eq;

	private float timeSinceAttack;
	private List<Abilities> hit;
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
		hit = new List<Abilities>();
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
				Abilities bg = other.GetComponentInParent<Abilities>();
				if (bg != null && bg != parent && ! hit.Contains(bg))
				{
					hit.Add(bg);
					bg.Damage(dmg, other, attackType);
					//canDmg = false;
				}
				
			//}
		}
	}
}
