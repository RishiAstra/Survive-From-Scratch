using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//TODO: this class will automatically make inventory slots on the ui canvas and fill them to target inventory
public class InventoryUI : MonoBehaviour
{
    public Inventory target;
    public GameObject slotPrefab;
    public float preferedSize;
    public float defaultSize;//used to scale
    public RectTransform slotBounds;
    //public Transform slotParent;
    public int w, h;
    public bool overflowDown;

    public List<RectTransform> slotT;
    private List<ItemIcon> slotI;

    private float slotSize;
    private int pSize = -1;
    private float size;
    private float initialHeight;
    private float maxSlotSizeX;
    private float maxSlotSizeY;
    private float initialYPadding;//space around the top and bottom initially, used for overflowDown auto readjusting height of scrollable
    private Vector2 initialSizeDeta;
    private bool initialized;
    // Start is called before the first frame update
    void Start()
    {
        
        //InitializeSlots();
    }
    public void InitializeSlots()
	{
        //delete old 
        for(int i = slotBounds.childCount - 1; i >= 0; i--)
		{
            Destroy(slotBounds.GetChild(i).gameObject);
		}

        slotT = new List<RectTransform>();
        slotI = new List<ItemIcon>();

		//Vector2 min = slotBounds.anchorMin;
		//min.x *= Screen.width / gameControll.main.mainCanvas.scaleFactor;
		//min.y *= Screen.height / gameControll.main.mainCanvas.scaleFactor;

		//min += slotBounds.offsetMin;

		//Vector2 max = slotBounds.anchorMax;
		//max.x *= Screen.width / gameControll.main.mainCanvas.scaleFactor;
		//max.y *= Screen.height / gameControll.main.mainCanvas.scaleFactor;

		//max += slotBounds.offsetMax;

		//Vector2 diff = max - min;
		//make new slots
		//print(slotBounds.rect.ToString());
        maxSlotSizeX = slotBounds.rect.width / w;
        maxSlotSizeY = overflowDown ? float.MaxValue : slotBounds.rect.height / h;//if can overflow down, height won't limit it
        size = Mathf.Min(maxSlotSizeX, maxSlotSizeY, preferedSize);
        initialYPadding = slotBounds.rect.height - h * size;
        initialSizeDeta = slotBounds.sizeDelta;
        //print(slotBounds.sizeDelta);
        initialHeight = slotBounds.rect.height;
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
        initialized = true;
        CorrectSlotCount();
    }

    void CorrectSlotCount()
	{
		maxSlotSizeX = slotBounds.rect.width / w;
		maxSlotSizeY = overflowDown ? float.MaxValue : slotBounds.rect.height / h;//if can overflow down, height won't limit it
		size = Mathf.Min(maxSlotSizeX, maxSlotSizeY, preferedSize);
  //      if(overflowDown && size * h > slotBounds.rect.height)
		//{
  //          slotBounds.sizeDelta += new Vector2(0, size * h - slotBounds.rect.height);
		//}
		//adjust the height if needed
		int yRequired = Mathf.CeilToInt(((float)target.items.Count) / (float)w);
        float heightRequired = yRequired * size + initialYPadding;
		//if(overflowDown)print(heightRequired);
		//if (heightRequired > slotBounds.rect.height)
		//{
		//print(slotBounds.sizeDelta + "|" + initialSizeDeta);
		slotBounds.sizeDelta = initialSizeDeta + new Vector2(0, heightRequired - initialHeight);
		//slotBounds.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightRequired);
		//}
		//else
		//{
		//          slotBounds.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialHeight);
		//      }


		if (slotT.Count > target.items.Count)
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

                slotT[i].anchorMin = new Vector2(0.5f, 1f);
                slotT[i].anchorMax = slotT[i].anchorMin;
                slotT[i].anchoredPosition = new Vector2(size * (x - (w - 1) / 2f), -size * y - initialYPadding / 2f - size / 2f);// new Vector2(size * (x - (w - 1) / 2f), -size * (y - (h - 1) / 2f));
                slotT[i].localScale = new Vector3(size, size, size) / defaultSize;
                slotI[i].parent = target;
                slotI[i].index = i;
            }
        }
	}

    // Update is called once per frame
    void Update()
    {
        if(initialized && target != null && target.items.Count != pSize)
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
