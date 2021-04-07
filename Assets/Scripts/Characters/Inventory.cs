using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bobStuff;
using UnityEngine.Events;
using Newtonsoft.Json;

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
    void Awake()
    {
		items = new List<Item>();
        for(int i = 0; i < size; i++)
		{
            items.Add(new Item());
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

	public string GetData()
	{
		SaveDataInventory s = new SaveDataInventory(items);
		return JsonConvert.SerializeObject(s, Formatting.Indented);
	}

	public void SetData(string data)
	{
		SaveDataInventory s = JsonConvert.DeserializeObject<SaveDataInventory>(data);
		//TODO: warning, sceneindex not considered here
		this.items = s.items;
	}
}
