﻿/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill : ScriptableObject
{
	[TextArea(5, 10)]
	public string description;
	public bool levelable;
}