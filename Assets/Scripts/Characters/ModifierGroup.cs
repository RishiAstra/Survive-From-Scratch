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

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		GetTypedModifierInfoText(sb, "DEF", globalArmorModifiers, GameControl.main.armorColor);
		GetTypedModifierInfoText(sb, "ATK", atkMods, GameControl.main.atkColor);
		GetModifierInfoText(sb, "HP", hpMods, GameControl.main.hpColor);
		GetModifierInfoText(sb, "MP", mpMods, GameControl.main.mpColor);
		GetModifierInfoText(sb, "ENG", engMods, GameControl.main.engColor);
		GetModifierInfoText(sb, "MOR", morMods, GameControl.main.morColor);
		return sb.ToString();
	}

	private void GetModifierInfoText(StringBuilder sb, string modName, List<Modifier> mods, Color col)
	{
		float preadd = GetPreAddFromTypedModifiers(mods);
		float premult = GetPreMultFromTypedModifiers(mods);
		float postadd = GetPostAddFromTypedModifiers(mods);
		float postmult = GetPostMultFromTypedModifiers(mods);
		if (preadd > 0) sb.Append(GetAddString(preadd, modName, false, col) + "\n");
		if (premult > 0) sb.Append(GetMultString(premult, modName, false, col) + "\n");
		if (postadd > 0) sb.Append(GetAddString(postadd, modName, true, col) + "\n");
		if (postmult > 0) sb.Append(GetMultString(postmult, modName, true, col) + "\n");
	}

	private void GetTypedModifierInfoText(StringBuilder sb, string modName, List<TypedModifier> mods, Color col)
	{
		foreach(TypedModifier m in mods)
		{
			string temp = m.ToString(modName, col);
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
	public static float GetComputedValueFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial = 0)
	{
		//base amount
		float x = GetPreAddFromTypedModifiers(modifiers, type, initial);
		//print("1:" + x);
		//premultiply
		x *= (1 + GetPreMultFromTypedModifiers(modifiers, type, initial));
		//print("2:" + x);
		//postadd
		x += GetPostAddFromTypedModifiers(modifiers, type, initial);
		//print("3:" + x);
		//postmultiply
		x *= (1 + GetPostMultFromTypedModifiers(modifiers, type, initial));

		return x;
	}


	public static float GetPreAddFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial)
	{
		float x = initial;
		for (int i = 0; i < modifiers.Count; i++)
		{
			if (AttackTypeOverlap(modifiers[i].type, type))
			{
				x += modifiers[i].preadd;
			}
		}
		return x;
	}

	public static float GetPreMultFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial)
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

	public static float GetPostAddFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial)
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

	public static float GetPostMultFromTypedModifiers(List<TypedModifier> modifiers, AttackType type, float initial)
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

	/************************************/

	/// <summary>
	/// Returns the computed value after applying Modifiers
	/// </summary>
	/// <param name="modifiers">the modifiers to applu</param>
	/// <param name="initial">the initial amount, e.g. base attack is 5 before all mods</param>
	/// <returns></returns>
	public static float GetComputedValueFromTypedModifiers(List<Modifier> modifiers, float initial = 0)
	{
		//base amount
		float x = GetPreAddFromTypedModifiers(modifiers, initial);
		//print("1:" + x);
		//premultiply
		x *= (1 + GetPreMultFromTypedModifiers(modifiers));
		//print("2:" + x);
		//postadd
		x += GetPostAddFromTypedModifiers(modifiers);
		//print("3:" + x);
		//postmultiply
		x *= (1 + GetPostMultFromTypedModifiers(modifiers));

		return x;
	}


	public static float GetPreAddFromTypedModifiers(List<Modifier> modifiers, float initial = 0)
	{
		float x = initial;
		for (int i = 0; i < modifiers.Count; i++)
		{
			x += modifiers[i].preadd;
		}
		return x;
	}

	public static float GetPreMultFromTypedModifiers(List<Modifier> modifiers)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			x += modifiers[i].premult;
		}
		return x;
	}

	public static float GetPostAddFromTypedModifiers(List<Modifier> modifiers)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			x += modifiers[i].postadd;
		}
		return x;
	}

	public static float GetPostMultFromTypedModifiers(List<Modifier> modifiers)
	{
		float x = 0;
		for (int i = 0; i < modifiers.Count; i++)
		{
			x += modifiers[i].postmult;
		}
		return x;
	}
}
