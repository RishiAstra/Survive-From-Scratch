/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bobStuff;

[System.Serializable]
public class LocationQuest : IQuest
{

	public string locationToVisit;
	public string descriptionText;
	public string questName;
	public Reward reward;
	public int questIndicatorIndex = 1;
	private bool visited;

	private string nextDialoguePath;
	[System.NonSerialized]
	private GameObject questIndicator;

	public string GetDescription()
	{
		string temp = string.IsNullOrEmpty(descriptionText) ? ("Go to <b>" + locationToVisit + "</b>") : descriptionText;
		return temp;
	}

	public string GetNextDialoguePath()
	{
		return nextDialoguePath;
	}

	public float GetProgress()
	{
		//convert to floats, then use division
		return visited ? 1 : 0;// Mathf.Clamp01(((float)amountKilled) / ((float)amountToKill));
	}

	public string GetQuestName()
	{
		return questName;
	}

	public bool IsFinished()
	{
		if (questIndicator == null)
		{
			LocationCheck d = LocationCheck.GetInstance(locationToVisit);
			if (d != null)
			{
				questIndicator = GameObject.Instantiate(ProgressTracker.main.questWorldIndicators[questIndicatorIndex], d.transform);
			}
		}
		return visited;
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

		if (location == locationToVisit){
			visited = true;
			NotificationControl.main.AddNotification(
				new Notification()
				{
					message = GetDescription() + " <#00ff00>Complete</color>",
					sentFrom = this
				}
			);
		}
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
		//This quest type doesn't care
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
