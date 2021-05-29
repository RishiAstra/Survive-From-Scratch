/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    public Abilities parent;//used to calculate damage etc.
    // Start is called before the first frame update

    public float GetDamageAmount(AttackType type, float dmgMult = 1)
	{
        return parent.myStat.GetOutputDamageAmount(type, dmgMult);
	}
}
