using bobStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
//TODO: perhaps have each tag have a int as well, e.g. tag Axe, 10 means this is an axe that is 10 good at being an axe?
//TODO: check if has permission to place or remove before doing it
public class ItemIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Item held;
    public static ItemIcon heldFrom;

    public Inventory parent;
    public int index;
	[Tooltip("the gameobject to activate when selected")]
	public GameObject selectedGameObject;

    public Image img;
    public TextMeshProUGUI amountText;
    //TODO: show strength/durability remaining

    private bool mouseOver;
    public bool selected;
    [Tooltip("The recttransform of this ItemIcon, defaults to attached")]
    public RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        if(rt == null) rt = GetComponent<RectTransform>();
        UpdateIcon();
    }

    public void UpdateIcon()
    {
        if (index >= parent.items.Count || parent.items[index].id == 0 || parent.items[index].amount == 0)
		{
            img.color = Color.clear;
            amountText.text = "";
			selectedGameObject.SetActive(false);
		}
		else
		{
            img.color = Color.white;
            int id = parent.items[index].id;
            img.sprite = (id >= 0 && id < GameControl.itemTypes.Count) ? GameControl.itemTypes[id].icon : GameControl.itemTypes[0].icon;
            amountText.text = parent.items[index].amount.ToString();
			selectedGameObject.SetActive(selected);
        }        
	}

    //TODO: call this when the inventoryUI is closed if the held item is in that inventory
    public static void CancelMove()
	{
  //      if(held != null)
		//{
        if(held.id == heldFrom.parent.items[heldFrom.index].id)
		{
            Item tempItem = heldFrom.parent.items[heldFrom.index];
            tempItem.amount += held.amount;
            heldFrom.parent.items[heldFrom.index] = tempItem;
            held = new Item();
            Debug.Log("Canceled moving item, added back");
        }else if (heldFrom.parent.items[heldFrom.index].id == 0)
		{
            Debug.Log("Canceled moving item, added back to empty slot");
		}
		else
		{
            //TODO: destroying items is very bad, should be fixed
            Debug.LogError("Canceled moving item, but the original slot was the wrong id. The item is destroyed instead");
            held = new Item();
		}
		//}
		//else
		//{
  //          Debug.Log("Canceled moving item nothing");
		//}
	}

	private void OnDisable()
	{
        mouseOver = false;
	}

	// Update is called once per frame
	void Update()
    {
        UpdateIcon();//TODO: only use this when needed
        if(mouseOver && Input.GetMouseButtonDown(0))
		{
            parent.invClicked.Invoke(index);

            //you have nothing, want to grab
            if(held.id == 0)
			{
				if (parent.take)
				{
                    
                    held = parent.items[index];
                    parent.items[index] = new Item();
                    heldFrom = this;
                    parent.invChange.Invoke(index);
                    UpdateIcon();
                }                
			}
			else
			{
                //you have something, want to take or add
                //add the stacks of items (add the 2 items' amounts to make 1 item)
                //TODO: WARNING integer overflow (unlikely) or undesirably large "stacks" possible
                //TODO: WARNING any swap with same id is allowed
                //TODO: consider not allowing crafting item to be taken unless enough resources, if not, return resources
                if(held.id == parent.items[index].id)
				{
                    //if same id, try to place that item
					if (parent.put)
					{
                        Item tempItem = parent.items[index];
                        tempItem.amount += held.amount;
                        parent.items[index] = tempItem;
                        held = new Item();
                        heldFrom = null;
                        parent.invChange.Invoke(index);
                        UpdateIcon();
                    }else if (parent.take)//otherwise, try to take that item and add it to held
					{
                        Item tempItem = held;// parent.items[index];
                        tempItem.amount += parent.items[index].amount;// held.amount;
                        held = tempItem;
                        //parent.items[index] = tempItem;
                        parent.items[index] = new Item();
                        //heldFrom = null;
                        parent.invChange.Invoke(index);
                        UpdateIcon();
                    }                
                }
				else//this also handles putting items in an empty inventory slot
				{
                    //you have something different, want to swap
                    //you can swap if both put and take, or if put and you're swapping with empty slot
					if (parent.put && (parent.take || parent.items[index].id == 0))
					{
                        //swap
                        Item temp = held;
                        held = parent.items[index];
                        parent.items[index] = temp;
                        parent.invChange.Invoke(index);
                        UpdateIcon();
					}
                    
                }
			}
		}


        //right click in inventory

        if (mouseOver && Input.GetMouseButtonDown(1))
        {
            parent.invClicked.Invoke(index);

            //you have nothing, want to grab half
            //TODO: checking id might not be enough, held.amount might become 0
            if (held.id == 0)
            {
                if (parent.take)
                {
                    int toTake = Mathf.CeilToInt(parent.items[index].amount / 2.0f);
                    Item parentItem = parent.items[index];
                    parentItem.amount -= toTake;

                    held = parent.items[index];
                    held.amount = toTake;

                    //held = parent.items[index];
                    parent.items[index] = parentItem;// new Item();
					heldFrom = this;
					parent.invChange.Invoke(index);
                    UpdateIcon();
                }
            }
            else
            {
                //you have something, want to take or add
                //add the stacks of items (add the 2 items' amounts to make 1 item)
                //TODO: WARNING integer overflow (unlikely) or undesirably large "stacks" possible
                //TODO: WARNING any swap with same id is allowed
                //TODO: consider not allowing crafting item to be taken unless enough resources, if not, return resources
                if (held.id == parent.items[index].id)
                {
                    //if same id, try to place that item
                    if (parent.put)
                    {
                        Item tempItem = parent.items[index];
                        tempItem.amount += 1;// held.amount;
                        held.amount -= 1;
                        if (held.amount <= 0)
						{
							held = new Item();
                            heldFrom = null;
                        }

                        parent.items[index] = tempItem;
                        //held = new Item();
                        parent.invChange.Invoke(index);
                        UpdateIcon();
                    }
                    //else if (parent.take)//otherwise, try to take that item and add it to held
                    //{
                    //    Item tempItem = held;// parent.items[index];
                    //    tempItem.amount += parent.items[index].amount;// held.amount;
                    //    held = tempItem;
                    //    //parent.items[index] = tempItem;
                    //    parent.items[index] = new Item();
                    //    //heldFrom = null;
                    //    parent.invChange.Invoke(index);
                    //    UpdateIcon();
                    //}
                }
                else//this also handles putting items in an empty inventory slot
                {
                    //you have something different, want to swap
                    //you can swap if both put and take, or if put and you're swapping with empty slot
                    if (parent.put && parent.items[index].id == 0)
                    {

                        //Item tempItem = new Item(held.id, 1, 0, 0);
                        //tempItem.amount += 1;// held.amount;
                        held.amount -= 1;
                        if (held.amount <= 0)
                        {
                            held = new Item();
                            heldFrom = null;
                        }
                        parent.items[index] = new Item(held.id, 1, 0, 0);
                        //held = new Item();
                        parent.invChange.Invoke(index);
                        UpdateIcon();
                        

                        //swap
                        //Item temp = held;
                        //held = parent.items[index];
                        //parent.items[index] = temp;
                        //parent.invChange.Invoke(index);
                        //UpdateIcon();
                    }

                }
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;

        if(parent.items[index].id != 0 && parent.items[index].amount > 0)
		{
            GameControl.main.ShowInfo(parent.items[index], rt);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        GameControl.main.HideInfo(rt);
    }
}
