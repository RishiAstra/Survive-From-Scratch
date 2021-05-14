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

	public static bool StatEquals(Stat c1, Stat c2)
	{
		return
			c1.hp == c2.hp &&
			c1.mp == c2.mp &&
			c1.eng == c2.eng &&
			c1.mor == c2.mor &&
			c1.atk == c2.atk;
	}
}

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

//all modifiers that an equipment will give
[System.Serializable]
public class ModifierGroup
{
	//used when dmg is taken
	public List<TypedModifier> globalArmorModifiers;
	public List<TypedModifier> hpMods;
	public List<TypedModifier> mpMods;
	public List<TypedModifier> engMods;
	public List<TypedModifier> morMods;
	public List<TypedModifier> atkMods;
}

/// <summary>
/// a modifier for hp, dmg, armor, etc. that only effects for a type
/// </summary>
[System.Serializable]
public class TypedModifier: Modifier
{
	public AttackType type;
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

public class StatScript : MonoBehaviour, ISaveable
{
	public const float RESIST_EXPONENT_BASE = 2f;

	public bool resetOnStart = true;
	public Stat maxStat;
	public Stat stat;

	public List<Armor> armors;

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
		float damageTaken = GetDamageAmount(dmg, type, a);
		float previousHp = stat.hp;
		stat.hp -= damageTaken;


		//update the progress tracker
		SaveEntity save = GetComponent<SaveEntity>();
		if (save != null)
		{
			ProgressTracker.main.RegisterDamage(damageTaken, from, save.type);

			//if not already dead, register the kill
			if (stat.hp <= 0 && previousHp > 0)
			{
				ProgressTracker.main.RegisterKill(save.type, this, from);
			}
		}

		DamageTextControl.PutDamageText(cols.bounds.center, damageTaken);
	}
	public float GetDamageAmount(float damage, AttackType type, Armor armor)
	{

		List<TypedModifier> modifiers = new List<TypedModifier>();
		modifiers.AddRange(armor.localArmorModifiers);

		foreach (Armor a in armors)
		{
			modifiers.AddRange(a.mods.globalArmorModifiers);
		}


		float armorValue = Getpre

		//fix this
		//if (type == AttackType.Pierce) armorValue = armor.pierceResist;
		//if (type == AttackType.blunt) armorValue = armor.bluntResist;
		//if (type == AttackType.slash) armorValue = armor.slashResist;
		//if (type == AttackType.Magic) armorValue = armor.magicResist;
		float mult = 1 - Mathf.Pow(RESIST_EXPONENT_BASE, -damage / armorValue);
		return damage * mult;
	}

	public float GetComputedValueFromTypedModifiers(List<TypedModifier> modifiers, AttackType type)
	{
		//base amount
		float x = GetPreAddFromTypedModifiers(modifiers, type);
		//premultiply
		x *= (1 + GetPreMultFromTypedModifiers(modifiers, type));
		//postadd
		x += GetPostAddFromTypedModifiers(modifiers, type);
		//postmultiply
		x *= (1 + GetPostMultFromTypedModifiers(modifiers, type));
		
		return x;
	}


	public float GetPreAddFromTypedModifiers(List<TypedModifier> modifiers, AttackType type)
	{
		float x = 0;
		for(int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].preadd;
			}
		}
		return x;
	}

	public float GetPreMultFromTypedModifiers(List<TypedModifier> modifiers, AttackType type)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].premult;
			}
		}
		return x;
	}

	public float GetPostAddFromTypedModifiers(List<TypedModifier> modifiers, AttackType type)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].postadd;
			}
		}
		return x;
	}

	public float GetPostMultFromTypedModifiers(List<TypedModifier> modifiers, AttackType type)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].postmult;
			}
		}
		return x;
	}

	public bool AttackTypeOverlap(AttackType a, AttackType b)
	{
		//if bitwise and is not zero, some bits (enum flags) must be shared
		return (a & b) != 0;
	}


	public string GetData()
	{
		SaveDataStat s = new SaveDataStat(stat, maxStat);
		return JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings);
	}

	public void SetData(string data)
	{
		SaveDataStat s = JsonConvert.DeserializeObject<SaveDataStat>(data);
		//TODO: warning, sceneindex not considered here
		this.stat = s.stat;
		this.maxStat = s.maxStat;
	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "Stats";
	}
}