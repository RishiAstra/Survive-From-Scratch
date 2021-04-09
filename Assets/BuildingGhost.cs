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
		Vector3 size = transform.TransformVector(b);
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		size.z = Mathf.Abs(size.z);
		return size;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, Vector3.Scale(b, transform.lossyScale));
	}
}
