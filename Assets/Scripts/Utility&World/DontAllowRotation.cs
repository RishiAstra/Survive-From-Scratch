/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DontAllowRotation : MonoBehaviour
{
    public Quaternion rot;
    //[Space(10)]
    //public Quaternion rotq;
    // Start is called before the first frame update
    void Start()
    {
        
    }

	//private void OnValidate()
	//{
 //       rotq = rot;// Quaternion.Euler(rot);
	//}

	// Update is called once per frame
	void Update()
    {
        transform.rotation = rot;
    }
}
