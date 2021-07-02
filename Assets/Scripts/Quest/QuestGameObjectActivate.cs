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
	public bool defaultActive;
	public Animator anim;


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
		if (ProgressTracker.main.activates.TryGetValue(myName, out QuestGameObjectData result))
		{
			gameObject.SetActive(result.active);
			if(anim != null && !string.IsNullOrEmpty(result.animationState))
			{
				anim.Play(result.animationState);
			}
		}
		else
		{
			gameObject.SetActive(defaultActive);
		}
	}
}

public struct QuestGameObjectData
{
	public bool active;
	public string animationState;
	public float animationNormalizedTime;
}