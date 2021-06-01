/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextControl : MonoBehaviour
{
    public static DamageTextControl main;
    public static List<GameObject> texts;
    public static List<DamageText> damageTexts;
    public static List<GameObject> textsReady;
    public static List<DamageText> damageTextsReady;

    public GameObject damageTextPrefab;
    public GameObject damageTextUIPrefab;
    public Transform damateTextUIPosition;
    public Image receiveDamageTint;
    public Gradient receiveDamageTintGradient;
    public float receiveDamageTintTime;

    private float receiveDamageTintTimeLeft;


    // Start is called before the first frame update
    void Start()
    {
        
        if (main != null)
		{
			Debug.LogError("Two DamageTextControl");
            DestroyImmediate(gameObject);
            return;
		}
		main = this;

        texts = new List<GameObject>();
        damageTexts = new List<DamageText>();
        textsReady = new List<GameObject>();
        damageTextsReady = new List<DamageText>();
    }

    public static void PutDamageText(float damageAmount, bool onUI = true)
	{
        PutDamageText(main.damateTextUIPosition.position, damageAmount, onUI);
	}

    public static void PutDamageText(Vector3 position, float damageAmount, bool onUI = false)
	{
        GameObject g;
        DamageText d;

		if (onUI)
		{
            g = Instantiate(main.damageTextUIPrefab);
            d = g.GetComponent<DamageText>();
            if (d == null) Debug.LogError("damage text prefab must have DamageText component");
            g.transform.SetParent(GameControl.main.mainCanvas.transform);
            main.receiveDamageTintTimeLeft = main.receiveDamageTintTime;
        }
		else
		{
            if(textsReady.Count > 0)
		    {
                int index = textsReady.Count - 1;
                g = textsReady[index];
                d = damageTextsReady[index];
                textsReady.RemoveAt(index);
                damageTextsReady.RemoveAt(index);
		    }
		    else
		    {
                g = Instantiate(main.damageTextPrefab);
                d = g.GetComponent<DamageText>();
                if (d == null) Debug.LogError("damage text prefab must have DamageText component");
		    }
		}
        

        g.transform.position = position;
        d.ResetText(damageAmount);
	}

    // Update is called once per frame
    void Update()
    {
        receiveDamageTintTimeLeft -= Time.deltaTime;
        if (receiveDamageTintTimeLeft <= 0.001f)
		{
            receiveDamageTint.color = Color.clear;
		}
		else
		{
            receiveDamageTint.color = receiveDamageTintGradient.Evaluate(1 - (receiveDamageTintTimeLeft / receiveDamageTintTime));
		}
    }
}
