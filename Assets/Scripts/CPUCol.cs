using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUCol : MonoBehaviour {

	// Use this for initialization
	void Start () {
		foreach(Rigidbody g in FindObjectsOfType<Rigidbody>())
		{
			if(g.gameObject.name == "S")
			{
				g.AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))*100);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
