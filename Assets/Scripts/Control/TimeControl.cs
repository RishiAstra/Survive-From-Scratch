/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct floatstring
{
    public float f;
    public string s;
    public floatstring(float nf, string ns)
	{
        f = nf;
        s = ns;
	}
}
/// <summary>
/// This class handles time scales to allow multiple things to put time scales in a queue and remove some. 
/// For example, a skill causes time to slow down to 0.5f, then a menu activates and makes time 0.0f. 
/// When the menu is deactivated, the time scale should return to 0.5f, not 1.0f.
/// to remove specfic timescales, a string id is included
/// </summary>

public class TimeControl : MonoBehaviour
{
    public static TimeControl main;

    public List<floatstring> times;
    // Start is called before the first frame update
    void Awake()
    {
        if (main != null) Debug.LogError("two TimeControl");
        main = this;
        times = new List<floatstring>();
    }

    public void SetTimeScale(float scale, string id)
	{
        times.Add(new floatstring(scale, id));
        UpdateTimeScale();
        //print("made time scale: " + id + ", scale: " + scale);
	}

    public void RemoveTimeScale(string id)
	{
        for(int i = times.Count - 1; i >= 0; i--)
		{
            if(times[i].s == id)
			{
                times.RemoveAt(i);
                //print("removed time scale: " + id);
                UpdateTimeScale();
                return;
			}
		}
        Debug.LogError("tried to remove a timescale but it didn't exist");
        UpdateTimeScale();
	}

	private void UpdateTimeScale()
	{
        if(times.Count > 0)
		{
            Time.timeScale = times[times.Count - 1].f;
        }
        else
		{
            Time.timeScale = 1f;
        }
    }

	// Update is called once per frame
	void Update()
    {
        
    }
}
