﻿using bobStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: consider making Item a struct
//TODO: make recipie class or struct, check if for craftable things, remove items when crafted

[System.Flags]
[System.Serializable]
public enum CraftType
{
    none = 1 << 0,
    tools = 1 << 1
}

//TODO: WARNING: recipie cannot have 2 items of the same type
[System.Serializable]
public struct Recipie
{
    public List<Item> ingredients;
    public Item result;
    public CraftType craftType;
}

public class Crafting : MonoBehaviour
{
    public Inventory craftInventory;
    public Inventory craftResult;

    public List<Recipie> recipies;

    private gameControll me;
    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<gameControll>();

        craftInventory.invChange.AddListener(OnCraftInventoryChanged);
        craftInventory.take = true;
        craftInventory.put = true;

        craftResult.invChange.AddListener(OnItemCraft);
        craftResult.take = true;
        craftResult.put = false;
    }

    public void OnCraftInventoryChanged(int itemIndex)
	{
        RefreshCraftableRecipies();
	}

    public void OnItemCraft(int itemIndex)
	{
        Item item = craftResult.items[itemIndex];
        //TODO: remove the crafting ingredients
	}

    public void RefreshCraftableRecipies()
	{
        craftResult.items = new List<Item>();
        //go through all recipies
        for (int i = 0; i < recipies.Count; i++)
        {
            //go through the required ingredients
            for (int j = 0; j < recipies[i].ingredients.Count; j++)
            {
                int count = 0;
                //go through the craft inventory to find the ingredients
                for (int k = 0; k < craftInventory.items.Count; k++)
                {
                    if(craftInventory.items[k].id == recipies[i].ingredients[j].id)
					{
                        count += craftInventory.items[k].amount;

                    }
                }

                //if too little of this ingredient, cannot craft this, break (move on to the next recipie in the i recipies.Count forloop)
                if(count < recipies[i].ingredients[j].amount)
				{
                    break;
				}
            }

            //if it survived till here, there are enough materials to craft the recipie
            craftResult.items.Add(recipies[i].result);
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (gameControll.main.craftInventory.activeSelf)
		{
            
            RefreshCraftableRecipies();
		}
		else
		{

		}
    }
}
