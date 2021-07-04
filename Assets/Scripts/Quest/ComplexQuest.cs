/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ComplexQuest : IQuest
{

	public List<IQuest> quests;
	public int current;
	public Reward reward;
	public Dictionary<string, QuestGameObjectData> toActivate;

	private string nextDialoguePath;

	public string GetDescription()
	{
		if(current < quests.Count)
		{
			return quests[current].GetDescription();
		}
		return "empty complex quest";
	}

	public string GetNextDialoguePath()
	{
		if (current < quests.Count)
		{
			return quests[current].GetNextDialoguePath();
		}
		return null;
	}

	public float GetProgress()
	{
		if (current < quests.Count)
		{
			return quests[current].GetProgress();
		}
		return 0;
	}

	public string GetQuestName()
	{
		if (current < quests.Count)
		{
			return quests[current].GetQuestName();
		}
		return null;
	}

	public bool IsFinished()
	{
		return current >= quests.Count;
	}

	public void OnEntityDamaged(string type, Abilities dmgBy, float dmgAmount)
	{
		if (current < quests.Count)
		{
			quests[current].OnEntityDamaged(type, dmgBy, dmgAmount);
		}
		UpdateMyQuests();
	}

	public void OnEntityKilled(string type, Abilities killedBy)
	{
		if (current < quests.Count)
		{
			quests[current].OnEntityKilled(type, killedBy);
		}
		UpdateMyQuests();
	}

	public void OnLocationReached(string location)
	{
		if (current < quests.Count)
		{
			quests[current].OnLocationReached(location);
		}
		UpdateMyQuests();
	}

	public bool TryCompleteMission()
	{
		if (!IsFinished()) return false;
		bool ok = reward == null || reward.TryGetReward();
		if (ok && toActivate != null)
		{
			//add (overwright if already) keyvalues
			foreach(KeyValuePair<string, QuestGameObjectData> v in toActivate)
			{
				ProgressTracker.main.activates[v.Key] = v.Value;
			}
		}
		return ok;
	}

	public void SetNextDialoguePath(string s)
	{
		nextDialoguePath = s;
	}

	public void OnTalked(string talkedTo)
	{
		if (current < quests.Count)
		{
			quests[current].OnTalked(talkedTo);
		}
		UpdateMyQuests();
	}

	private void UpdateMyQuests()
	{
		if (current < quests.Count && quests[current].IsFinished() && quests[current].TryCompleteMission())
		{
			current++;

			if (current < quests.Count)
			{
				//when part of a complex quest is completed, show a notification saying what the next part is
				NotificationControl.main.AddNotification(
					new Notification()
					{
						message = GetDescription()
					}
				);
			}
			
		}
	}

	public void OnSceneReached(string scene)
	{
		if (current < quests.Count)
		{
			quests[current].OnSceneReached(scene);
		}
		UpdateMyQuests();
	}
}
