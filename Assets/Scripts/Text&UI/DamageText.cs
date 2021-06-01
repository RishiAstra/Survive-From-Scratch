/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public AnimationCurve occupacity;
    public float riseSpeed;
    public float lifeTime;
    public TextMeshPro text;
    public TextMeshProUGUI textUI;

    private Stopwatch sw;
    // Start is called before the first frame update
    //need awake to initialize it before sw is used
    void Awake()
    {
        sw = new Stopwatch();
    }

	public void ResetText(float damageAmount)
	{
        sw.Restart();
        string toDisplay;
        //use 2 decimal places for 1 digit numbers, 1 for 2 digit numbers, otherwise no decimal
		if (damageAmount < 10f)
		{
            toDisplay = damageAmount.ToString("F2");
		}else if (damageAmount < 100f)
		{
            toDisplay = damageAmount.ToString("F1");
        }
		else
		{
            toDisplay = damageAmount.ToString("F0");
        }
        if (text != null) text.text = toDisplay;
        if (textUI != null) textUI.text = toDisplay;
    }

	// Update is called once per frame
	void Update()
    {
        float timepassed = sw.ElapsedMilliseconds / 1000f;
		if (timepassed > lifeTime)
		{
            gameObject.SetActive(false);
            return;
		}

        if(text != null)
		{
            Color color = text.color;
		    color.a = occupacity.Evaluate(timepassed / lifeTime);
            text.color = color;
		}

        if(textUI != null)
		{
            Color color = textUI.color;
            color.a = occupacity.Evaluate(timepassed / lifeTime);
            textUI.color = color;
        }		

        transform.Translate(0, riseSpeed * Time.deltaTime, 0);
    }
}
