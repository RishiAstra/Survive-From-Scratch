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
	public bool useDefaultData;
	public QuestGameObjectData defaultData;

	private bool alreadySavedOnDestroy = false;
	private void Start()
	{
		instances.Add(this);
		if (useDefaultData)
		{
			ProgressTracker.main.activates.Add(myName, defaultData);
		}
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
				anim.Play(result.animationState, 0, result.animationNormalizedTime);
			}
		}
		else
		{
			gameObject.SetActive(defaultActive);
		}
	}

	private void OnDestroy()
	{
		CheckSave();
	}

	public static void CheckSaveAll()
	{
		for (int i = instances.Count - 1; i >= 0; i--)
		{
			QuestGameObjectActivate q = instances[i];
			q.CheckSave();
		}
	}

	private void CheckSave()
	{
		if (alreadySavedOnDestroy) return;
		alreadySavedOnDestroy = true;

		if(anim != null)
		{
			AnimatorStateInfo s = anim.GetCurrentAnimatorStateInfo(0);
			QuestGameObjectData d;

			//if an animation or active was set
			if (ProgressTracker.main.activates.TryGetValue(myName, out d))
			{
				//remember the animation time
				d.animationNormalizedTime = s.normalizedTime;
				//animation state name is already set, so no need to set it
				ProgressTracker.main.activates[myName] = d;
			}
			else
			{
				//dont need to save  
			}
		}

		instances.Remove(this);
	}
}

[System.Serializable]
public struct QuestGameObjectData
{
	public bool active;
	public string animationState;
	public float animationNormalizedTime;
}