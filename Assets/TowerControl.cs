using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerControl : MonoBehaviour
{
    public static TowerControl main;

    public List<Tower> towers;
    public Menu towerMenu;
    public TMP_Text levelSelectedText;
    public RectTransform buttonParent;
    public GameObject towerLevelButton;
    public List<TowerSelectionButtonUI> buttons;
    public int buttonIndexSelected;
    public int t;
    // Start is called before the first frame update
    void Awake()
	{
		if (main != null) Debug.LogError("two TowerControl");
		main = this;

		LoadTowers();
	}

	private void LoadTowers()
	{
		for (int i = 0; i < towers.Count; i++)
		{
			//if a tower has been saved, load it
			if (File.Exists(GetFileName(towers[i])))
			{
				towers[i] = JsonConvert.DeserializeObject<Tower>(File.ReadAllText(GetFileName(towers[i])));
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
			File.WriteAllText(GetFileName(towers[i]), JsonConvert.SerializeObject(towers[t], Formatting.Indented));
		}
	}

	public void SetTowerSelected(int tower)
	{
        t = tower;
        //set buttons count

        while (buttons.Count > towers[t].unlockedLevels.Count)
		{
            Destroy(buttons[buttons.Count - 1].gameObject);
            buttons.RemoveAt(buttons.Count - 1);
		}

        for(int i = buttons.Count; i < towers[t].unlockedLevels.Count; i++)
		{
            GameObject g = Instantiate(towerLevelButton, buttonParent);
            buttons.Add(g.GetComponent<TowerSelectionButtonUI>());
		}

        for (int i = 0; i < towers[t].unlockedLevels.Count; i++)
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
        for (int i = 0; i < towers[t].unlockedLevels.Count; i++)
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
		if (buttonIndexSelected >= towers[t].unlockedLevels.Count) buttonIndexSelected = towers[t].unlockedLevels.Count - 1;
	}

    public void EnterCurrentLevel()
	{
        //if within range etc, and unlocked that level
        if(towers[t] != null && towers[t].unlockedLevels.Count > 0 && buttonIndexSelected >= 0 && buttonIndexSelected < towers[t].unlockedLevels.Count && towers[t].unlockedLevels[buttonIndexSelected])
		{
            //levels start at 1, not 0, so an offset of 1 is required
            GameControl.main.BeginLoadMapLocation(towers[t].GetLevelSceneName(buttonIndexSelected + 1));
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
