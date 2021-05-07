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

	public KillQuest(string typeToKill, int amountToKill) : this(typeToKill, amountToKill, 0)
	{
		
	}

	public KillQuest(string typeToKill, int amountToKill, int amountKilled)
	{
		this.typeToKill = typeToKill;
		this.amountToKill = amountToKill;

		//you start out with 0 killed
		this.amountKilled = amountKilled;
	}

	public string GetDescription()
	{
		return "Kill <b>" + typeToKill + "</b>: " + amountKilled + "/" + amountToKill;
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
		if(type == typeToKill)
		{
			//killed by the player
			if(killedBy == GameControl.main.myAbilities)
			{
				amountKilled++;
			}
		}		
	}

	public void OnLocationReached(string location)
	{
		//This quest type doesn't care
	}
}
