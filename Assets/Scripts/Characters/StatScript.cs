/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using bobStuff;
using System.Text;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

[System.Serializable]
public struct Stat
{
	public float hp;//health points, if reaches 0 die
	public float mp;//mana points, can be used to cast spells or skills
	public float eng;//energy, used for pretty much everything especially big attacks or running
	public float mor;//morale, effects stats
	public float atk;//attack power
	//TODO: defence should be implemented in colliders that can have different defence values and resistances

	public Stat GetLeveled(int level)
	{
		return new Stat
		{
			hp = GetLeveledFloat(hp, level),
			mp = GetLeveledFloat(mp, level),
			eng = GetLeveledFloat(eng, level),
			mor = GetLeveledFloat(mor, level),
			atk = GetLeveledFloat(atk, level),
		};
	}

	public static float GetLeveledFloat(float toLvl, float level)
	{
		return toLvl * (1 + 0.1f * (level - 1));// ((Mathf.Pow(level + 15, 1.5f) - 64) / 16 + 1);
	}

	public static bool StatEquals(Stat c1, Stat c2)
	{
		return
			c1.hp == c2.hp &&
			c1.mp == c2.mp &&
			c1.eng == c2.eng &&
			c1.mor == c2.mor &&
			c1.atk == c2.atk;
	}

	public void Multiply(Stat other)
	{
		hp  *= other.hp;
		mp  *= other.mp;
		eng *= other.eng;
		mor *= other.mor;
		atk *= other.atk;
	}

	public void Multiply(float mult)
	{
		hp  *= mult;
		mp  *= mult;
		eng *= mult;
		mor *= mult;
		atk *= mult;
	}

	public void Divide(Stat other)
	{
		hp  /= other.hp;
		mp  /= other.mp;
		eng /= other.eng;
		mor /= other.mor;
		atk /= other.atk;
	}

	public void Add(Stat other)
	{
		hp  += other.hp;
		mp  += other.mp;
		eng += other.eng;
		mor += other.mor;
		atk += other.atk;
	}

	public void Subtract(Stat other)
	{
		hp  -= other.hp;
		mp  -= other.mp;
		eng -= other.eng;
		mor -= other.mor;
		atk -= other.atk;
	}

	public bool GreaterThanOrEqualTo(Stat other)
	{
		return
			hp >= other.hp &&
			mp >= other.mp &&
			eng >= other.eng &&
			mor >= other.mor &&
			atk >= other.atk;
	}

	public override string ToString()
	{
		return
			"hp: " + hp + "\n" +
			"mp: " + mp + "\n" +
			"eng: " + eng + "\n" +
			"mor: " + mor + "\n" +
			"atk: " + atk + "\n";
	}
}
//TODO: maybe just use ModifierGroup instead of this
[System.Serializable]
public class Armor
{
	public Collider[] col;
	public ModifierGroup mods;

	//used when this armor is hit and dmg is taken
	public List<TypedModifier> localArmorModifiers;

	public bool HasCollider(Collider c)
	{
		return col.Contains(c);
	}
}

public enum LvlGrowthType
{
	None,
	Linear,
}


/// <summary>
/// a modifier for hp, dmg, armor, etc.
/// </summary>
[System.Serializable]
public class Modifier
{
	public float preadd (int lvl){ return GetLvledValue(m_preadd, lvl); }
	public float premult (int lvl) { return GetLvledValue(m_premult, lvl); }//NOTE: mults are relative to 100%, so a mult of 0.1 means 110%, a mult of 0 means 100%
	public float postadd (int lvl) { return GetLvledValue(m_postadd, lvl); }
	public float postmult (int lvl) { return GetLvledValue(m_postmult, lvl); }

	public float m_preadd;
	public float m_premult;//NOTE: mults are relative to 100%, so a mult of 0.1 means 110%, a mult of 0 means 100%
	public float m_postadd;
	public float m_postmult;
	public LvlGrowthType growthType;
	public float a;//for linear, this is the slope

	private float GetLvledValue(float f, int lvl)
	{
		if (f < 0.00001f) return f;//don't grow mods that are at 0, they are meant to stay at 0
		switch (growthType)
		{
			case LvlGrowthType.None:
				return f;
			case LvlGrowthType.Linear:
				return f + (lvl - 1) * a;
			default:
				return f;
		}
	}
}

