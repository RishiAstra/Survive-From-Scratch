using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using bobStuff;

public class ItemInfoUI : MonoBehaviour
{
    public static ItemInfoUI main;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemIcon;
    // Start is called before the first frame update
    void Awake()
    {
        main = this;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void SetInfo(Item i)
	{
        ItemType t = GameControl.itemTypes[i.id];
        itemNameText.text = t.name;
        itemIcon.sprite = t.icon;

        //immediately update the size etc of this
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
