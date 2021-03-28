using bobStuff;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public static string recipiesPath = Application.streamingAssetsPath + @"/Items/recipies.json";//@"Assets\Resources\item types.json";


    public static Crafting main;

    public Inventory craftInventory;
    public Inventory craftResult;
	public InventoryUI craftInventoryUI;
	public InventoryUI craftResultUI;

    public List<Recipie> recipies;

    public GameControl me;
    // Start is called before the first frame update
    public void Start()
    {
        if (main != null) Debug.LogError("Two Crafting");
        main = this;
        //me = GetComponent<GameControl>();
        if (GameControl.main == null) GameControl.main = me;

        craftInventory.invChange.AddListener(OnCraftInventoryChanged);
        craftInventory.take = true;
        craftInventory.put = true;

        craftResult.invChange.AddListener(OnItemCraft);
        craftResult.take = true;
        craftResult.put = false;
        OnCraftInventoryChanged(0);
    }

    public void OnCraftInventoryChanged(int itemIndex)
	{
        RefreshCraftableRecipies();
	}

    public void OnItemCraft(int itemIndex)
	{
        Item item = craftResult.items[itemIndex];//TODO: unused
        //TODO: remove the crafting ingredients
        RemoveCraftingIngredients(itemIndex);
	}

    public void RefreshCraftableRecipies()
	{
        craftResult.items = new List<Item>();
        //go through all recipies
        for (int i = 0; i < recipies.Count; i++)
        {
            bool canMakeThis = true;
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
                    canMakeThis = false;
                    break;
				}
            }

            if (canMakeThis) craftResult.items.Add(recipies[i].result);
        }
    }

    public void RemoveCraftingIngredients(int index)
	{
        //go through the required ingredients
        for (int j = 0; j < recipies[index].ingredients.Count; j++)
        {
            int required = recipies[index].ingredients[j].amount;
            //go through the craft inventory to find the ingredients
            for (int k = 0; k < craftInventory.items.Count; k++)
            {
                if (craftInventory.items[k].id == recipies[index].ingredients[j].id)
                {
                    if(required >= craftInventory.items[k].amount)
					{
                        //remove all of this item, because even all of it isn't enough
                        Item temp = craftInventory.items[k];
                        required -= temp.amount;//remove the ingredient used
                        temp.amount = 0;
                        temp.id = 0;
                        craftInventory.items[k] = temp;
					}
					else
					{
                        //remove as much as needed, some of this item will be left
                        Item temp = craftInventory.items[k];
                        temp.amount -= required;
                        required = 0;
                        craftInventory.items[k] = temp;
                        break;//we are done fulfilling this ingredient spending requirement for the recipie
                    }
                }                
            }
            //TODO: make this better
            //error if we still need to take more ingredient, but there is none left to take. This means that the crafting was illegitimate
            if (required > 0) Debug.LogError("Not enough of ingredient " + "in crafting recipie");
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (GameControl.main.craftInventory.gameObject.activeSelf)
		{
            
            RefreshCraftableRecipies();
		}
		else
		{

		}
    }

	internal void InitializeUI()
	{
		craftInventoryUI.InitializeSlots();
		craftResultUI.InitializeSlots();
	}

    public void CheckRecipies()
    {
        if (recipies == null)
		{
			LoadRecipies();
		}
	}

	public void LoadRecipies()
	{
		if (File.Exists(recipiesPath))
		{
			Debug.Log("read ItemTypes");
			recipies = JsonConvert.DeserializeObject<Recipie[]>(File.ReadAllText(recipiesPath)).ToList();
		}
		else
		{
			recipies = new List<Recipie>();
			recipies.Add(new Recipie());
			Debug.LogError("No recipie list, made a new one");
		}
	}

	public void SaveRecipies()
    {
        File.WriteAllText(recipiesPath, JsonConvert.SerializeObject(recipies.ToArray(), Formatting.Indented));
        Debug.Log("Saved Recipies");
    }

}