/// <summary>
/// a modifier for hp, dmg, armor, etc. that only effects for a type
/// </summary>
[System.Serializable]
public class TypedModifier: Modifier
{
	public AttackType type;

	public string GetUpgradeString(string modifyName, int lvl, int nextlvl, Color col)
	{
		StringBuilder sb = new StringBuilder();
		string p = "<#" + ColorUtility.ToHtmlStringRGB(col) + ">+";
		string m = modifyName + "</color> ";
		string t = type.ToString();

		float pra = preadd(lvl);
		float pra2 = preadd(nextlvl);
		if (pra > 0) sb.Append(p + pra.ToString("F1") + "->" + pra2.ToString("F1") + " " + m + " pre " + t + "\n");
		float prm = premult(lvl);
		float prm2 = premult(nextlvl);
		if (prm > 0) sb.Append(p + (prm * 100f).ToString("F1") + "%->" + (prm2 * 100f).ToString("F1") + "% " + m + " pre " + t + "\n");
		float poa = postadd(lvl);
		float poa2 = postadd(nextlvl);
		if (poa > 0) sb.Append(p + poa.ToString("F1") + "->" + poa2.ToString("F1") + " " + m + " post " + t + "\n");
		float pom = postmult(lvl);
		float pom2 = postmult(nextlvl);
		if (pom > 0) sb.Append(p + (pom * 100f).ToString("F1") + "%->" + (pom2 * 100f).ToString("F1") + "% " + m + " post " + t + "\n");
		//(pom * 100f).ToString("F1")

		return sb.ToString();
	}
	public string ToString(string modifyName, int lvl, Color col)
	{
		StringBuilder sb = new StringBuilder();
		string p = "<#" + ColorUtility.ToHtmlStringRGB(col) + ">+";
		string m = modifyName + "</color> ";
		string t = type.ToString();

		float pra = preadd(lvl);
		if (pra > 0) sb.Append(p + pra.ToString("F1") + " " + m + " pre " + t + "\n");
		float prm = premult(lvl);
		if (prm > 0) sb.Append(p + (prm * 100f).ToString("F1") + "% " + m + " pre " + t + "\n");
		float poa = postadd(lvl);
		if (poa > 0) sb.Append(p + poa.ToString("F1") + " " + m + " post " + t + "\n");
		float pom = postmult(lvl);
		if (pom > 0) sb.Append(p + (pom * 100f).ToString("F1") + "% " + m + " post " + t + "\n");

		return sb.ToString(); 
	}
}

[System.Flags]
public enum AttackType
{
	//physical types (0-6)
	pierce = 1 << 0,
	blunt = 1 << 1,
	slash = 1 << 2,
	//magic types (6-9)
	impact = 1 << 6,
	surround = 1 << 7,
	//elements (10-20)
	fire = 1 << 10,
	water = 1 << 11,
	air = 1 << 12,
	earth = 1 << 13,

	//combos
	physical = pierce | blunt | slash,
	magic = impact | surround,
	elemental = fire | water | air | earth,
};

[System.Serializable]
public struct DamageRecord
{
	public float amount;
	public float timeLeft;
	public long damagedBy;
}

public class StatScript : MonoBehaviour, ISaveable
{
	public const float RESIST_EXPONENT_BASE = 2f;
	public const float DAMAGERECORD_TIME = 60f;//remember damages received for 60s
	public const float STAT_REGAIN_PROPORTION = 0.01f;//the amount/proportion of stat to regain every second

	public static Dictionary<long, float> unclaimedXPBounties = new Dictionary<long, float>();//xp from kills that wasn't claimed yet <id, amount of unclaimed xp>

	public bool resetOnStart = true;
	public int lvl;
	public Stat maxStat;
	public Stat stat;
	public float dieTime = 2.5f;
	public float xp;
	public float xpBounty;

	public List<Armor> armors;
	public List<Item> itemsEquipped;
	public List<StatRestore> statRestores;

	public bool dead;

	private Stat initialMaxStat;
	private int plvl;
	private List<Item> pItemsEquipped;
	public List<DamageRecord> damageRecords;
	public SaveEntity mySave;
	public List<StatSkill> statSkills;
	public List<int> skillLvls;

