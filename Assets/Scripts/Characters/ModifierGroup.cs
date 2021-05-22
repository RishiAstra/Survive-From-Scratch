using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//all modifiers that an equipment will give
[System.Serializable]
public class ModifierGroup
{
	//used when dmg is taken, depends on type because can have +20% armor against fire
	public List<TypedModifier> globalArmorModifiers;
	//used to increase stats
	//these don't depend on the type, because it doesn't make sence to have +20% fire HP
	public List<Modifier> hpMods;
	public List<Modifier> mpMods;
	public List<Modifier> engMods;
	public List<Modifier> morMods;
	//this depends on the type, because you can have +20% fire attack
	public List<TypedModifier> atkMods;

	public ModifierGroup()
	{
		globalArmorModifiers = new List<TypedModifier>();
		hpMods =  new List<Modifier>();
		mpMods =  new List<Modifier>();
		engMods = new List<Modifier>();
		morMods = new List<Modifier>();
		atkMods = new List<TypedModifier>();
	}

	#region text

	public string ToString(int lvl)
	{
		StringBuilder sb = new StringBuilder();
		GetTypedModifierInfoText(sb, "DEF", globalArmorModifiers, lvl, GameControl.main.armorColor);
		GetTypedModifierInfoText(sb, "ATK", atkMods, lvl, GameControl.main.atkColor);
		List<int> lvls;

		lvls = new List<int>();
		for(int i = 0; i < hpMods.Count; i++)
		{
			lvls.Add(lvl);
		}
		GetModifierInfoText(sb, "HP", hpMods, lvls, GameControl.main.hpColor);
		lvls = new List<int>();
		for (int i = 0; i < mpMods.Count; i++)
		{
			lvls.Add(lvl);
		}
		GetModifierInfoText(sb, "MP", mpMods, lvls, GameControl.main.mpColor);
		lvls = new List<int>();
		for (int i = 0; i < engMods.Count; i++)
		{
			lvls.Add(lvl);
		}
		GetModifierInfoText(sb, "ENG", engMods, lvls, GameControl.main.engColor);
		lvls = new List<int>();
		for (int i = 0; i < morMods.Count; i++)
		{
			lvls.Add(lvl);
		}
		GetModifierInfoText(sb, "MOR", morMods, lvls, GameControl.main.morColor);
		return sb.ToString();
	}

	public string GetUpgradeString(int lvl, int nextlvl)
	{
		StringBuilder sb = new StringBuilder();
		GetTypedModifierUpgradeText(sb, "DEF", globalArmorModifiers, lvl, nextlvl, GameControl.main.armorColor);
		GetTypedModifierUpgradeText(sb, "ATK", atkMods, lvl, nextlvl, GameControl.main.atkColor);
		List<int> lvls;
		List<int> nextlvls;

		lvls = new List<int>();
		nextlvls = new List<int>();
		for (int i = 0; i < hpMods.Count; i++)
		{
			lvls.Add(lvl);
			nextlvls.Add(nextlvl);
		}
		GetModifierUpgradeText(sb, "HP", hpMods, lvls, nextlvls, GameControl.main.hpColor);
		////////////////////////////
		lvls = new List<int>();
		nextlvls = new List<int>();
		for (int i = 0; i < mpMods.Count; i++)
		{
			lvls.Add(lvl);
			nextlvls.Add(nextlvl);
		}
		GetModifierUpgradeText(sb, "MP", mpMods, lvls, nextlvls, GameControl.main.mpColor);
		////////////////////////////
		lvls = new List<int>();
		nextlvls = new List<int>();
		for (int i = 0; i < engMods.Count; i++)
		{
			lvls.Add(lvl);
			nextlvls.Add(nextlvl);
		}
		GetModifierUpgradeText(sb, "ENG", engMods, lvls, nextlvls, GameControl.main.engColor);
		////////////////////////////
		nextlvls = new List<int>();
		lvls = new List<int>();
		for (int i = 0; i < morMods.Count; i++)
		{
			lvls.Add(lvl);
			nextlvls.Add(nextlvl);
		}
		GetModifierUpgradeText(sb, "MOR", morMods, lvls, nextlvls, GameControl.main.morColor);
		///////////////////////////
		return sb.ToString();
	}

