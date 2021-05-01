using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenToggle : MonoBehaviour
{
	private int pWidth;
	private int pHeight;
    // Start is called before the first frame update
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

			//remember the previous window size
			pWidth = Screen.width;
			pHeight = Screen.height;

			//go fullscreen at the max supported resolution
			Screen.SetResolution(resolution.width, resolution.height, true);
		}
		
	}
}
