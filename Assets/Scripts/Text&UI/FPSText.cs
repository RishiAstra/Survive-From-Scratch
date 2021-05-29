/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSText : MonoBehaviour {
	public float sampleTime;
	private TextMeshProUGUI text;
	private float sampleTimeLeft;
	private int frames;
	// Use this for initialization
	void Start () {
		text = GetComponent<TextMeshProUGUI>();
	}
	
	// Update is called once per frame
	void Update () {
		sampleTimeLeft -= Time.unscaledDeltaTime;
		frames++;
		if (sampleTimeLeft <= 0)
		{
			text.text = "FPS " + Mathf.RoundToInt(frames/sampleTime);
			sampleTimeLeft += sampleTime;
			frames = 0;
		}
	}
}
