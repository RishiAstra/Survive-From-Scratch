using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: make camera zoom out on death
public class Cam : MonoBehaviour
{
	public const float ZOOM_LERP_SPEED = 0.2f;

	//public Player myPlayer;
	public Transform pivot;
	//public float wait;
	//public Vector2 mouseSensitivity;
	//public bool invertMouseY;
	public Vector3 offset;
	public float dist;
	public float minDist;
	public float maxDist;
	//public float scrollSencitivity;

	public float minX;
	public float maxX;
	//private List<float> f = new List<float>();
	//private float temp = 0;
	// Use this for initialization
	void Start()
	{
		//myPlayer = GetComponentInParent<Player>();//TODO: dangerous
		dist = transform.localPosition.magnitude;
		offset = transform.localPosition / dist;
	}

	public void AddPitch(float p)
	{
		Vector3 temp = pivot.eulerAngles;
		temp.x = Mathf.Clamp(Mathf.DeltaAngle(0, pivot.eulerAngles.x + p), minX, maxX);
		pivot.eulerAngles = temp;
	}

	public void AddDist(float d)
	{
		dist += d;
		dist = Mathf.Clamp(dist, minDist, maxDist);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		transform.localPosition = offset * Mathf.Lerp(transform.localPosition.magnitude, dist, ZOOM_LERP_SPEED);
	}
}
