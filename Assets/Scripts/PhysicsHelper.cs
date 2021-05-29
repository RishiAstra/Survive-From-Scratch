/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOTE: targets are required to be children (or on) of this script's gameobject
/// </summary>
public class PhysicsHelper : MonoBehaviour
{
    public List<Transform> targets;
    public List<Transform> homes;
    public GameObject helper;

    private List<GameObject> toDestroy;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

	public void Initialize()
	{
        toDestroy = new List<GameObject>();
        for (int i = 0; i < targets.Count;i++)
		{
            Transform t = targets[i];

            GameObject g = homes[i].gameObject;
            Rigidbody r;
            if (g == null)
            {
                print("no helper, made one");
                g = new GameObject(t.gameObject.name + "_home");
                r = g.AddComponent<Rigidbody>();
                r.isKinematic = true;
                g.transform.SetParent(t.parent);
                g.transform.position = t.position;
                g.transform.rotation = t.rotation;
            }
			else
			{
                r = g.GetComponent<Rigidbody>();
			}
            


            GameObject h = Instantiate(helper);
            h.name = t.root.name + "_Physics helper";
            h.transform.position = t.position;
            h.transform.rotation = t.rotation;
            Joint j = h.GetComponent<Joint>();
            j.connectedBody = r;
            j.connectedAnchor = Vector3.zero;

            PositionLimiter p = h.GetComponent<PositionLimiter>();
            p.relativeTo = g.transform;

            toDestroy.Add(h);

            Follow f = t.GetComponent<Follow>();
            if(f != null) f.target = h.transform;
        }
	}

	private void OnDestroy()
	{
		foreach(GameObject g in toDestroy)
		{
            if(g != null) Destroy(g);
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
