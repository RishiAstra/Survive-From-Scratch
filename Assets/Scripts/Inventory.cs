using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bobStuff;

public class Inventory : MonoBehaviour
{
    public int size;
	public List<Item> items;
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
