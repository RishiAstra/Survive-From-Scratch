using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
	public Vector3 b;
	public Transform[] linkPoints;

	//these are used to prevent placing buildings in overlapping positions
	public bool overlapping;
	//public LayerMask overlapMask;

	public List<Transform> overlaps;
	//private List<BuildingGhost> bgs;
    // Start is called before the first frame update
    void Start()
    {
		overlapping = false;
		overlaps = new List<Transform>();
		//bgs = new List<BuildingGhost>();
    }

    // Update is called once per frame
    void Update()
    {
		//overlapping = false;
  //      foreach(BuildingGhost b in bgs)
		//{

		//}
    }

	private void OnTriggerEnter(Collider other)
	{
		//if contained in overlapMask
		//if((overlapMask & (1 << other.gameObject.layer)) > 0 && ! overlaps.Contains(other.transform))
		//{
		if (overlaps.Contains(other.transform)) Debug.LogError("e1");
		overlaps.Add(other.transform);
		UpdateOverlapBool();
			//BuildingGhost bg = other.GetComponent<BuildingGhost>();
			//if (bg != null) bgs.Add(bg);
		//}
	}

	private void UpdateOverlapBool()
	{
		overlapping = overlaps.Count > 0;
	}

	private void OnTriggerExit(Collider other)
	{
		//if contained in overlapMask
		//if ((overlapMask & (1 << other.gameObject.layer)) > 0)
		//{
			bool s = overlaps.Remove(other.transform);
			if (!s) Debug.LogError("e");
			UpdateOverlapBool();
			//BuildingGhost bg = other.GetComponent<BuildingGhost>();
			//if (bg != null) bgs.Remove(bg);
		//}
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
