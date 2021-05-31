/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class MapCamera : MonoBehaviour
{
    [Range(1, 25)] public float fps;
    public Camera cam;
    //public Shader unlit;

    private double ps;
    private Stopwatch sw;
    private Transform mainCameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        sw = new Stopwatch();
        sw.Start();
        mainCameraTransform = Camera.main.transform.root;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = mainCameraTransform.position;
        pos.y = transform.position.y;
        transform.position = pos;

        double ticks = sw.ElapsedTicks;
        double seconds = ticks / Stopwatch.Frequency;

		double timePerFrame = 1.0 / fps;
		if (seconds - ps > timePerFrame)
		{
			

            if(seconds - ps > timePerFrame * 3)
			{
                //there was lag of more than 3x a frame or something, so just reset the time difference to prevent it from trying to catch up
                ps = seconds;
			}
			else
			{
                //one frame was completed, so remove 1 frame worth of time from the timer
                ps += timePerFrame;
            }

            cam.Render();// WithShader(unlit, null);
		}
    }
}
