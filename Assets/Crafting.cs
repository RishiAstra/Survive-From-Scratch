using bobStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: make recipie class or struct, check if for craftable things, remove items when crafted

public class Crafting : MonoBehaviour
{
    public Inventory craftInventory;
    public Inventory craftResult;
    private gameControll me;
    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<gameControll>();

        craftInventory.take = true;
        craftInventory.put = true;

        craftResult.invChange.AddListener(OnItemCraft);
        craftResult.take = true;
        craftResult.put = false;
    }

    public void OnItemCraft(int itemIndex)
	{
        Item item = craftResult.items[itemIndex];
        //TODO: remove the crafting ingredients
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
