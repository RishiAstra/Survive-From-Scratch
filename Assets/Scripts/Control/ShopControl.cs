/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using bobStuff;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopControl : MonoBehaviour
{
	public static ShopControl main;

    public Inventory shopInventory;
    public Inventory sellInventory;

    public InventoryUI shopInventoryUI;
    public InventoryUI sellInventoryUI;

    public int buyDealSelected = -1;
    public TextMeshProUGUI sellPriceText;
    public TextMeshProUGUI buyPriceText;
	public GameObject doesNotBuyWarning;
	public ShopKeeper current;
    //public List<ShopItem> buyDeals;
    //public List<ShopItem> sellDeals;//NOTE: don't put 2 of the same item in here

	public int mult = 1;
    // Start is called before the first frame update
    void Start()
	{
		if (main != null) Debug.LogError("two ShopControls");
		main = this;
		shopInventory.invClicked.AddListener(TrySetBuy);
		//shopInventory.items = new List<Item>();
		//for (int i = 0; i < current.buyDeals.Count; i++)
		//{
		//	shopInventory.items.Add(current.buyDeals[i].item);
		//}
		sellInventory.invChange.AddListener(TrySetSell);
		sellInventory.items = new List<Item>();
		sellInventory.items.Add(new Item());

		//sellInventory.invChange.AddListener(TrySetSell);
		//sellInventory.items = new List<Item>();
		//sellInventory.items.Add(new Item());

		shopInventoryUI.InitializeSlots();
		sellInventoryUI.InitializeSlots();

		//UpdateShopInventoryUI();
		SetBuyMult(1);
	}

	public void SetBuyMult(int newMult)
	{
		mult = newMult;
		TrySetBuy(buyDealSelected);//update the buy text
		UpdateShopInventoryUI();
	}

	public void UpdateShopInventoryUI()
	{
		if (current == null) return;
		shopInventory.items = new List<Item>();
		for (int i = 0; i < current.buyDeals.Count; i++)
		{
			shopInventory.items.Add(current.buyDeals[i].item);
		}
		shopInventoryUI.CorrectSlotCount();
		//shopInventoryUI.Refresh();
		//shopInventoryUI.Correct();
		//sellInventoryUI.InitializeSlots();

		for (int i = 0; i < current.buyDeals.Count; i++)
		{
			ShopItem s = current.buyDeals[i];
			//not used
			//s.item.amount *= mult;
			//taken care of in Setup()
			//s.price *= mult;
			shopInventoryUI.slotT[i].GetComponent<ShopItemUI>().Setup(s, mult);
			shopInventoryUI.slotI[i].UpdateIcon();
		}
	}

	void TrySetSell(int index)
	{
		
		if (sellInventory.items[0].id == 0)
		{
			doesNotBuyWarning.SetActive(false);
			sellPriceText.text = "Sell for: --";
            return;
		}

		for (int i = 0; i < current.sellDeals.Count; i++)
		{
			if (current.sellDeals[i].item.id == sellInventory.items[0].id && sellInventory.items[0].amount > 0)
			{
				doesNotBuyWarning.SetActive(false);
				sellPriceText.text = "Sell for: " + GetSellPrice(i);
				//GameControl.main.money += sellDeals[i].price;
				//ItemIcon.held.amount -= 1;
				return;
			}
		}
		doesNotBuyWarning.SetActive(true);
		sellPriceText.text = "Sell for: --";//no sell deal, so can't sell
    }

	private int GetSellPrice(int i)
	{
		return GetPriceFromShopItem(current.sellDeals[i], sellInventory.items[0].amount);
	}

	public void SellCurrentItem()
	{
        if (sellInventory.items[0].id == 0) return;
        for (int i = 0; i < current.sellDeals.Count; i++)
        {
            if (current.sellDeals[i].item.id == sellInventory.items[0].id && sellInventory.items[0].amount > 0)
            {
                GameControl.main.money += GetSellPrice(i);// sellDeals[i].price;
                sellInventory.items[0] = new Item();
                sellPriceText.text = "Sell for: --";
                return;
            }
        }
    }

    void TrySetBuy(int index)
	{
        buyDealSelected = index;
        shopInventoryUI.SelectSlot(index);
        if (buyDealSelected >= 0 && buyDealSelected < current.buyDeals.Count)
		{
			buyPriceText.text = "Buy x" + mult + " for: " + GetPriceFromShopItem(current.buyDeals[index], mult);
		}
		else
		{
			buyPriceText.text = "Buy x" + mult + " for: --";
		}
		//      ShopItem buy = buyDeals[index];
		//      int cost = buy.price;
		//      if(GameControl.main.money >= buy.price)
		//{
		//          if (ItemIcon.held.id == 0)
		//          {
		//              ItemIcon.held = buy.item;
		//              GameControl.main.money -= buy.price;
		//          }

		//          if (ItemIcon.held.id == buy.item.id)
		//          {
		//              ItemIcon.held.amount += buy.item.amount;
		//              GameControl.main.money -= buy.price;
		//          }
		//      }

	}

    public void BuyCurrentItem()
	{
        if (buyDealSelected < 0 || buyDealSelected >= current.buyDeals.Count) return;

        ShopItem buy = current.buyDeals[buyDealSelected];
		//this is taken care of in GetPriceFromShopItem count
		//buy.priceMult *= mult;
		buy.item.amount *= mult;
        if (GameControl.main.money >= GetPriceFromShopItem(buy, mult))
        {
            //get it if u can, then if u succeeded, take the money
            if(GameControl.main.GetItem(buy.item.id, buy.item.amount))
			{
				GameControl.main.money -= GetPriceFromShopItem(buy, mult);
			}

			//if (ItemIcon.held.id == 0)
			//{
			//    ItemIcon.held = buy.item;
			//    GameControl.main.money -= buy.price;
			//}

			//if (ItemIcon.held.id == buy.item.id)
			//{
			//    ItemIcon.held.amount += buy.item.amount;
			//    GameControl.main.money -= buy.price;
			//}
		}
    }

	public static int GetPriceFromShopItem(ShopItem i, int count)
	{
		return Mathf.RoundToInt(GameControl.itemTypes[i.item.id].cost * i.priceMult * count);
	}
}

[System.Serializable]
public struct ShopItem
{
    public Item item;
	public float priceMult;
}
