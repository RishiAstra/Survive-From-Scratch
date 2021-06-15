/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using System.IO;

[RequireComponent(typeof(PersistantSaveID))]
[ExecuteInEditMode]
public class ShopKeeper : MonoBehaviour, IMouseHoverable
{
	public GameObject onHover;

	public List<ShopItem> buyDeals;
	public List<ShopItem> sellDeals;//NOTE: don't put 2 of the same item in here

	private PersistantSaveID myId;
	private string savePath
	{
		get { return Application.streamingAssetsPath + "/PersistantID/Shopkeepers/" + myId.id + "/"; }
	}

	private void Awake()
	{
		myId = GetComponent<PersistantSaveID>();
		onHover.SetActive(false);
	}



	public void OnEnable()
	{
		string sellPath = savePath + "sell.json";
		if(File.Exists(sellPath)) sellDeals = JsonConvert.DeserializeObject<List<ShopItem>>(File.ReadAllText(sellPath));
		string buyPath = savePath + "buy.json";
		if(File.Exists(buyPath)) buyDeals = JsonConvert.DeserializeObject<List<ShopItem>>(File.ReadAllText(buyPath));
	}

#if UNITY_EDITOR

	public void SaveDeals()
	{
		Directory.CreateDirectory(savePath);
		File.WriteAllText(savePath + "sell.json", JsonConvert.SerializeObject(sellDeals, Formatting.Indented));
		File.WriteAllText(savePath + "buy.json", JsonConvert.SerializeObject(buyDeals, Formatting.Indented));
		print("saved shopkeeper");
	}

#endif

	public void OnMouseHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(true);
		if (!GameControl.main.shopMenu.gameObject.activeSelf && !Menu.IsOtherMenuActive(GameControl.main.shopMenu) && InputControl.InteractKeyDown())
		{
			ShopControl.main.current = this;
			ShopControl.main.UpdateShopInventoryUI();
			GameControl.main.shopMenu.ToggleMenu();
		}
	}

	private void Update()
	{
		if (GameControl.main == null) return;

		if (GameControl.main.shopMenu.gameObject.activeSelf && InputControl.InteractKeyDown())
		{
			GameControl.main.shopMenu.TryDeactivateMenu();
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
