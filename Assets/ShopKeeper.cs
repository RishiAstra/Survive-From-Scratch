using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopKeeper : MonoBehaviour, IMouseHoverable
{
	public GameObject onHover;

	private void Start()
	{
		onHover.SetActive(false);
	}
	public void OnMouseHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(true);
		if (Input.GetMouseButtonDown(0))
		{
			GameControl.main.shopMenu.ToggleMenu();
		}
	}

	public void OnMouseStopHoverFromRaycast()
	{
		if(onHover != null) onHover.SetActive(false);
	}

	//private void OnMouseDown()
	//{
	//	GameControl.main.shopMenu.ToggleMenu();
	//}
}
