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
    public int num;

    private List<RectTransform> slotT;
    private List<ItemIcon> slotI;

    private float slotSize;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: this might cause problems
        target = Player.main.GetComponent<Inventory>();//TODO: TEMPORAIRY
        float maxSlotSizeX = slotBounds.sizeDelta.x;
        float maxSlotSizeY = slotBounds.sizeDelta.y;
        float size = Mathf.Min(maxSlotSizeX, maxSlotSizeX, preferedSize);
        for(int i = 0; i < target.items.Count; i++)
		{
            GameObject g = Instantiate(slotPrefab);
            slotT.Add(g.GetComponent<RectTransform>());
            slotI.Add(g.GetComponent<ItemIcon>());
            

            slotT[i].localScale = new Vector3(size, size, size);
            slotI[i].parent = target;
            slotI[i].index = i;
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh()
	{
        throw new System.NotImplementedException();
	}	
}
