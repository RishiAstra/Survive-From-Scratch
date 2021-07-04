/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bobStuff;
using System;
using System.Text;

[System.Serializable]
public class GiveQuest : IQuest 
{

	public string toTalkTo;
	public string questName;
	public List<Item> items;
	public Reward reward;
	public int questIndicatorIndex = 1;
	private bool talked;

	private string nextDialoguePath;
	[System.NonSerialized]
	private GameObject questIndicator;

	public string GetDescription()
	{
		return GetDescription(-1);
	}
	public string GetDescription(int emphaziseIndex)
	{
		StringBuilder sb = new StringBuilder("Give <b>");

		Inventory mainInv = GameControl.main.mainInventoryUI.target;
		Inventory hotBar = GameControl.main.myAbilities.GetComponent<Inventory>();
		Inventory craftInv = Crafting.main.craftInventory;
		if (mainInv == null || hotBar == null || craftInv == null)
		{
			Debug.LogError("inventory not found. Main:" + (mainInv != null) + ", hotbar:" + (hotBar != null) + ", craft:" + (craftInv != null));

			for (int i = 0; i < items.Count; i++)
			{
				Item it = items[i];
				sb.Append(it.amount + " " + GameControl.itemTypes[it.id].name + ", ");
			}
			sb.Remove(sb.Length - 3, 2);//remove last comma
			sb.Append("</b> to " + toTalkTo);
			return sb.ToString();
		}
		else
		{
			List<Inventory> invs = new List<Inventory>();
			invs.Add(hotBar);
			invs.Add(mainInv);
			invs.Add(craftInv);

			List<int> itemCounts = GetItemCounts(invs);

			for (int i = 0; i < items.Count; i++)
			{
				Item it = items[i];
				if (i == emphaziseIndex) sb.Append("<#00FF00>");
				sb.Append(Mathf.Min(itemCounts[i], it.amount) + "/" + it.amount + " " + GameControl.itemTypes[it.id].name + ", ");
				if (i == emphaziseIndex) sb.Append("</color>");
			}
			sb.Remove(sb.Length - 3, 2);//remove last comma
			sb.Append("</b> to " + toTalkTo);
			return sb.ToString();
		}

		

		//List<int> itemCounts = GetItemCounts(invs);
		//if (mainInv == null || hotBar == null)
		//{
		//	Debug.LogError("inventory not found. Main:" + (mainInv != null) + ", hotbar:" + (hotBar != null));
			
		//	for (int i = 0; i < items.Count; i++)
		//	{
		//		Item it = items[i];
		//		sb.Append(it.amount + " " + GameControl.itemTypes[it.id].name + ", ");
		//	}
		//	sb.Remove(sb.Length - 3, 2);//remove last comma
		//	sb.Append("</b> to " + toTalkTo);
		//	return sb.ToString();
		//}
		//else
		//{
		//	List<int> itemCounts = GetItemCounts(mainInv, hotBar);

		//	for (int i = 0; i < items.Count; i++)
		//	{
		//		Item it = items[i];
		//		if (i == emphaziseIndex) sb.Append("<#00FF00>");
		//		sb.Append(Mathf.Min(itemCounts[i], it.amount) + "/" + it.amount + " " + GameControl.itemTypes[it.id].name + ", ");
		//		if (i == emphaziseIndex) sb.Append("</color>");
		//	}
		//	sb.Remove(sb.Length - 3, 2);//remove last comma
		//	sb.Append("</b> to " + toTalkTo);
		//	return sb.ToString();
		//}		
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
		//don't TryTakeItems() twice
		if (!talked && talkedTo == toTalkTo && TryTakeItems())
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

	private bool TryTakeItems()
	{
		Inventory mainInv = GameControl.main.mainInventoryUI.target;
		Inventory hotBar = GameControl.main.myAbilities.GetComponent<Inventory>();
		Inventory craftInv = Crafting.main.craftInventory;
		if (mainInv == null || hotBar == null || craftInv == null)
		{
			Debug.LogError("inventory not found. Main:" + (mainInv != null) + ", hotbar:" + (hotBar != null) + ", craft:" + (craftInv != null));
			return false;
		}

		List<Inventory> invs = new List<Inventory>();
		invs.Add(hotBar);
		invs.Add(mainInv);
		invs.Add(craftInv);
		
		List<int> itemCounts = GetItemCounts(invs);

		//if has too little, can't give items and finish quest etc
		for(int i = 0; i < items.Count; i++)
		{
			if (itemCounts[i] < items[i].amount) return false;
		}

		//if it reaches here, there are enough items. Now remove those items

		//go through the required ingredients
		for (int j = 0; j < items.Count; j++)
		{
			int required = items[j].amount;

			foreach (Inventory h in invs) { 
				//go through the inventory to remove the ingredients
				for (int k = 0; k < h.items.Count; k++)
				{
					if (h.items[k].id == items[j].id)
					{
						if (required >= h.items[k].amount)
						{
							//remove all of this item, because even all of it isn't enough
							Item temp = h.items[k];
							required -= temp.amount;//remove the ingredient used
							temp.amount = 0;
							temp.id = 0;
							h.items[k] = temp;
						}
						else
						{
							//remove as much as needed, some of this item will be left
							Item temp = h.items[k];
							temp.amount -= required;
							required = 0;
							h.items[k] = temp;
							break;//we are done fulfilling this ingredient spending requirement for the recipie
						}
					}
				}

				//skip other inventories if already have enough
				if (required <= 0)
				{
					if (required == 0) break;
					else Debug.LogError("Somehow took too much items for givequest, required: " + required);
				}

			}


			////go through the inventory to remove the ingredients
			//for (int k = 0; k < mainInv.items.Count; k++)
			//{
			//	if (mainInv.items[k].id == items[j].id)
			//	{
			//		if (required >= mainInv.items[k].amount)
			//		{
			//			//remove all of this item, because even all of it isn't enough
			//			Item temp = mainInv.items[k];
			//			required -= temp.amount;//remove the ingredient used
			//			temp.amount = 0;
			//			temp.id = 0;
			//			mainInv.items[k] = temp;
			//		}
			//		else
			//		{
			//			//remove as much as needed, some of this item will be left
			//			Item temp = mainInv.items[k];
			//			temp.amount -= required;
			//			required = 0;
			//			mainInv.items[k] = temp;
			//			break;//we are done fulfilling this ingredient spending requirement for the recipie
			//		}
			//	}
			//}


			//TODO: make this better
			//error if we still need to take more ingredient, but there is none left to take. This means that the crafting was illegitimate
			if (required > 0) Debug.LogError("Not enough of ingredient " + "for give quest");
		}

		//if it got this far, it had the items and removed them successfully

		return true;
	}

	private List<int> GetItemCounts(List<Inventory> invs)
	{
		List<int> result = new List<int>();
		for (int j = 0; j < items.Count; j++)
		{
			int count = 0;
			foreach (Inventory mainInv in invs)
			{
				//go through the craft inventory to find the ingredients
				for (int k = 0; k < mainInv.items.Count; k++)
				{
					if (mainInv.items[k].id == items[j].id)
					{
						count += mainInv.items[k].amount;

					}
				}
			}

			result.Add(count);

		}

		return result;
	}

	public void OnSceneReached(string scene)
	{
		//This quest type doesn't care
	}

	public void OnItemObtained(Item it)
	{
		int index = -1;
		for (int i = 0; i < items.Count; i++)
		{
			if(items[i].id == it.id)
			{
				index = i;
				break;
			}
		}

		if(index >= 0 && index < items.Count)
		{
			NotificationControl.main.AddNotification(
				new Notification()
				{
					message = GetDescription(index)
				}
			);
		}
	}
}
