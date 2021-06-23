/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bobStuff;

//TODO: save stuff in saveentity.cs
[RequireComponent(typeof(StatScript))]
public class Loot : LootDropper
{

    private bool looted;
    private StatScript a;
    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<StatScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!looted && a.dead)
		{
            GenerateLoot();
            looted = true;
		}
    }

	private void OnDestroy()
	{
        if (!looted && a.dead)
        {
            GenerateLoot();
            looted = true;
        }
    }
}