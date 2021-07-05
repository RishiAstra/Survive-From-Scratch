/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bobStuff;

[System.Serializable]
public class SceneQuest : IQuest
{

	public string sceneToVisit;
	public string descriptionText;
	public string questName;
	public Reward reward;
	private bool visited;

	private string nextDialoguePath;

	public string GetDescription()
	{
		string temp = string.IsNullOrEmpty(descriptionText) ? ("Enter <b>" + sceneToVisit + "</b>") : descriptionText;
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

	public void OnSceneReached(string scene)
	{
		if (scene == sceneToVisit)
		{
			visited = true;
			NotificationControl.main.AddNotification(
				new Notification()
				{
					message = GetDescription() + " <#00ff00>Complete</color>"
				}
			);
		}

		
	}

	public void OnItemObtained(Item i)
	{
		//This quest type doesn't care
	}
}
