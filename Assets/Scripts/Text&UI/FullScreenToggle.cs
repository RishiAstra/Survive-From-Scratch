/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenToggle : MonoBehaviour
{
	private int pWidth;
	private int pHeight;
	// Start is called before the first frame update
	private void Start()
	{
		Resolution r1 = new Resolution() { width = 0, height = 0 };
		Resolution r2 = new Resolution() { width = 0, height = 0 };//2nd highest resolution supported
		foreach (Resolution r in Screen.resolutions)
		{
			if (r.width > r1.width && r.height > r1.height)
			{
				r2 = r1;
				r1 = r;
			}
		}
		if(r2.width == 0 || r2.height == 0)
		{
			//default to something really small if 2nd highest recolution not found
			r2.width = 256;
			r2.height = 196;
		}
		pWidth = r2.width;
		pHeight = r2.height;
	}
	public void Toggle()
	{
		if (Screen.fullScreen)
		{
			//change to windowed mode with the previously remembered width and height
			Screen.SetResolution(pWidth, pHeight, false);
		}
		else
		{
			//go through the supported resolutions and find the biggest
			Resolution resolution = new Resolution() { width = 0, height = 0 };
			foreach(Resolution r in Screen.resolutions)
			{
				if(r.width > resolution.width && r.height > resolution.height)
				{
					resolution = r;
				}
			}

			if (resolution.width == 0 || resolution.height == 0)
			{
				//default to something really small if highest recolution not found
				resolution.width = 256;
				resolution.height = 196;
			}

			//remember the previous window size
			pWidth = Screen.width;
			pHeight = Screen.height;

			//go fullscreen at the max supported resolution
			Screen.SetResolution(resolution.width, resolution.height, true);
		}
		
	}
}
