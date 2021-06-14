/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
	[Tooltip("Can the menu be opened by this")]public bool allowOpen;
	[Tooltip("Can the menu be closed by this")] public bool allowClose;

	[Tooltip("The menu to toggle")] public GameObject menu;
    // Start is called before the first frame update
    public void Toggle()
	{
		if (menu.activeSelf)
		{
			if (allowClose) menu.SetActive(false);
		}
		else
		{
			if (allowOpen) menu.SetActive(true);
		}
		
	}
}
