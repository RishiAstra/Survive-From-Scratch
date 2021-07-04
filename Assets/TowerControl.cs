using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TowerControl : MonoBehaviour
{
    public static TowerControl main;
    public static List<TowerLevelUnlocker> towerLevelUnlockers = new List<TowerLevelUnlocker>();
	
	public int currentLevelInTower;
	public List<Tower> towers;
    public Menu towerMenu;
    public TMP_Text levelSelectedText;
    public TMP_Text guardText;//text to tell the player how many guards killed / how many remaining
	public TMP_Text towerNameText;
    public RectTransform buttonParent;
    public GameObject towerLevelButton;
    public List<TowerSelectionButtonUI> buttons;
    public int buttonIndexSelected;
    public int t;
    public GameObject guardIconGameObject;
    // Start is called before the first frame update
    void Awake()
	{
		if (main != null) Debug.LogError("two TowerControl");
		main = this;

		LoadTowers();

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	//when the scene is loaded, assign the tower index based on the actual scene loaded
	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		if (towers.Count == 0)
		{
			currentLevelInTower = -1;
			return;
		}

		for(int i = 0; i < towers.Count; i++)
		{
			for (int j = 0; j < towers[i].levelsBeaten.Count; j++)
			{
				string expectedName = GameControl.main.mapScenePath + towers[i].GetLevelSceneName(j + 1);
				
				string actualName = arg0.path;
				string assetPrefex = "Assets/";
				if (actualName.IndexOf(assetPrefex) == 0) actualName = actualName.Substring(assetPrefex.Length);
				int index = actualName.LastIndexOf(".");
				if (index != -1) actualName = actualName.Substring(0, index);
				//if()

				print(expectedName + "||" + actualName);
				if (actualName == expectedName)
				{
					currentLevelInTower = j;
					return;
				}
			}
		}

		currentLevelInTower = -1;
		return;		
	}

	private void LoadTowers()
	{
		for (int i = 0; i < towers.Count; i++)
		{
			//if a tower has been saved, load it
			if (File.Exists(GetFileName(towers[i])))
			{
                int length = towers[i].levelsBeaten.Count;
				towers[i].levelsBeaten = JsonConvert.DeserializeObject<List<bool>>(File.ReadAllText(GetFileName(towers[i])));

                //if new levels were added, fill them as not unlocked
                for(int j = towers[i].levelsBeaten.Count; j < length; j++)
				{
                    towers[j].levelsBeaten.Add(false);
				}
			}
		}
	}

	public int GetTowerIndex(string towerName)
	{
		for(int i = 0; i < towers.Count; i++)
		{
            if(towers[i].name == towerName)
			{
                return i;
			}
		}

        Debug.LogError("Tower not found: " + towerName);
        return -1;
	}

	string GetFileName(Tower tower)
    {
        return GetFileDirectory() + tower.name + ".json";
    }

    private static string GetFileDirectory()
    {
        return GameControl.saveDirectory + "towers/";
    }

    void OnDestroy()
	{
		SaveTowers();
	}

	public void SaveTowers()
	{
		Directory.CreateDirectory(GetFileDirectory());
		for (int i = 0; i < towers.Count; i++)
		{
			File.WriteAllText(GetFileName(towers[i]), JsonConvert.SerializeObject(towers[t].levelsBeaten, Formatting.Indented));
		}
	}

	public void SetTowerSelected(int tower)
	{
        t = tower;
        //set buttons count

        while (buttons.Count > towers[t].levelsBeaten.Count)
		{
            Destroy(buttons[buttons.Count - 1].gameObject);
            buttons.RemoveAt(buttons.Count - 1);
		}

        for(int i = buttons.Count; i < towers[t].levelsBeaten.Count; i++)
		{
            GameObject g = Instantiate(towerLevelButton, buttonParent);
            buttons.Add(g.GetComponent<TowerSelectionButtonUI>());
		}

        for (int i = 0; i < towers[t].levelsBeaten.Count; i++)
        {
            buttons[i].SetIndex(i);
            Button b = buttons[i].GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            int dispose_i = i;
            b.onClick.AddListener(() => SetButtonIndexSelected(dispose_i));
        }

        //make sure that selected button is in range
        CheckButtonIndexSelected();

        RefreshSelectedTint();
    }

    public void RefreshSelectedTint()
	{
        if (towers[t] == null) return;
        for (int i = 0; i < towers[t].levelsBeaten.Count; i++)
        {
            buttons[i].SetSelected(false);
        }

        buttons[buttonIndexSelected].SetSelected(true);
    }

    public void SetButtonIndexSelected(int index)
	{
		buttonIndexSelected = index;
        levelSelectedText.text = "Level " + (index + 1);
		CheckButtonIndexSelected();
        RefreshSelectedTint();
	}

	private void CheckButtonIndexSelected()
	{
        if(towers[t] == null)
		{
            buttonIndexSelected = 0;
            return;
		}
		if (buttonIndexSelected < 0) buttonIndexSelected = 0;
		if (buttonIndexSelected >= towers[t].levelsBeaten.Count) buttonIndexSelected = towers[t].levelsBeaten.Count - 1;
	}



    public void EnterCurrentLevel()
	{
		//if within range etc, and unlocked that level
		if (towers[t] != null && towers[t].levelsBeaten.Count > 0 && buttonIndexSelected >= 0 && buttonIndexSelected < towers[t].levelsBeaten.Count && CanEnterLevel(buttonIndexSelected))
		{
			//levels start at 1, not 0, so an offset of 1 is required
			string sceneName = towers[t].GetLevelSceneName(buttonIndexSelected + 1);
			GameControl.main.BeginLoadMapLocation(sceneName);
		}
	}

	public bool CurrentLevelCleared()
	{
		if (t < 0 || t > towers.Count) return false;
		if (currentLevelInTower < 0 || currentLevelInTower >= towers[t].levelsBeaten.Count) return false;

		return towers[t].levelsBeaten[currentLevelInTower];
	}

	public bool CanEnterLevel(int index)
	{
		//if out of range, can't enter
		if (index < 0 || index >= towers[t].levelsBeaten.Count) return false;
		//if first lvl, can enter
		if (index == 0) return true;
		//otherwise can enter only if previous level cleared
		return towers[t].levelsBeaten[index - 1];
	}

	// Update is called once per frame
	void Update()
    {
        int monstersLeft = 0;
        foreach(TowerLevelUnlocker tu in towerLevelUnlockers)
		{
            monstersLeft += tu.GetMonstersLeft();
		}

		if (currentLevelInTower == -1)
		{
			guardText.text = "";
		}
		else
		{
			guardText.text = (monstersLeft > 0) ? (monstersLeft + " monsters left") : "Level Cleared!";
		}

		towerNameText.text = towers[t].name;
    }
}
