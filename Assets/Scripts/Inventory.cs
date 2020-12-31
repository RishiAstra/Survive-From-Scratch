using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bobStuff;

public class Inventory : MonoBehaviour
{
	public List<Item> items;

    private InventoryUI ui;
    // Start is called before the first frame update
    void Start()
    {
		items = new List<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshUI()
	{
        if(ui != null && ui.gameObject.activeInHierarchy)
		{
            ui.Refresh();
		}
	}
}
