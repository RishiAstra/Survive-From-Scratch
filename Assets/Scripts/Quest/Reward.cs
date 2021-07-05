/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using bobStuff;

[System.Serializable]
public class Reward
{
	public enum RewardType
	{
		item,
		money,
		character
	}

	public RewardType type;
	public Item item;
	public int money;
	public string character;
	public string characterPositionReplace;

	public bool TryGetReward()
	{
		switch (type)
		{
			case RewardType.item:
				if (GameControl.main.GetItem(item))//Player
				{
					return true;
				}
				break;
			case RewardType.money:
				GameControl.main.money += money;
				return true;
			case RewardType.character:
				if (!string.IsNullOrEmpty(characterPositionReplace))
				{
					QuestGameObjectActivate q = QuestGameObjectActivate.instances.Find(x => x.myName == characterPositionReplace);
					if (q != null)
					{
						GameControl.main.StartAddNewCharacterToParty(character, true, q.transform.position);
						ProgressTracker.main.activates[characterPositionReplace] = new QuestGameObjectData() {active = false };
						QuestGameObjectActivate.CheckAll();
					}
					else
					{
						GameControl.main.StartAddNewCharacterToParty(character);
					}

				}
				else
				{
					GameControl.main.StartAddNewCharacterToParty(character);
				}
				return true;

		}

		return false;
	}

}
