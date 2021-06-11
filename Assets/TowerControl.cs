using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerControl : MonoBehaviour
{
    public static TowerControl main;

    public Menu towerMenu;
    public TMP_Text levelSelectedText;
    public RectTransform buttonParent;
    public GameObject towerLevelButton;
    public List<TowerSelectionButtonUI> buttons;
    public int buttonIndexSelected;
    public Tower t;
    // Start is called before the first frame update
    void Start()
    {
        if (main != null) Debug.LogError("two TowerControl");
        main = this;
    }

    public void SetTowerSelected(Tower tower)
	{
        t = tower;
        //set buttons count

        while (buttons.Count > t.unlockedLevels.Count)
		{
            Destroy(buttons[buttons.Count - 1].gameObject);
            buttons.RemoveAt(buttons.Count - 1);
		}

        for(int i = buttons.Count; i < t.unlockedLevels.Count; i++)
		{
            GameObject g = Instantiate(towerLevelButton, buttonParent);
            buttons.Add(g.GetComponent<TowerSelectionButtonUI>());
		}

        for (int i = 0; i < t.unlockedLevels.Count; i++)
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
        if (t == null) return;
        for (int i = 0; i < t.unlockedLevels.Count; i++)
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
        if(t == null)
		{
            buttonIndexSelected = 0;
            return;
		}
		if (buttonIndexSelected < 0) buttonIndexSelected = 0;
		if (buttonIndexSelected >= t.unlockedLevels.Count) buttonIndexSelected = t.unlockedLevels.Count - 1;
	}

    public void EnterCurrentLevel()
	{
        //if within range etc, and unlocked that level
        if(t != null && t.unlockedLevels.Count > 0 && buttonIndexSelected >= 0 && buttonIndexSelected < t.unlockedLevels.Count && t.unlockedLevels[buttonIndexSelected])
		{
            //levels start at 1, not 0, so an offset of 1 is required
            GameControl.main.BeginLoadMapLocation(t.GetLevelSceneName(buttonIndexSelected + 1));
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
