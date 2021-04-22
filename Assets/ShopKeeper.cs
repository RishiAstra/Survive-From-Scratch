using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopKeeper : MonoBehaviour, IMouseHoverable
{
	public void OnMouseHoverFromRaycast()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GameControl.main.shopMenu.ToggleMenu();
		}
	}

	//private void OnMouseDown()
	//{
	//	GameControl.main.shopMenu.ToggleMenu();
	//}
}
