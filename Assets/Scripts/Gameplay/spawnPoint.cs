using UnityEngine;
using System.Collections;

public class spawnPoint : MonoBehaviour {
	public bool autoHeight = true;
	public LayerMask putOnTopOfThese;
	// Use this for initialization
	void Start () {
		RaycastHit hit;
		Physics.Raycast (transform.position + Vector3.up * 100, -Vector3.up, out hit, 200, putOnTopOfThese);
		transform.position = hit.point + Vector3.up;
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