	private float timeSinceRegain;

	private void Awake()
	{
		//default to lvl 1 for skills u start with
		for(int i = 0; i < statSkills.Count; i++)
		{
			if(skillLvls.Count <= i)
			{
				skillLvls.Add(1);
			}
		}

		mySave = GetComponent<SaveEntity>();
		initialMaxStat = maxStat;
		pItemsEquipped = new List<Item>();
		


	}

	private void Start()
	{
		UpdateXP();
		CheckStats();
		if (resetOnStart) ResetStats();//reset before anythign else can happen
	}

	public void AddStatRestore(StatRestore s)
	{
		s.timeSpent = 0;//reset the time left
		statRestores.Add(s);
	}

	public int GetSkillPointTotal()
	{
		return (lvl - 1) * 2;//2 skill points for each lvl, but you start at lvl 1 with 0 skill points
	}

	#region stats and xp updates

	public void GiveXp(float amount)
	{
		xp += amount;
		UpdateXP();
	}

	private void UpdateXP(bool scaleStats = true)
	{
		int l = lvl;
		bool changed = false;

		//perform a binary search-like thing to make sure l isn't waaaay off
		while (GetRequiredXPForLvl(l + 1) <= xp / 10f || GetRequiredXPForLvl(l) > xp * 10f)
		{
			if (GetRequiredXPForLvl(l + 1) <= xp / 5f)
			{
				l *= 2;
			}
			else
			{
				l /= 2;
				Debug.LogError("Too high lvl for xp, reducing xp");
			}
		}

		//while you have more xp than required for next lvl, increase lvl
		while (GetRequiredXPForLvl(l + 1) <= xp)
		{
			l++;
			changed = true;
		}

		//while you have less xp than required for current lvl, reduce lvl
		while(l > 1 && GetRequiredXPForLvl(l) > xp)
		{
			Debug.LogError("Too high lvl for xp, reducing xp");
			l--;
			changed = true;
		}

		if (changed)
		{
			lvl = l;
			CheckStats(scaleStats);
		}
	}

	public static float GetRequiredXPForLvl(int level)
	{
		float q = 2.8f;
		float c1 = 1f;
		float c2 = 0.4f;
		float multiplier = 100f;
		float f(float x)
		{
			return Mathf.Pow(c2 * (x - c1), q) + x - 1;
		}

		float f3(float x)
		{
			return f(x) - f(2) + 1;
		}

		float f1(float x)
		{
			return Mathf.Max(multiplier * f3(x), 0);
		}

		return f1(level);
	}

	void CheckStats(bool scaleStats = true)
	{
		//if lvl changed or items equipped changed
		bool shouldRecalculate = plvl != lvl | pItemsEquipped.Count != itemsEquipped.Count;

		if (!shouldRecalculate)
		{
			for(int i = 0; i < pItemsEquipped.Count; i++)
			{
				if(!Item.ItemEquals(pItemsEquipped[i], itemsEquipped[i]))
				{
					shouldRecalculate = true;
					break;
				}
			}
		}

		if(shouldRecalculate)
		{
			UpdateStats(scaleStats);
			plvl = lvl;
			pItemsEquipped = new List<Item>();
			pItemsEquipped.AddRange(itemsEquipped);
		}
	}

