using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

[System.Serializable]
public struct Stat
{
	public float hp;//health points, if reaches 0 die
	public float mp;//mana points, can be used to cast spells or skills
	public float eng;//energy, used for pretty much everything especially big attacks or running
	public float mor;//morale, effects stats
	public float atk;//attack power
	//TODO: defence should be implemented in colliders that can have different defence values and resistances
	public float def;//defence
	public float mag;//

	public static bool StatEquals(Stat c1, Stat c2)
	{
		return
			c1.hp == c2.hp &&
			c1.mp == c2.mp &&
			c1.eng == c2.eng &&
			c1.mor == c2.mor &&
			c1.atk == c2.atk &&
			c1.def == c2.def &&
			c1.mag == c2.mag;
	}
}
[System.Serializable]
public struct Armor
{
	public Collider[] col;
	public float pierceResist;
	public float bluntResist;
	public float slashResist;
	public float magicResist;

	public bool HasCollider(Collider c)
	{
		return col.Contains(c);
	}
}

public enum AttackType
{
	Pierce = 1,
	Blunt = 2,
	Slash = 4,
	Magic = 8
};

public class Abilities : MonoBehaviour, ISaveable
{
	public const float RESIST_EXPONENT_BASE = 2f;



	public bool dead;//TODO: consider stopping all attacks already happening when it dies
	public bool resetOnStart = true;
	public Stat maxStat;
	public Stat stat;
	public List<Skill> skills;
	public List<Armor> armors;
	public bool busy;
	public bool attackAllowed;
	public Animator anim;
	public float dieTime = 2.5f;

	public int currentAttackTransform;
	public Transform[] attackTranforms;

	public string[] enemyString;

	private void Awake()
	{
		if (resetOnStart) ResetStats();//reset before anythign else can happen

	}
	// Start is called before the first frame update
	void Start()
	{
	}

	public void ResetStats()
	{
		stat = maxStat;
	}

	// Update is called once per frame
	void Update()
	{
		if(stat.hp <= 0 && !dead)
		{
			dead = true;
			Destroy(gameObject, dieTime);
		}
	}

	#region Attack and Defense

	public void Attack()
	{
		if (dead) return;
		if (attackAllowed)
		{
			foreach (Weapon w in attackTranforms[currentAttackTransform].GetComponentsInChildren<Weapon>())
			{
				w.Attack1();
			}
		}
	}

	public void StopAttack()
	{
		foreach (Weapon w in attackTranforms[currentAttackTransform].GetComponentsInChildren<Weapon>())
		{
			w.StopAttack1();
		}
	}

	public void Damage(float dmg, Collider cols, AttackType type)//accouns for weakpoints in different armor pieces
	{
		Damage(dmg, cols, type, cols.bounds.center);
	}

	public void Damage(float dmg, Collider cols, AttackType type, Vector3 damagePosition)//accouns for weakpoints in different armor pieces
	{
		Armor a = armors.Count > 0 ? armors[0] : new Armor();//default to first
		foreach(Armor t in armors)
		{
			if (t.HasCollider(cols))
			{
				a = t;
			}
		}
		float damageTaken = GetDamageAmount(dmg, type, a);
		stat.hp -= damageTaken;
		DamageTextControl.PutDamageText(cols.bounds.center, damageTaken);
	}
	public float GetDamageAmount(float damage, AttackType type, Armor armor)
	{
		float armorValue = 0;
		if (type == AttackType.Pierce)	armorValue = armor.pierceResist;
		if (type == AttackType.Blunt)	armorValue = armor.bluntResist;
		if (type == AttackType.Slash)	armorValue = armor.slashResist;
		if (type == AttackType.Magic)	armorValue = armor.magicResist;
		float mult = 1 - Mathf.Pow(RESIST_EXPONENT_BASE, -damage / armorValue);
		return damage * mult;
	}

	/// <summary>
	/// Uses a skill. It's ok to call this every frame since it won't execute the skill if busy.
	/// </summary>
	/// <param name="i"></param>
	public void UseSkill(int i)
	{
		if (dead) return;
		if (!busy)
		{
			busy = true;
			anim.SetBool("Attacking", true);
			StartCoroutine(ExecuteSkill(i));
		}
		
	}
	public IEnumerator ExecuteSkill(int i)
	{
		//print("using skill " + i);
		Skill s = skills[i];
		foreach(Action a in s.actions)
		{
			yield return ExecuteAction(a);
		}
		anim.speed = 1;
		yield return new WaitForEndOfFrame();
		anim.SetBool("Attacking", false);
		busy = false;
	}
	public IEnumerator ExecuteAction(Action a)
	{
		//print("using action ");
		if (a.useWeapons)
		{
			currentAttackTransform = a.spawnTransform;
			attackAllowed = true;
		}
		anim.SetInteger("AttackIndex", a.animationIndex);
		anim.SetTrigger("Attack");
		anim.speed = 1 / a.time;//slow down the animation to take up a.time, this supposes that the animation was originally 1 sec
		
		//if there is something to spawn, spawn it
		if(a.spawn != null)
		{
			GameObject g = Instantiate(a.spawn);
			if (a.spawnAsChild) g.transform.SetParent(attackTranforms[a.spawnTransform]);
			g.transform.position = attackTranforms[a.spawnTransform].position;
			g.GetComponent<SkillObject>().parent = this;
		}
		
		yield return new WaitForSeconds(a.time);
		attackAllowed = false;
		//print("finished action ");
	}


	#endregion

	public string GetData()
	{
		SaveDataAbilities s = new SaveDataAbilities(stat, maxStat);
		return JsonConvert.SerializeObject(s, Formatting.Indented);
	}

	public void SetData(string data)
	{
		SaveDataAbilities s = JsonConvert.DeserializeObject<SaveDataAbilities>(data);
		//TODO: warning, sceneindex not considered here
		this.stat = s.stat;
		this.maxStat = s.maxStat;
	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "Stats";
	}
}

[System.Serializable]
public class Skill
{
	public Stat cost;
	public float useDist;//use this for AI to determine what skill to use or if it needs to approach
	public float useAngle;
	public Action[] actions;
}

[System.Serializable]
public class Action
{
	public int animationIndex;
	public float time;
	public GameObject spawn;//what to spawn
	public int spawnTransform;//attackTransforms[spawnTranfrorm]
	public bool spawnAsChild;//should it be child of attackTransforms[spawnTranfrorm]?
	public bool useWeapons;//should the weapons cause damage during this action?
}