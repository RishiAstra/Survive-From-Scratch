using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
	public Menu escapeMenu;
    // Start is called before the first frame update
    void Start()
    {

    }

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			if (Menu.activeMenus.Count > 0)
			{
				//Cursor.lockState = CursorLockMode.Locked;
				//disable all menus
				//the disabling function will reduce the number of active menus, so i++ not required
				for (int i = 0; i < Menu.activeMenus.Count;)
				{
					Menu.activeMenus[i].TryDeactivateMenu();
				}
			}
			else
			{
				escapeMenu.TryActivateMenu();
			}
		}
	}
}
