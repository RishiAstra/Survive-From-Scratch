using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TowerTeleporter : MonoBehaviour, IMouseHoverable
{
	public string towerName;
	private int t;
	public GameObject onHover;
	
	// Start is called before the first frame update
	void Start()
	{
		onHover.SetActive(false);
		t = TowerControl.main.GetTowerIndex(towerName);
	}

	public void OnMouseHoverFromRaycast()
	{
		onHover.SetActive(true);

		if (InputControl.InteractKeyDown())
		{
			t = TowerControl.main.GetTowerIndex(towerName);
			TowerControl.main.SetTowerSelected(t);
			TowerControl.main.SetButtonIndexSelected(0);
			TowerControl.main.towerMenu.TryActivateMenu();
		}
	}

	public void OnMouseStopHoverFromRaycast()
	{
		onHover.SetActive(false);
	}

	

	// Update is called once per frame
	void Update()
	{
		if(TowerControl.main.towerMenu.gameObject.activeSelf && InputControl.InteractKeyDown())
		{
			TowerControl.main.towerMenu.TryDeactivateMenu();
		}
	}

	//void OnValidate()
	//{
	//	Tower tower = TowerControl.main.towers[t];
	//	if (tower.unlockedLevels.Count == 0) tower.unlockedLevels.Add(true);
	//	tower.unlockedLevels[0] = true;
	//}

	//string GetFileName()
	//{
	//	return GetFileDirectory() + t.name;
	//}

	//private static string GetFileDirectory()
	//{
	//	return GameControl.saveDirectory + "towers/";
	//}

	//void OnDestroy()
	//{
	//	Directory.CreateDirectory(GetFileDirectory());
	//	File.WriteAllText(GetFileName(), JsonConvert.SerializeObject(t, Formatting.Indented));
	//}

}

[System.Serializable]
public class Tower
{
	public string name;
	public List<bool> levelsBeaten;

	public Tower(string name, int floorCount)
	{
		this.name = name;
		this.levelsBeaten = new List<bool>();
		for(int i = 0; i < floorCount; i++)
		{
			levelsBeaten.Add(false);
		}
	}

	public string GetLevelSceneName(int level)
	{
		return name + "/" + level;
	}

}
