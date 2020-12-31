﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeldItem : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI amountText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ItemIcon.held != null)
        {
            img.color = Color.white;
            img.sprite = Player.itemTypes[ItemIcon.held.id].icon;
            amountText.text = ItemIcon.held.amount.ToString();
            transform.position = Input.mousePosition;
        }
		else
        {
            img.color = Color.clear;
		}
        
    }
}
