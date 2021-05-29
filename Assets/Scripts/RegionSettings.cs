/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionSettings : MonoBehaviour
{
    public static RegionSettings main;

	public bool allowCombat = true;
	public bool allowBuilding = true;

	private void Awake()
	{
		print("Region Settings started");
		main = this;
	}
}
