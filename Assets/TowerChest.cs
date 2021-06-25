using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerChest : Chest
{
	public override bool IsUnlocked()
	{
		//tower chest is unlocked if current level cleared
		return TowerControl.main.CurrentLevelCleared();
	}
}