	private void GetModifierInfoText(StringBuilder sb, string modName, List<Modifier> mods, List<int> lvl, Color col)
	{
		float preadd = GetPreAddFromTypedModifiers(mods, lvl);
		float premult = GetPreMultFromTypedModifiers(mods, lvl);
		float postadd = GetPostAddFromTypedModifiers(mods, lvl);
		float postmult = GetPostMultFromTypedModifiers(mods, lvl);
		if (preadd > 0) sb.Append(GetAddString(preadd, modName, false, col) + "\n");
		if (premult > 0) sb.Append(GetMultString(premult, modName, false, col) + "\n");
		if (postadd > 0) sb.Append(GetAddString(postadd, modName, true, col) + "\n");
		if (postmult > 0) sb.Append(GetMultString(postmult, modName, true, col) + "\n");
	}

	private void GetModifierUpgradeText(StringBuilder sb, string modName, List<Modifier> mods, List<int> lvl, List<int> nextlvl, Color col)
	{
		float preadd = GetPreAddFromTypedModifiers(mods, lvl);
		float premult = GetPreMultFromTypedModifiers(mods, lvl);
		float postadd = GetPostAddFromTypedModifiers(mods, lvl);
		float postmult = GetPostMultFromTypedModifiers(mods, lvl);

		float preadd2 = GetPreAddFromTypedModifiers(mods, nextlvl);
		float premult2 = GetPreMultFromTypedModifiers(mods, nextlvl);
		float postadd2 = GetPostAddFromTypedModifiers(mods, nextlvl);
		float postmult2 = GetPostMultFromTypedModifiers(mods, nextlvl);

		if (preadd > 0 || preadd2 > 0) sb.Append(GetAddUpgradeString(preadd, preadd2, modName, false, col) + "\n");
		if (premult > 0 || premult2 > 0) sb.Append(GetMultUpgradeString(premult, premult2, modName, false, col) + "\n");
		if (postadd > 0 || postadd2 > 0) sb.Append(GetAddUpgradeString(postadd, postadd2, modName, true, col) + "\n");
		if (postmult > 0 || postmult2 > 0) sb.Append(GetMultUpgradeString(postmult, postmult2, modName, true, col) + "\n");
	}

	private void GetTypedModifierInfoText(StringBuilder sb, string modName, List<TypedModifier> mods, int lvl, Color col)
	{
		foreach(TypedModifier m in mods)
		{
			string temp = m.ToString(modName, lvl, col);
			sb.Append(temp);
		}
	}

	private void GetTypedModifierUpgradeText(StringBuilder sb, string modName, List<TypedModifier> mods, int lvl, int nextlvl, Color col)
	{
		foreach (TypedModifier m in mods)
		{
			string temp = m.GetUpgradeString(modName, lvl, nextlvl, col);
			sb.Append(temp);
		}
	}

	private string GetAddString(float v, string n, bool post, Color col)
	{
		return "<#" + ColorUtility.ToHtmlStringRGB(col) + ">+" + v.ToString("F1") + " " + n + "</color> " + (post ? " post" : " pre");
	}

	private string GetMultString(float v, string n, bool post, Color col)
	{
		return "<#" + ColorUtility.ToHtmlStringRGB(col) + ">+" + (v * 100f).ToString("F1") + "% " + n + "</color> " + (post ? " post" : " pre");
	}

	private string GetAddUpgradeString(float v, float v2, string n, bool post, Color col)
	{
		return "<#" + ColorUtility.ToHtmlStringRGB(col) + ">+" + v.ToString("F1") + "->" + v2.ToString("F1") + " " + n + "</color> " + (post ? " post" : " pre");
	}

