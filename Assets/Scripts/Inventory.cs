using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bobStuff;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public bool take;//permissions for if the current player can take and place items in this inventory
    public bool put;//TODO: permissions might be a list of players who can access
    public int size;
	public List<Item> items;

    public UnityEvent<int> invChange;//called when the inventory is changed (trigger this by script, manual inspector change won't trigger this)
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

 //   void RefreshUI()
	//{
 //       if(ui != null && ui.gameObject.activeInHierarchy)
	//	{
 //           ui.Refresh();
	//	}
	//}
}
