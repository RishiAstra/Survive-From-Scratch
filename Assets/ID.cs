using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ID : MonoBehaviour
{
	public string idString;
	public int id = -1;

	// Start is called before the first frame update
	void Awake()
	{
		id = GameControl.NameToId(idString);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
