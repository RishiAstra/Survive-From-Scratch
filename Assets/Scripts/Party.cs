using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Party
{
    public List<PartyMember> members;
	public int lastUsed;

    public void TeleportAll (string mapName)
	{

		string path = GameControl.main.mapScenePath + mapName;

		for (int i = 0; i < members.Count; i++)
		{

			SaveEntity.TeleportEntityBetweenScenes(members[i].id, SceneUtility.GetBuildIndexByScenePath(path));
		}
	}

	public Party()
	{
		members = new List<PartyMember>();
	}
}

[System.Serializable]
public class PartyMember
{
	public long id;
	public string type;
	public string name;
	[System.NonSerialized]
	public GameObject g;
}
