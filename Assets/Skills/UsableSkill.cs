using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "ScriptableObjects/UsableSkill", order = 1)]
[System.Serializable]
public class UsableSkill : Skill
{
	public Stat cost;
	//public float useDist;//use this for AI to determine what skill to use or if it needs to approach
	//public float useAngle;
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
