using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//TODO: this class will automatically make inventory slots on the ui canvas and fill them to target inventory
public class InventoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject slotPrefab;
    public float preferedSize;
    public RectTransform slotBounds;
    public int num;

    private List<RectTransform> slotT;
    private List<Image> slotI;

    private float slotSize;
    // Start is called before the first frame update
    void Start()
    {
        float maxSlotSizeX = slotBounds.sizeDelta.x;
        float maxSlotSizeY = slotBounds.sizeDelta.y;
        for(int i = 0; i < num; i++)
		{
            GameObject g = Instantiate(slotPrefab);
            slotT.Add(g.GetComponent<RectTransform>());
            slotI.Add(g.GetComponent<Image>());

            slotT[i].sizeDelta = new Vector2();
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

	public void OnPointerEnter(PointerEventData eventData)
	{
		throw new System.NotImplementedException();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		throw new System.NotImplementedException();
	}
}
