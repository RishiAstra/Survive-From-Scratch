using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class KillQuest : IQuest
{

	public string typeToKill;
	public int amountToKill;
	public int amountKilled;
	public string questName;
	public Reward reward;

	private string nextDialoguePath;

	//public KillQuest() 
	//{

	//}

	//public KillQuest(string typeToKill, int amountToKill) : this(typeToKill, amountToKill, 0)
	//{

	//}

	//public KillQuest(string typeToKill, int amountToKill, int amountKilled)
	//{
	//	this.typeToKill = typeToKill;
	//	this.amountToKill = amountToKill;

	//	//you start out with 0 killed
	//	this.amountKilled = amountKilled;
	//}

	public string GetDescription()
	{
		return "Kill <b>" + typeToKill + "</b>: " + amountKilled + "/" + amountToKill;
	}

	public string GetNextDialoguePath()
	{
		return nextDialoguePath;
	}

	public float GetProgress()
	{
		//convert to floats, then use division
		return Mathf.Clamp01(((float)amountKilled) / ((float)amountToKill));
	}

	public string GetQuestName()
	{
		return questName;
	}

	public bool IsFinished()
	{
		return amountKilled >= amountToKill;
	}

	public void OnEntityDamaged(string type, Abilities dmgBy, float dmgAmount)
	{
		//This quest type doesn't care
	}

	public void OnEntityKilled(string type, Abilities killedBy)
	{
		//if it's the type to kill
		if (type == typeToKill)
		{
			//killed by the player
			if (killedBy == GameControl.main.myAbilities)
			{
				amountKilled++;
				if (amountKilled > amountToKill) amountKilled = amountToKill;
			}
		}
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
		//This quest type doesn't care
	}
}
