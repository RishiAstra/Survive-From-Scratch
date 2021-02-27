using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
	public GameObject menu;
    // Start is called before the first frame update
    void Toggle()
	{
		menu.SetActive(!menu.activeSelf);
	}
}
