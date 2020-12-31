using bobStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Item held;
    public static ItemIcon heldFrom;

    public Inventory parent;
    public int index;

    public Image img;
    public TextMeshProUGUI amountText;
    //TODO: show strength/durability remaining

    private bool mouseOver;
    private bool selected;
    // Start is called before the first frame update
    void Start()
    {
    }

    void UpdateIcon()
	{
        img.sprite = Player.itemTypes[parent.items[index].id].icon;
        amountText.text = parent.items[index].amount.ToString();
	}

    //TODO: call this when the inventoryUI is closed if the held item is in that inventory
    public static void CancelMove()
	{
        if(held != null)
		{
            if(held.id == heldFrom.parent.items[heldFrom.index].id)
			{
                heldFrom.parent.items[heldFrom.index].amount += held.amount;
                held = null;
                Debug.Log("Canceled moving item, added back");
            }else if (heldFrom.parent.items[heldFrom.index].id == 0)
			{
                Debug.Log("Canceled moving item, added back to empty slot");
			}
			else
			{
                //TODO: destroying items is very bad, should be fixed
                Debug.LogError("Canceled moving item, but the original slot was the wrong id. The item is destroyed instead");
                held = null;
			}
		}
		else
		{
            Debug.Log("Canceled moving item nothing");
		}
	}

    // Update is called once per frame
    void Update()
    {
        UpdateIcon();//TODO: only use this when needed
        if(mouseOver && Input.GetMouseButtonUp(0))
		{
            if(held == null)
			{
                held = parent.items[index];
                parent.items[index] = new Item();
                heldFrom = this;
                UpdateIcon();
			}
			else
			{
                //add the stacks of items (add the 2 items' amounts to make 1 item)
                //TODO: WARNING overflow possible
                //TODO: WARNING any swap with same id is allowed
                if(held.id == parent.items[index].id)
				{
                    parent.items[index].amount += held.amount;
                    held = null;
                    heldFrom = null;
                    UpdateIcon();
                }
				else
				{
                    //swap
                    Item temp = held;
                    held = parent.items[index];
                    parent.items[index] = temp;
                    UpdateIcon();
                }
			}
		}
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }
}
