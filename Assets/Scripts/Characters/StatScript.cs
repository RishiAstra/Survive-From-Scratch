using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using bobStuff;
using System.Text;
using System;

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
		return toLvl * ((Mathf.Pow(level + 15, 1.5f) - 64) / 16 + 1);
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

	public void Divide(Stat other)
	{
		hp  /= other.hp;
		mp  /= other.mp;
		eng /= other.eng;
		mor /= other.mor;
		atk /= other.atk;
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

/// <summary>
/// a modifier for hp, dmg, armor, etc.
/// </summary>
[System.Serializable]
public class Modifier
{
	public float preadd;
	public float premult;//NOTE: mults are relative to 100%, so a mult of 0.1 means 110%, a mult of 0 means 100%
	public float postadd;
	public float postmult;
}

/// <summary>
/// a modifier for hp, dmg, armor, etc. that only effects for a type
/// </summary>
[System.Serializable]
public class TypedModifier: Modifier
{
	public AttackType type;

	public string ToString(string modifyName, Color col)
	{
		StringBuilder sb = new StringBuilder();
		string p = "<#" + ColorUtility.ToHtmlStringRGB(col) + ">+";
		string m = modifyName + "</color> ";
		string t = type.ToString();

		if (preadd > 0) sb.Append(p + preadd.ToString("F1") + " " + m + " pre " + t + "\n");
		if (premult > 0) sb.Append(p + (premult * 100f).ToString("F1") + "% " + m + " pre " + t + "\n");
		if (postadd > 0) sb.Append(p + postadd.ToString("F1") + " " + m + " post " + t + "\n");
		if (postmult > 0) sb.Append(p + (postmult * 100f).ToString("F1") + "% " + m + " post " + t + "\n");

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

	public bool dead;

	private Stat initialMaxStat;
	private int plvl;
	private List<Item> pItemsEquipped;
	public List<DamageRecord> damageRecords;
	public SaveEntity mySave;

	private void Awake()
	{
		mySave = GetComponent<SaveEntity>();
		initialMaxStat = maxStat;
		pItemsEquipped = new List<Item>();
		UpdateXP();
		CheckStats();
		if (resetOnStart) ResetStats();//reset before anythign else can happen

	}

	#region stats and xp updates

	public void GiveXp(float amount)
	{
		xp += amount;
		UpdateXP();
	}

	private void UpdateXP()
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
			CheckStats();
		}
	}

	public static float GetRequiredXPForLvl(int level)
	{
		return 100f * ((Mathf.Pow(level + 14, 1.5f) - 64) / 4 + 1);
	}

	void CheckStats()
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
			UpdateStats();
			plvl = lvl;
			pItemsEquipped = new List<Item>();
			pItemsEquipped.AddRange(itemsEquipped);
		}
	}

	void UpdateStats()
	{
		//armor and atk depend on type, all others should have modifiers applier below
		if (lvl <= 0) lvl = 1;
		//calculate the new max stat
		Stat newMaxStat = initialMaxStat.GetLeveled(lvl);//get base max stat from lvl
														 //apply modifiers
		List<Modifier> modifiers;

		modifiers = new List<Modifier>();
		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.hpMods != null) modifiers.AddRange(GameControl.itemTypes[i.id].mods.hpMods);
		}
		newMaxStat.hp = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, newMaxStat.hp);

		modifiers = new List<Modifier>();
		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.mpMods != null) modifiers.AddRange(GameControl.itemTypes[i.id].mods.mpMods);
		}
		newMaxStat.mp = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, newMaxStat.mp);

		modifiers = new List<Modifier>();
		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.engMods != null) modifiers.AddRange(GameControl.itemTypes[i.id].mods.engMods);
		}
		newMaxStat.eng = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, newMaxStat.eng);

		modifiers = new List<Modifier>();
		foreach (Item i in itemsEquipped)
		{
			if (GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.morMods != null) modifiers.AddRange(GameControl.itemTypes[i.id].mods.morMods);
		}
		newMaxStat.mor = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, newMaxStat.mor);

		//print(maxStat + "|" + newMaxStat);
		//now that the new max stat has been calculated, scale the current stat so that if you had 80% hp before the max stat chance, you will have 80% afterward
		Stat temp = newMaxStat;
		temp.Divide(maxStat);//get the proportions/percent changes (0-1) of max stat

		//avoid NaN
		if (float.IsNaN(temp.hp)) temp.hp = 1;
		if (float.IsNaN(temp.mp)) temp.mp = 1;
		if (float.IsNaN(temp.eng)) temp.eng = 1;
		if (float.IsNaN(temp.mor)) temp.mor = 1;
		if (float.IsNaN(temp.atk)) temp.atk = 1;

		stat.Multiply(temp);

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

		for(int i = 0; i < damageRecords.Count; i++)
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
			float amount = d.amount / t * xpBounty;//proportional to dmg dealt by this damage record
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

		DamageTextControl.PutDamageText(cols.bounds.center, damageTaken);
	}
	
	public float GetReceiveDamageAmount(float damage, AttackType type, Armor armor)
	{

		List<TypedModifier> modifiers = new List<TypedModifier>();
		if(armor.localArmorModifiers != null) modifiers.AddRange(armor.localArmorModifiers);

		foreach (Armor a in armors)
		{
			if(a.mods.globalArmorModifiers != null) modifiers.AddRange(a.mods.globalArmorModifiers);
		}

		//get effective armor value
		float effectiveArmorValue = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, type);

		float mult = 1 - Mathf.Pow(RESIST_EXPONENT_BASE, -damage / effectiveArmorValue);
		return damage * mult;
	}

	public float GetOutputDamageAmount(AttackType type, float dmgMult = 1)
	{

		List<TypedModifier> modifiers = new List<TypedModifier>();

		foreach (Item i in itemsEquipped)
		{
			if(GameControl.itemTypes[i.id].mods != null && GameControl.itemTypes[i.id].mods.atkMods != null) modifiers.AddRange(GameControl.itemTypes[i.id].mods.atkMods);
		}

		//get effective dmg value
		float effectiveDmgValue = ModifierGroup.GetComputedValueFromTypedModifiers(modifiers, type, stat.atk);
		print(effectiveDmgValue + "|" + modifiers.Count + "|" + stat.atk);
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

	public string GetData()
	{
		SaveDataStat s = new SaveDataStat(stat, initialMaxStat, lvl, damageRecords);
		return JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings);
	}

	public void SetData(string data)
	{
		SaveDataStat s = JsonConvert.DeserializeObject<SaveDataStat>(data);
		//TODO: warning, sceneindex not considered here
		this.initialMaxStat = s.initialMaxStat;
		this.lvl = s.lvl;
		UpdateStats();//this will change stat, so do if before the right value of stat is asigned
		this.stat = s.stat;
		this.damageRecords = s.dmgs;
		if (damageRecords == null) damageRecords = new List<DamageRecord>();

	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "Stats";
	}

	#endregion
}