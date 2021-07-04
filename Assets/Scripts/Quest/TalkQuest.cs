/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bobStuff;

[System.Serializable]
public class TalkQuest : IQuest
{

	public string toTalkTo;
	public string questName;
	public Reward reward;
	public int questIndicatorIndex = 1;
	private bool talked;

	private string nextDialoguePath;
	[System.NonSerialized]
	private GameObject questIndicator;

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
		if (questIndicator == null)
		{
			DialogueOnClick d = DialogueOnClick.GetInstance(toTalkTo);
			if (d != null)
			{
				questIndicator = GameObject.Instantiate(ProgressTracker.main.questWorldIndicators[questIndicatorIndex], d.transform);
			}
		}
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
		GameObject.Destroy(questIndicator);
		return reward == null || reward.TryGetReward();
	}

	public void SetNextDialoguePath(string s)
	{
		nextDialoguePath = s;
	}

	public void OnTalked(string talkedTo)
	{
		if (talkedTo == toTalkTo)
		{
			talked = true;
			NotificationControl.main.AddNotification(
				new Notification()
				{
					message = GetDescription() + " <#00ff00>Complete</color>"
				}
			);
		}		
	}

	public void OnSceneReached(string scene)
	{
		//This quest type doesn't care
	}

	public void OnItemObtained(Item i)
	{
		//This quest type doesn't care
	}
}
