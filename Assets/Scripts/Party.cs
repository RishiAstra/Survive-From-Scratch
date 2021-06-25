using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Party
{
    public List<long> myIds;

    public void TeleportAll (string mapName)
	{

		string path = GameControl.main.mapScenePath + mapName;

		for (int i = 0; i < myIds.Count; i++)
		{

			SaveEntity.TeleportEntityBetweenScenes(myIds[i], SceneUtility.GetBuildIndexByScenePath(path));
		}
	}

	public Party()
	{
		myIds = new List<long>();
	}
}
