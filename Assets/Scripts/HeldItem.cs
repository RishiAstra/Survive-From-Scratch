using System.Collections;
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
        if (ItemIcon.held.id != 0)
        {
            img.color = Color.white;
            img.sprite = gameControll.itemTypes[ItemIcon.held.id].icon;
			amountText.enabled = true;
			amountText.text = ItemIcon.held.amount > 1 ? ItemIcon.held.amount.ToString() : "";
            transform.position = Input.mousePosition;
        }
		else
        {
            img.color = Color.clear;
			amountText.enabled = false;
			ItemIcon.held.amount = 0;
		}
        
    }
}
