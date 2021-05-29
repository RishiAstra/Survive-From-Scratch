/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(ShopKeeper))]
public class ShopKeeperEditor : Editor
{
	public override void OnInspectorGUI()
	{
		ShopKeeper s = target as ShopKeeper;
		DrawDefaultInspector();
		if (GUILayout.Button("Save"))
		{
			s.SaveDeals();
		}

		if (GUILayout.Button("Load"))
		{
			s.OnEnable();
		}

	}
}
