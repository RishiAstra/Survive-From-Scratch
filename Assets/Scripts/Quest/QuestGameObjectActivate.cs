/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Newtonsoft.Json;

public class QuestGameObjectActivate : MonoBehaviour
{
	public static List<QuestGameObjectActivate> instances = new List<QuestGameObjectActivate>();

	public string myName;
	public bool defaultState;


	private void Start()
	{
		instances.Add(this);
		CheckActive();
	}

	public static void CheckAll()
	{
		foreach(QuestGameObjectActivate q in instances)
		{
			q.CheckActive();
		}
	}

	public void CheckActive()
	{
		if (ProgressTracker.main.activates.TryGetValue(myName, out bool result))
		{
			gameObject.SetActive(result);
		}
		else
		{
			gameObject.SetActive(defaultState);
		}
	}
}