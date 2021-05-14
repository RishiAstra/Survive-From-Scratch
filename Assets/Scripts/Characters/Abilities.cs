using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class Abilities : MonoBehaviour, ISaveable
{
	public const float RESIST_EXPONENT_BASE = 2f;



	public bool dead;//TODO: consider stopping all attacks already happening when it dies
	public bool resetOnStart = true;
	public StatScript myStat;
	public List<Skill> skills;
	public bool busy;
	public bool attackAllowed;
	public Animator anim;
	public float dieTime = 2.5f;

	public int currentAttackTransform;
	public Transform[] attackTranforms;

	public string[] enemyString;

	private void Awake()
	{
		
	}


	// Update is called once per frame
	void Update()
	{
		if(myStat.stat.hp <= 0 && !dead)
		{
			Die();
		}
	}

	private void Die()
	{
		dead = true;
		Destroy(gameObject, dieTime);
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


	/// <summary>
	/// Uses a skill. It's ok to call this every frame since it won't execute the skill if busy.
	/// </summary>
	/// <param name="i"></param>
	public void UseSkill(int i)
	{
		if (dead || !RegionSettings.main.allowCombat) return;
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

	/*************save skills and stuff below*************/

	public string GetData()
	{
		//SaveDataAbilities s = new SaveDataAbilities(stat, maxStat);
		//return JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings);
		return null;
	}

	public void SetData(string data)
	{
		//SaveDataAbilities s = JsonConvert.DeserializeObject<SaveDataAbilities>(data);
		////TODO: warning, sceneindex not considered here
		//this.stat = s.stat;
		//this.maxStat = s.maxStat;
	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return null;// "Stats";
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