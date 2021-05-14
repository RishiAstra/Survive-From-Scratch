using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TalkQuest : IQuest
{

	public string toTalkTo;
	public string questName;
	public Reward reward;
	private bool talked;

	private string nextDialoguePath;

	public string GetDescription()
	{
		return "Talk to <b>" + toTalkTo + "</b>";
	}

	public string GetNextDialoguePath()
	{
		return nextDialoguePath;
	}

	public float GetProgress()
	{
		//convert to floats, then use division
		return talked ? 1 : 0;// Mathf.Clamp01(((float)amountKilled) / ((float)amountToKill));
	}

	public string GetQuestName()
	{
		return questName;
	}

	public bool IsFinished()
	{
		return talked;
	}

	public void OnEntityDamaged(string type, Abilities dmgBy, float dmgAmount)
	{
		//This quest type doesn't care
	}

	public void OnEntityKilled(string type, Abilities killedBy)
	{
		//This quest type doesn't care	
	}

	public void OnLocationReached(string location)
	{
		//This quest type doesn't care
	}

	public bool TryCompleteMission()
	{
		if (!IsFinished()) return false;
		return reward == null || reward.TryGetReward();
	}

	public void SetNextDialoguePath(string s)
	{
		nextDialoguePath = s;
	}

	public void OnTalked(string talkedTo)
	{
		if (talkedTo == toTalkTo) talked = true;
	}
}
