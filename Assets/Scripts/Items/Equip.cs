/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip : MonoBehaviour {
	public NPCControl bob;//this is auto set by Player.cs
					  //public string idString;
					  //public int id = -1;
	public ID myID;
	// Use this for initialization
	void Awake () {
		//id = gameControll.NameToId(idString);
		myID = GetComponent<ID>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
