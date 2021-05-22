﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatSkill", menuName = "ScriptableObjects/StatSkill", order = 1)]
[System.Serializable]
public class StatSkill : Skill
{
	public ModifierGroup mods;
}
