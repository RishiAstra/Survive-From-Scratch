using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//TODO: FIX RECTTRANSFORM GET TRUE SCALED SIZE
//TODO: this class will automatically make inventory slots on the ui canvas and fill them to target inventory
public class InventoryUI : MonoBehaviour
{
    public Inventory target;
    public GameObject slotPrefab;
    public float preferedSize;
    public float defaultSize;//used to scale
    public RectTransform slotBounds;
    public int w, h;

    private List<RectTransform> slotT;
    private List<ItemIcon> slotI;

    private float slotSize;
    private int pSize = 0;
    private float size;
    private float initialHeight;
    private float maxSlotSizeX;
    private float maxSlotSizeY;
    // Start is called before the first frame update
    void Awake()
    {
        slotT = new List<RectTransform>();
        slotI = new List<ItemIcon>();
        initialHeight = slotBounds.rect.height;
        InitializeSlots();
    }
    void InitializeSlots()
	{
        //delete old 
        for(int i = slotBounds.childCount - 1; i >= 0; i--)
		{
            Destroy(slotBounds.GetChild(i).gameObject);
		}

        slotT = new List<RectTransform>();
        slotI = new List<ItemIcon>();
        
        //make new slots
        maxSlotSizeX = slotBounds.rect.size.x / w;
        maxSlotSizeY = slotBounds.rect.size.y / h;
        size = Mathf.Min(maxSlotSizeX, maxSlotSizeY, preferedSize);
        //      for(int i = 0; i < target.items.Count; i++)
        //{
        //          GameObject g = Instantiate(slotPrefab, slotBounds);
        //          //g.transform.SetParent(slotBounds);
        //          //g.GetComponent<Transform>().SetParent(slotBounds);
        //          slotT.Add(g.GetComponent<RectTransform>());
        //          slotI.Add(g.GetComponent<ItemIcon>());
        //          int x = i % w;
        //          int y = Mathf.FloorToInt(i / w);
        //          slotT[i].anchoredPosition = new Vector2(size * (x - (w-1)/2f), size * (y - (h-1)/2f));
        //          slotT[i].localScale = new Vector3(size, size, size) / defaultSize;
        //	slotI[i].parent = target;
        //	slotI[i].index = i;
        //}
        CorrectSlotCount();
    }

    void CorrectSlotCount()
	{
        if(slotT.Count > target.items.Count)
		{
            //delete excess slots
            for(int i = slotT.Count - 1; i > target.items.Count - 1; i--)
			{
                Destroy(slotT[i].gameObject);
                slotT.RemoveAt(i);
                slotI.RemoveAt(i);
			}
		}
        else if (slotT.Count < target.items.Count)
		{
            for (int i = slotT.Count; i < target.items.Count; i++)
            {
                GameObject g = Instantiate(slotPrefab, slotBounds);
                //g.transform.SetParent(slotBounds);
                //g.GetComponent<Transform>().SetParent(slotBounds);
                slotT.Add(g.GetComponent<RectTransform>());
                slotI.Add(g.GetComponent<ItemIcon>());
                int x = i % w;
                int y = Mathf.FloorToInt(i / w);
                if(y * size > slotBounds.rect.height)
				{
                    slotBounds.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y * size);
				}else if( y * size < slotBounds.rect.width)
				{
                    slotBounds.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialHeight);
                }
                slotT[i].anchoredPosition = new Vector2(size * (x - (w - 1) / 2f), size * (y - (h - 1) / 2f));
                slotT[i].localScale = new Vector3(size, size, size) / defaultSize;
                slotI[i].parent = target;
                slotI[i].index = i;
            }
        }
	}

    // Update is called once per frame
    void Update()
    {
        if(target.items.Count != pSize)
		{
            CorrectSlotCount();
            pSize = target.items.Count;
		}
    }

    public void Refresh()
	{
        throw new System.NotImplementedException();
	}	
}
