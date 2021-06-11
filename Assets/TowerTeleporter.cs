using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTeleporter : MonoBehaviour, IMouseHoverable
{
    public Tower t;
    public GameObject onHover;

	public void OnMouseHoverFromRaycast()
	{
        onHover.SetActive(true);

		if (InputControl.InteractKeyDown())
		{
            TowerControl.main.towerMenu.ToggleMenu();
		}
	}

	public void OnMouseStopHoverFromRaycast()
	{
        onHover.SetActive(false);
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TowerControl.main.towerMenu.gameObject.activeSelf && InputControl.InteractKeyDown())
		{
            TowerControl.main.towerMenu.TryDeactivateMenu();
		}
    }
}

[System.Serializable]
public class Tower
{
    public string name;
    public List<bool> unlockedLevels;

    public Tower(string name, int floorCount)
	{
        this.name = name;
        this.unlockedLevels = new List<bool>();
        for(int i = 0; i < floorCount; i++)
		{
            unlockedLevels.Add(false);
		}
	}

    public string GetLevelSceneName(int level)
	{
        return name + level;
	}

}
