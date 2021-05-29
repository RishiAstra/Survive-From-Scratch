/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrollable : MonoBehaviour
{
    private RectTransform rt;//my recttransform
    private RectTransform prt;//parent's
    private Vector2 initialAnchorPos;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        prt = transform.parent.GetComponent<RectTransform>();
        initialAnchorPos = rt.anchoredPosition;
    }

    public void OnValueChanged(float value)
	{
        float deltaHeight = rt.rect.height - prt.rect.height;
        if (deltaHeight < 0) return;
        float ypos = deltaHeight * value;
        rt.anchoredPosition = initialAnchorPos + new Vector2(0, ypos);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
