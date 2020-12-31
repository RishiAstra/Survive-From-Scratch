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
    public RectTransform slotBounds;
    public int w, h;

    private List<RectTransform> slotT;
    private List<ItemIcon> slotI;

    private float slotSize;
    private int pSize = 0;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: this might cause problems
        target = Player.main.GetComponent<Inventory>();//TODO: TEMPORAIRY
    }
    void RefreshSlots()
	{
        //delete old 
        for(int i = slotBounds.childCount - 1; i >= 0; i--)
		{
            Destroy(slotBounds.GetChild(i).gameObject);
		}

        slotT = new List<RectTransform>();
        slotI = new List<ItemIcon>();
        
        //make new slots
        float maxSlotSizeX = slotBounds.sizeDelta.x / w;
        float maxSlotSizeY = slotBounds.sizeDelta.y / h;
        print(maxSlotSizeY);
        print(maxSlotSizeY);
        print(preferedSize);
        float size = Mathf.Min(maxSlotSizeX, maxSlotSizeY, preferedSize);
        for(int i = 0; i < target.items.Count; i++)
		{
            GameObject g = Instantiate(slotPrefab, slotBounds);
            //g.transform.SetParent(slotBounds);
            //g.GetComponent<Transform>().SetParent(slotBounds);
            slotT.Add(g.GetComponent<RectTransform>());
            slotI.Add(g.GetComponent<ItemIcon>());

            slotT[i].localPosition = new Vector2(size * (i - target.items.Count / 2f), 0);
            slotT[i].localScale = new Vector3(size, size, size);
			slotI[i].parent = target;
			slotI[i].index = i;
		}
    }

    // Update is called once per frame
    void Update()
    {
        if(target.items.Count != pSize)
		{
            RefreshSlots();
            pSize = target.items.Count;
		}
    }

    public void Refresh()
	{
        throw new System.NotImplementedException();
	}	
}
