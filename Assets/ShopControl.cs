using bobStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopControl : MonoBehaviour
{
    public Inventory shopInventory;
    public Inventory sellInventory;

    public InventoryUI shopInventoryUI;
    public InventoryUI sellInventoryUI;

    public List<ShopItem> buyDeals;
    public List<ShopItem> sellDeals;//NOTE: don't put 2 of the same item in here

    // Start is called before the first frame update
    void Start()
    {
        shopInventory.invClicked.AddListener(TryBuy);
        shopInventory.items = new List<Item>();
        for(int i = 0; i < buyDeals.Count; i++)
		{
            shopInventory.items.Add(buyDeals[i].item);
		}
        sellInventory.invChange.AddListener(TrySell);
        sellInventory.items = new List<Item>();
        sellInventory.items.Add(new Item());

        shopInventoryUI.InitializeSlots();
        sellInventoryUI.InitializeSlots();
    }

    void TrySell(int index)
	{
        if (sellInventory.items[0].id == 0) return;
        for (int i = 0; i < sellDeals.Count; i++)
        {
			if (sellDeals[i].item.id == sellInventory.items[0].id && sellInventory.items[0].amount > 0)
			{
                GameControl.main.money += sellDeals[i].price;
                ItemIcon.held.amount -= 1;
                return;
			}
        }
    }

    void TryBuy(int index)
	{
        ShopItem buy = buyDeals[index];
        int cost = buy.price;
        if(GameControl.main.money >= buy.price)
		{
            if (ItemIcon.held.id == 0)
            {
                ItemIcon.held = buy.item;
                GameControl.main.money -= buy.price;
            }

            if (ItemIcon.held.id == buy.item.id)
            {
                ItemIcon.held.amount += buy.item.amount;
            }
        }
        
	}
}

[System.Serializable]
public struct ShopItem
{
    public Item item;
    public int price;
}
