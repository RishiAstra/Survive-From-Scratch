using bobStuff;

[System.Serializable]
public class Reward
{
	public enum RewardType
	{
		item,
		money
	}

	public RewardType type;
	public Item item;
	public int money;

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
				break;

		}

		return false;
	}

}
