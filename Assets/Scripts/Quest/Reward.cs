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
				GameControl.main.AddNewCharacterToParty(character);
				return true;

		}

		return false;
	}

}
