/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bobStuff;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class IntUnityEvent: UnityEvent<int>
{

}

public class Inventory : MonoBehaviour, ISaveable
{
	public bool take;//permissions for if the current player can take and place items in this inventory
	public bool put;//TODO: permissions might be a list of players who can access
	public int size;
	public List<Item> items;

	public IntUnityEvent invChange;//called when the inventory is changed (trigger this by script, manual inspector change won't trigger this)
								   //public InventoryUI ui;
								   // Start is called before the first frame update
	public IntUnityEvent invClicked;
	void Awake()
	{
		InitializeIfEmpty();
	}

	void Start()
	{
		InitializeIfEmpty();
	}

	public void InitializeIfEmpty()
	{
		if (items.Count == 0)
		{
			items = new List<Item>();
			for (int i = 0; i < size; i++)
			{
				items.Add(new Item());
			}
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	//TODO: use this everywhere
	void SetInv(Item item, int index)
	{
		//Item temp = items[index];
		if (item.amount == 0) item.id = 0;
		items[index] = item;
	}

	//   void RefreshUI()
	//{
	//       if(ui != null && ui.gameObject.activeInHierarchy)
	//	{
	//           ui.Refresh();
	//	}
	//}

	public JObject GetData()
	{
		SaveDataInventory s = new SaveDataInventory(items);
		return new JObject(s);//  JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings);
	}

	public void SetData(JObject data)
	{
		SaveDataInventory s = data.ToObject<SaveDataInventory>();// JsonConvert.DeserializeObject<SaveDataInventory>(data);
		//TODO: warning, sceneindex not considered here
		this.items = s.items;
	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "Inventory";
	}

	public static void TransferAllItems(Inventory from, Inventory to)
	{
		int failCount = 0;
		for(int i = 0; i < from.items.Count; i++)
		{
			if (from.items[i].IsEmpty()) continue;

			bool succeed = TransferItem(from, to, i);
			if (!succeed) failCount++;
		}

		print("transfered all items with " + failCount + " failed out of " + from.items.Count);
	}

	/// <summary>
	/// tries to transfer an item between inventories
	/// </summary>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <param name="fromIndex"></param>
	/// <returns></returns>
	public static bool TransferItem(Inventory from, Inventory to, int fromIndex)
	{
		List<Item> f = from.items;
		List<Item> t = to.items;

		if (f[fromIndex].IsEmpty())
		{
			Debug.LogError("tried to move empty item");
			return false;
		}

		//check bounds
		if (fromIndex < 0 || fromIndex >= f.Count)
		{
			Debug.LogError("fromIndex out of bounds");
			return false;
		}
		//if ((toIndex < 0 && toIndex != -1) || toIndex >= t.Count)
		//{
		//	Debug.LogError("toIndex out of bounds");
		//	return false;
		//}

		//search for same id, then add the item stack to the destination item stack with same id
		int target = -1;
		for(int i = 0; i < t.Count; i++)
		{
			if(t[i].id == f[fromIndex].id)
			{
				target = i;
				break;
			}
		}
		//then add the item stack to the destination item stack with same id
		if (target != -1)
		{
			Item temp = t[target]; 
			temp.amount += f[fromIndex].amount;
			t[target] = temp;
			f[fromIndex] = new Item(0, 0, 0, 0);
			return true;
		}

		//couldn't find same id, find an empty slot instead
		for (int i = 0; i < t.Count; i++)
		{
			if (t[i].IsEmpty())
			{
				target = i;
				break;
			}
		}

		if (target != -1)
		{
			//copy item stack and delete original, now the stack has been moved to the empty spot
			t[target] = f[fromIndex];
			f[fromIndex] = new Item(0, 0, 0, 0);
			return true;
		}

		return false;//no slot with same id or empty
	}

}
