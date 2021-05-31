/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShopMultiplierButton : MonoBehaviour
{
    public int mult;
    public GameObject selectedTint;

    private TextMeshProUGUI text;
    private Button b;
    public bool selected;
    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        b.onClick.AddListener(WhatToDoOnClick);
        text.text = "x" + mult;
    }

    void WhatToDoOnClick()
	{
        ShopControl.main.SetBuyMult(mult);
    }

    void SetSelected(bool s)
	{
        selected = s;
        selectedTint.SetActive(s);
	}

    // Update is called once per frame
    void Update()
    {
        //if the shop is using this buy quantity, then this button is selected
        if(ShopControl.main.mult == mult)
		{
            //refresh only if neccesary
			if (!selected)
			{
                SetSelected(true);
			}
		}
		else
		{
            //refresh only if neccesary
            if (selected)
            {
                SetSelected(false);
            }
        }
    }
}
