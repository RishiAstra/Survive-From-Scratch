using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
	public Vector3 b;
	public Transform[] linkPoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public Vector3 GetSize()
	{
		return transform.TransformVector(b);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawCube(transform.position, b);
	}
}
