using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerSelectionButtonUI : MonoBehaviour
{
    public TMP_Text levelText;
    public GameObject tint;
    public GameObject disabledTint;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetIndex(int i)
	{
        index = i;
        levelText.text = "Level " + (index + 1);
	}

    public void SetSelected(bool selected)
	{
        tint.SetActive(selected);
	}

    public void ClickMe()
	{
        TowerControl.main.SetButtonIndexSelected(index);
	}

    // Update is called once per frame
    void Update()
    {
        //show disabled tint if not allwoed to select this tower level
        disabledTint.SetActive(!TowerControl.main.towers[TowerControl.main.t].unlockedLevels[index]);
    }
}