	//stats that don't depend on type are calculated here; if they depend on type, they are in the damage function
	void UpdateStats(bool scaleStats = true)
	{
		//armor and atk depend on type, all others should have modifiers applier below
		if (lvl <= 0) lvl = 1;
		//calculate the new max stat
		Stat newMaxStat = initialMaxStat.GetLeveled(lvl);//get base max stat from lvl
														 //apply modifiers from items equipped and skills
		List<Modifier> modifiers;
		List<int> lvls;

		///*************hp**********/
		//modifiers = new List<Modifier>();
		//foreach (Item i in itemsEquipped)
		//{
		//	if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.hpMods != null) modifiers.AddRange(GameControl.itemTypes[i.id].mods.hpMods);
		//}

		//foreach (StatSkill s in statSkills)
		//{
		//	if (s.mods != null && s.mods.hpMods != null) modifiers.AddRange(s.mods.hpMods);
		//}

		//newMaxStat.hp = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, newMaxStat.hp);
		///***********mp************/

		//modifiers = new List<Modifier>();
		//foreach (Item i in itemsEquipped)
		//{
		//	if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.mpMods != null) modifiers.AddRange(GameControl.itemTypes[i.id].mods.mpMods);
		//}

		//foreach (StatSkill s in statSkills)
		//{
		//	if (s.mods != null && s.mods.mpMods != null) modifiers.AddRange(s.mods.mpMods);
		//}

		//newMaxStat.mp = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, newMaxStat.mp);
		/************hp***********/

		modifiers = new List<Modifier>();
		lvls = new List<int>();
		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.hpMods != null)
			{
				modifiers.AddRange(GameControl.itemTypes[i.id].mods.hpMods);
				for (int j = 0; j < GameControl.itemTypes[i.id].mods.hpMods.Count; j++)
				{
					lvls.Add(1);
				}
			}
		}

		for (int i = 0; i < statSkills.Count; i++)
		{
			StatSkill s = statSkills[i];
			if (s.mods != null && s.mods.hpMods != null)
			{
				modifiers.AddRange(s.mods.hpMods);
				for (int j = 0; j < s.mods.hpMods.Count; j++)
				{
					lvls.Add(skillLvls[i]);
				}
			}
		}

		newMaxStat.hp = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, lvls, newMaxStat.hp);
		/************mp***********/

		modifiers = new List<Modifier>();
		lvls = new List<int>();
		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.mpMods != null)
			{
				modifiers.AddRange(GameControl.itemTypes[i.id].mods.mpMods);
				for (int j = 0; j < GameControl.itemTypes[i.id].mods.mpMods.Count; j++)
				{
					lvls.Add(1);
				}
			}
		}

		for (int i = 0; i < statSkills.Count; i++)
		{
			StatSkill s = statSkills[i];
			if (s.mods != null && s.mods.mpMods != null)
			{
				modifiers.AddRange(s.mods.mpMods);
				for (int j = 0; j < s.mods.mpMods.Count; j++)
				{
					lvls.Add(skillLvls[i]);
				}
			}
		}

		newMaxStat.mp = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, lvls, newMaxStat.mp);
		/************eng***********/

		modifiers = new List<Modifier>();
		lvls = new List<int>();
		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.engMods != null)
			{
				modifiers.AddRange(GameControl.itemTypes[i.id].mods.engMods);
				for (int j = 0; j < GameControl.itemTypes[i.id].mods.engMods.Count; j++)
				{
					lvls.Add(1);
				}
			}
		}

		for (int i = 0; i < statSkills.Count; i++)
		{
			StatSkill s = statSkills[i];
			if (s.mods != null && s.mods.engMods != null)
			{
				modifiers.AddRange(s.mods.engMods);
				for(int j = 0; j < s.mods.engMods.Count; j++)
				{
					lvls.Add(skillLvls[i]);
				}
			}
		}

		newMaxStat.eng = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, lvls, newMaxStat.eng);
		/************mor***********/

		modifiers = new List<Modifier>();
		lvls = new List<int>();
		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.morMods != null)
			{
				modifiers.AddRange(GameControl.itemTypes[i.id].mods.morMods);
				for (int j = 0; j < GameControl.itemTypes[i.id].mods.morMods.Count; j++)
				{
					lvls.Add(1);
				}
			}
		}

		for (int i = 0; i < statSkills.Count; i++)
		{
			StatSkill s = statSkills[i];
			if (s.mods != null && s.mods.morMods != null)
			{
				modifiers.AddRange(s.mods.morMods);
				for (int j = 0; j < s.mods.morMods.Count; j++)
				{
					lvls.Add(skillLvls[i]);
				}
			}
		}

		newMaxStat.mor = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, lvls, newMaxStat.mor);
		///***********mor************/

		//modifiers = new List<Modifier>();
		//foreach (Item i in itemsEquipped)
		//{
		//	if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.morMods != null) modifiers.AddRange(GameControl.itemTypes[i.id].mods.morMods);
		//}

		//foreach (StatSkill s in statSkills)
		//{
		//	if (s.mods != null && s.mods.morMods != null) modifiers.AddRange(s.mods.morMods);
		//}

		//newMaxStat.mor = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, newMaxStat.mor);

		/***********END************/

		//print(maxStat + "|" + newMaxStat);
		//now that the new max stat has been calculated, scale the current stat so that if you had 80% hp before the max stat chance, you will have 80% afterward
		if (scaleStats)
		{
			Stat temp = newMaxStat;
			temp.Divide(maxStat);//get the proportions/percent changes (0-1) of max stat

			//avoid NaN
			if (float.IsNaN(temp.hp)) temp.hp = 1;
			if (float.IsNaN(temp.mp)) temp.mp = 1;
			if (float.IsNaN(temp.eng)) temp.eng = 1;
			if (float.IsNaN(temp.mor)) temp.mor = 1;
			if (float.IsNaN(temp.atk)) temp.atk = 1;

			stat.Multiply(temp);
		}
		

		//now that all that is done, assign the new max stat
		maxStat = newMaxStat;

		//make sure that stat is within range - it shouldn't be too large but just to be safe
		ClampStat();

	}

	private void ClampStat()
	{
		stat.hp = Mathf.Clamp(stat.hp, 0, maxStat.hp);
		stat.mp = Mathf.Clamp(stat.mp, 0, maxStat.mp);
		stat.eng = Mathf.Clamp(stat.eng, 0, maxStat.eng);
		stat.mor = Mathf.Clamp(stat.mor, 0, maxStat.mor);
		stat.atk = Mathf.Clamp(stat.atk, 0, maxStat.atk);
	}

	#endregion

	// Start is called before the first frame update
	void Update()
	{
		CheckStats();
		if (stat.hp <= 0 && !dead)
		{
			Die();
		}

		for (int i = 0; i < damageRecords.Count; i++)
		{
			DamageRecord temp = damageRecords[i];
			temp.timeLeft -= Time.deltaTime;
			damageRecords[i] = temp;
			if (damageRecords[i].timeLeft <= 0)
			{
				damageRecords.RemoveAt(i);
				i--;
			}
		}


		if (!dead)
		{
			timeSinceRegain += Time.deltaTime;
			if(timeSinceRegain > 1)
			{
				timeSinceRegain -= 1;
				Stat temp = maxStat;
				temp.Multiply(STAT_REGAIN_PROPORTION);
				stat.Add(temp);
				ClampStat();
			}

			for (int i = 0; i < statRestores.Count; i++)
			{
				StatRestore temp = statRestores[i];
				//total intervals before adding this frame's time
				int intervalsBefore = Mathf.FloorToInt(temp.timeSpent / temp.timeInterval);
				temp.timeSpent += Time.deltaTime;
				
				int intervalsAfter = Mathf.FloorToInt(temp.timeSpent / temp.timeInterval);
				//don't allow restoring more times than the intervalCount
				if (intervalsAfter > temp.intervalCount) intervalsAfter = temp.intervalCount;

				if(intervalsAfter > intervalsBefore)
				{
					Stat toAdd = temp.stat;
					//multiply toAdd by the new intervals passed divided by the interval count
					toAdd.Multiply((intervalsAfter - intervalsBefore) / (float)temp.intervalCount);
					stat.Add(toAdd);

					//clamp upper
					if (stat.hp >  maxStat.hp)  stat.hp =  maxStat.hp;
					if (stat.mp >  maxStat.mp)  stat.mp =  maxStat.mp;
					if (stat.eng > maxStat.eng) stat.eng = maxStat.eng;
					if (stat.mor > maxStat.mor) stat.mor = maxStat.mor;
					if (stat.atk > maxStat.atk) stat.atk = maxStat.atk;
					//TODO: consider clamping lower

				}


				//apply the changes
				statRestores[i] = temp;

				//if this restore is done, remove it
				if (intervalsAfter == temp.intervalCount)
				{
					statRestores.RemoveAt(i);
					i--;
				}

			}
		}
		
		

		//claim any unclaimed xp for this
		if (unclaimedXPBounties.ContainsKey(mySave.id))
		{
			GiveXp(unclaimedXPBounties[mySave.id]);
			unclaimedXPBounties.Remove(mySave.id);
		}
	}

	private void Die()
	{
		dead = true;


		//give xp to everyone who helped kill
		float t = 0;
		
		foreach(DamageRecord d in damageRecords)
		{
			t += d.amount;
		}

		foreach(DamageRecord d in damageRecords)
		{
			long id = d.damagedBy;
			float amount = d.amount / t * Stat.GetLeveledFloat(xpBounty, lvl);//proportional to dmg dealt by this damage record
			if (unclaimedXPBounties.ContainsKey(id))
			{
				unclaimedXPBounties[id] += amount;
			}
			else
			{
				unclaimedXPBounties.Add(id, amount);

			}
		}

		//now die
		Destroy(gameObject, dieTime);
	}

	public void ResetStats()
	{
		stat = maxStat;
	}

	#region damage

	public void Damage(float dmg, Abilities from, Collider cols, AttackType type)//accouns for weakpoints in different armor pieces
	{
		Damage(dmg, from, cols, type, cols.bounds.center);
	}

	public void Damage(float dmg, Abilities from, Collider cols, AttackType type, Vector3 damagePosition)//accouns for weakpoints in different armor pieces
	{
		if (!RegionSettings.main.allowCombat) return;

		Armor a = armors.Count > 0 ? armors[0] : new Armor();//default to first
		foreach (Armor t in armors)
		{
			if (t.HasCollider(cols))
			{
				a = t;
			}
		}

		//dmg = from.myStat

		float damageTaken = GetReceiveDamageAmount(dmg, type, a);
		float previousHp = stat.hp;
		stat.hp -= damageTaken;
		damageRecords.Add(new DamageRecord { amount = damageTaken, damagedBy = from.myStat.mySave.id});


		//update the progress tracker
		SaveEntity save = GetComponent<SaveEntity>();
		if (save != null)
		{
			ProgressTracker.main.RegisterDamage(damageTaken, from, save.type);

			//if not already dead, register the kill
			if (stat.hp <= 0 && !dead)
			{
				ProgressTracker.main.RegisterKill(save.type, this, from);
				Die();
			}
		}

		if(GameControl.main.myAbilities.myStat == this)
		{

			DamageTextControl.PutDamageText(damageTaken, true);

		}
		else
		{
			DamageTextControl.PutDamageText(damagePosition, damageTaken);
		}

	}
	
	public float GetReceiveDamageAmount(float damage, AttackType type, Armor armor)
	{

		List<TypedModifier> modifiers = new List<TypedModifier>();
		List<int> lvls = new List<int>();
		if (armor.localArmorModifiers != null)
		{
			modifiers.AddRange(armor.localArmorModifiers);
			for (int j = 0; j < armor.localArmorModifiers.Count; j++)
			{
				lvls.Add(1);
			}
		}

		foreach (Armor a in armors)
		{
			if (a.mods != null && a.mods.globalArmorModifiers != null)
			{
				modifiers.AddRange(a.mods.globalArmorModifiers);
				for (int j = 0; j < a.mods.globalArmorModifiers.Count; j++)
				{
					lvls.Add(1);
				}
			}
		}

		for (int i = 0; i < statSkills.Count; i++)
		{
			StatSkill s = statSkills[i];
			if (s.mods != null && s.mods.globalArmorModifiers != null)
			{
				modifiers.AddRange(s.mods.globalArmorModifiers);
				for (int j = 0; j < s.mods.globalArmorModifiers.Count; j++)
				{
					lvls.Add(skillLvls[i]);
				}
			}
		}

		//get effective armor value
		float effectiveArmorValue = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, lvls, type);

		float mult = 1 - Mathf.Pow(RESIST_EXPONENT_BASE, -damage / effectiveArmorValue);
		return damage * mult;
	}

	public float GetOutputDamageAmount(AttackType type, float dmgMult = 1)
	{

		List<TypedModifier> modifiers = new List<TypedModifier>();
		List<int> lvls = new List<int>();

		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.atkMods != null)
			{
				modifiers.AddRange(GameControl.itemTypes[i.id].mods.atkMods);
				for (int j = 0; j < GameControl.itemTypes[i.id].mods.atkMods.Count; j++)
				{
					lvls.Add(1);
				}
			}
		}

		for (int i = 0; i < statSkills.Count; i++)
		{
			StatSkill s = statSkills[i];
			if (s.mods != null && s.mods.atkMods != null)
			{
				modifiers.AddRange(s.mods.atkMods);
				for (int j = 0; j < s.mods.atkMods.Count; j++)
				{
					lvls.Add(skillLvls[i]);
				}
			}
		}

		//get effective dmg value
		float effectiveDmgValue = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, lvls, type, stat.atk);
		//print(effectiveDmgValue + "|" + modifiers.Count + "|" + stat.atk);
		return effectiveDmgValue * dmgMult;
	}

	#endregion

	//#region compute modifiers

	//public float GetComputedValueFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial = 0)
	//{
	//	//base amount
	//	float x = GetPreAddFromTypedModifiers(modifiers, type, initial);
	//	//print("1:" + x);
	//	//premultiply
	//	x *= (1 + GetPreMultFromTypedModifiers(modifiers, type, initial));
	//	//print("2:" + x);
	//	//postadd
	//	x += GetPostAddFromTypedModifiers(modifiers, type, initial);
	//	//print("3:" + x);
	//	//postmultiply
	//	x *= (1 + GetPostMultFromTypedModifiers(modifiers, type, initial));

	//	return x;
	//}


	//public float GetPreAddFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial)
	//{
	//	float x = initial;
	//	for(int i = 0; i < modifiers.Count; i++)
	//	{
	//		if (AttackTypeOverlap(modifiers[i].type, type))
	//		{
	//			x += modifiers[i].preadd;
	//		}
	//	}
	//	return x;
	//}

	//public float GetPreMultFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial)
	//{
	//	float x = 0;
	//	for (int i = 0; i < modifiers.Count; i++)
	//	{
	//		if (AttackTypeOverlap(modifiers[i].type, type))
	//		{
	//			x += modifiers[i].premult;
	//		}
	//	}
	//	return x;
	//}

	//public float GetPostAddFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial)
	//{
	//	float x = 0;
	//	for (int i = 0; i < modifiers.Count; i++)
	//	{
	//		if (AttackTypeOverlap(modifiers[i].type, type))
	//		{
	//			x += modifiers[i].postadd;
	//		}
	//	}
	//	return x;
	//}

	//public float GetPostMultFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial)
	//{
	//	float x = 0;
	//	for (int i = 0; i < modifiers.Count; i++)
	//	{
	//		if (AttackTypeOverlap(modifiers[i].type, type))
	//		{
	//			x += modifiers[i].postmult;
	//		}
	//	}
	//	return x;
	//}

	//#endregion

	//public bool AttackTypeOverlap(AttackType a, AttackType b)
	//{
	//	//if bitwise and is not zero, some bits (enum flags) must be shared
	//	return (a & b) != 0;
	//}

	#region save

	public JObject GetData()
	{
		List<string> temp = new List<string>();
		foreach (StatSkill st in statSkills)
		{
			temp.Add(st.name);
		}
		SaveDataStat s = new SaveDataStat(stat, initialMaxStat, xp, damageRecords, temp, skillLvls, statRestores);
		return JObject.FromObject(s, Save.jsonSerializer);// JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings);
	}

	public void SetData(JObject data)
	{
		SaveDataStat s = data.ToObject<SaveDataStat>();// JsonConvert.DeserializeObject<SaveDataStat>(data);
		//TODO: warning, sceneindex not considered here
		this.initialMaxStat = s.initialMaxStat;
		this.xp = s.xp;
		UpdateStats();//this will change stat, so do if before the right value of stat is asigned
		this.stat = s.stat;
		this.damageRecords = s.dmgs;
		this.statSkills = new List<StatSkill>();
		foreach (string st in s.statSkills)
		{
			//AsyncOperationHandle<StatSkill> a = Addressables.LoadAssetAsync<StatSkill>("Assets/Skills/" + st + ".asset");
			//await Task.WhenAll(a.Task);

			statSkills.Add(Resources.Load<StatSkill>(st));
		}
		this.skillLvls = s.skillLvls;
		this.statRestores = s.statRestores;
		//null checks
		if (damageRecords == null) damageRecords = new List<DamageRecord>();
		if (statSkills == null)
		{
			statSkills = new List<StatSkill>();
			skillLvls = new List<int>();
		}
		if (statRestores == null) statRestores = new List<StatRestore>();

		UpdateXP(false);
		CheckStats();

	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "Stats";
	}

	#endregion
}