	private string GetMultUpgradeString(float v, float v2, string n, bool post, Color col)
	{
		return "<#" + ColorUtility.ToHtmlStringRGB(col) + ">+" + (v * 100f).ToString("F1") + "%->" + (v2 * 100f).ToString("F1") + "% " + n + "</color> " + (post ? " post" : " pre");
	}

	#endregion

	public static bool AttackTypeOverlap(AttackType a, AttackType b)
	{
		//if bitwise and is not zero, some bits (enum flags) must be shared
		return (a & b) != 0;
	}

	/// <summary>
	/// Returns the computed value after applying TypedModifiers that match type
	/// </summary>
	/// <param name="modifiers">the modifiers to applu</param>
	/// <param name="type">the type to calculate the value for</param>
	/// <param name="initial">the initial amount, e.g. base attack is 5 before all mods</param>
	/// <returns></returns>
	public static float GetComputedValueFromTypedModifiers(List<TypedModifier> modifiers, List<int> lvl, AttackType type, float initial = 0)
	{
		//base amount
		float x = GetPreAddFromTypedModifiers(modifiers, lvl, type, initial);
		//print("1:" + x);
		//premultiply
		x *= (1 + GetPreMultFromTypedModifiers(modifiers, lvl, type));
		//print("2:" + x);
		//postadd
		x += GetPostAddFromTypedModifiers(modifiers, lvl, type);
		//print("3:" + x);
		//postmultiply
		x *= (1 + GetPostMultFromTypedModifiers(modifiers, lvl, type));

		return x;
	}


	public static float GetPreAddFromTypedModifiers(List<TypedModifier> modifiers, List<int> lvl, AttackType type, float initial)
	{
		float x = initial;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].preadd(lvl[i]);
			}
		}
		return x;
	}

	public static float GetPreMultFromTypedModifiers(List<TypedModifier> modifiers, List<int> lvl, AttackType type)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].premult(lvl[i]);
			}
		}
		return x;
	}

	public static float GetPostAddFromTypedModifiers(List<TypedModifier> modifiers, List<int> lvl, AttackType type)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].postadd(lvl[i]);
			}
		}
		return x;
	}

	public static float GetPostMultFromTypedModifiers(List<TypedModifier> modifiers, List<int> lvl, AttackType type)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].postmult(lvl[i]);
			}
		}
		return x;
	}

	/************************************/

	/// <summary>
	/// Returns the computed value after applying Modifiers
	/// </summary>
	/// <param name="modifiers">the modifiers to applu</param>
	/// <param name="initial">the initial amount, e.g. base attack is 5 before all mods</param>
	/// <returns></returns>
	public static float GetComputedValueFromTypedModifiers(List<Modifier> modifiers, List<int> lvl, float initial = 0)
	{
		//base amount
		float x = GetPreAddFromTypedModifiers(modifiers, lvl, initial);
		//print("1:" + x);
		//premultiply
		x *= (1 + GetPreMultFromTypedModifiers(modifiers, lvl));
		//print("2:" + x);
		//postadd
		x += GetPostAddFromTypedModifiers(modifiers, lvl);
		//print("3:" + x);
		//postmultiply
		x *= (1 + GetPostMultFromTypedModifiers(modifiers, lvl));

		return x;
	}


	public static float GetPreAddFromTypedModifiers(List<Modifier> modifiers, List<int> lvl, float initial = 0)
	{
		float x = initial;
		for (int i = 0; i < modifiers.Count; i++)
		{
			x += modifiers[i].preadd(lvl[i]);
		}
		return x;
	}

	public static float GetPreMultFromTypedModifiers(List<Modifier> modifiers, List<int> lvl)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			x += modifiers[i].premult(lvl[i]);
		}
		return x;
	}

	public static float GetPostAddFromTypedModifiers(List<Modifier> modifiers, List<int> lvl)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			x += modifiers[i].postadd(lvl[i]);
		}
		return x;
	}

	public static float GetPostMultFromTypedModifiers(List<Modifier> modifiers, List<int> lvl)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			x += modifiers[i].postmult(lvl[i]);
		}
		return x;
	}
}
