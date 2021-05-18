using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static int openMenuCount;
	public static List<Menu> activeMenus = new List<Menu>();

    public bool pauseOnActive = true;
	public bool activateOnEsc = false;

	private bool initialized;
    // Start is called before the first frame update
    void Awake()
    {
  //      if (!initialized && gameObject.activeSelf)
		//{
		//	ActivateMenu();
		//}
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyUp(KeyCode.Escape) && activeMenus.Count > 0)
		{
			//disable all menus
			for (int i = 0; i < activeMenus.Count;)
			{
				if (activeMenus[i].activateOnEsc)
				{
					activeMenus[i].TryActivateMenu();
					i++;
				}
				else
				{
					activeMenus[i].TryDeactivateMenu();
					//activeMenus.RemoveAt(i);
				}
				
			}
			Cursor.lockState = CursorLockMode.Locked;
		}
    }

    public void ToggleMenu()
    {
        if (gameObject.activeSelf)
        {
            TryDeactivateMenu();
        }
        else
        {
            TryActivateMenu();
        }
    }

    public void TryActivateMenu()
	{
		if (!gameObject.activeSelf)
		{
			ActivateMenu();
		}

	}

	private void ActivateMenu()
	{
		initialized = true;
		if(GameControl.playerExists) DisableAllMenus(this);

		openMenuCount++;
		activeMenus.Add(this);

		gameObject.SetActive(true);
		if (pauseOnActive)
		{
			TimeControl.main.SetTimeScale(0, "menu");
		}
		if (GameControl.main != null) GameControl.main.TryUnlockCursor();
		//print("activated menu: " + gameObject.name + ", " + openMenuCount);
	}

	//disables all menus except 1
	private static void DisableAllMenus(Menu exclude)
	{
		//disable all menus
		for (int i = 0; i < activeMenus.Count;)
		{
			if(activeMenus[i] == exclude)
			{
				i++;
			}
			else
			{
				activeMenus[i].TryDeactivateMenu();
				//activeMenus.RemoveAt(i);
			}			
		}
	}

	public void TryDeactivateMenu()
	{
		if (gameObject.activeSelf)
		{
			DeactivateMenu();
		}
	}

	private void DeactivateMenu()
	{
		openMenuCount--;
		activeMenus.Remove(this);
		gameObject.SetActive(false);
		if (pauseOnActive)
		{
			TimeControl.main.RemoveTimeScale("menu");
		}
		GameControl.main.TryLockCursor();
		//print("deactivated menu: " + gameObject.name + ", " + openMenuCount);
	}
}
