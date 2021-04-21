using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopKeeper : MonoBehaviour
{

	private void OnMouseDown()
	{
		GameControl.main.shopMenu.ToggleMenu();
	}
}
