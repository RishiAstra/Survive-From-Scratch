/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
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
    public TextMeshProUGUI itemModifiersText;
    public Image itemIcon;
    public RectTransform rt;
    // Start is called before the first frame update
    void Awake()
    {
        main = this;//TODO:warning: race condition vs gamecontrol
        //rt = GetComponent<RectTransform>();
        //if (gameObject.activeInHierarchy) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    public void SetInfo(Item i)
	{
        ItemType t = GameControl.itemTypes[i.id];
        itemNameText.text = t.name;
        itemDescriptionText.text = t.description;
        itemIcon.sprite = t.icon;
        itemModifiersText.text = t.mods.ToString(0) + "\n" + t.consumeRestore.ToString();

		//immediately update the size etc of this
		LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
	}

	private void OnEnable()
	{
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

	// Update is called once per frame
	void Update()
    {
        
    }
}
