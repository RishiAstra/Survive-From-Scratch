﻿/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bobStuff;

//TODO: save stuff in saveentity.cs
public class LootDropper : MonoBehaviour
{
    public List<LootItem> loots;


	public void GenerateLoot()
	{
		foreach(LootItem i in loots)
		{
            //chance of spawning
            if(Random.value <= i.chance)
			{
                //amount to make (exclusive max so add 1)
                int amount = Random.Range(i.minAmount, i.maxAmount + 1);
                int spawnLocationParentChildIndex = 0;
                
                //spawn that amount
                for(int j = 0; j < amount; j++){
                    Vector3 pos = transform.position;
                    Quaternion rot = transform.rotation;
                    
                    //if has spawn parent, cycle through the locations to spawn
                    if (i.spawnLocationParent != null)
					{
                        Transform t = i.spawnLocationParent.GetChild(spawnLocationParentChildIndex);
                        pos = t.position;
                        rot = t.rotation;
                        spawnLocationParentChildIndex++;
                        if (spawnLocationParentChildIndex >= i.spawnLocationParent.childCount) spawnLocationParentChildIndex = 0;
					}

                    Instantiate(GameControl.itemTypes[i.item.id].prefab, pos, rot);
				}
			}
		}
	}
}

//TODO: warning: ignores amount in item, uses min/maxamount
[System.Serializable]
public struct LootItem
{
    public int minAmount;//if drops, min/max amount
    public int maxAmount;
    public Transform spawnLocationParent;
    public float chance;//the chance that it will drop at all
    public Item item;//the item
}