using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {
	public static Transform cam;

	//public Vector3 offset;
	// Use this for initialization
	void Start () {
		if (cam == null) cam = Camera.main.transform;
	}
	
	// Update is called once per frame
	void OnWillRenderObject () {
		transform.forward = cam.forward;//.LookAt (Camera.main.transform.position);
	}
}
