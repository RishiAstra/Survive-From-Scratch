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

	//private void OnPreRender()
	//{
	//	temp = Time.time;
	//}

	//private void OnPostRender()
	//{
	//	if (f.Count > 20)
	//	{
	//		f.RemoveAt(0);
	//	}
	//	f.Add(Time.time - temp);
	//	wait = 0;
	//	int c = 0;
	//	for (int i = 0; i < f.Count; i++)
	//	{
	//		wait += f[i];
	//		c++;
	//	}
	//	wait /= c;
	//}

	void LockMouse()
	{
		Cursor.lockState = CursorLockMode.Locked;
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
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		transform.localPosition = offset * Mathf.Lerp(transform.localPosition.magnitude, dist, ZOOM_LERP_SPEED);
		//Vector3 temp = transform.parent.eulerAngles;
				//temp.x = Mathf.Clamp(Mathf.DeltaAngle(0, transform.eulerAngles.x - Input.GetAxis("Mouse Y") * mouseSensitivity.y * Time.fixedDeltaTime * (invertMouseY ? 1 : -1)), minX, maxX);
				//transform.parent.eulerAngles = temp;
				//if (myPlayer.isDead)
				//{
				//	Cursor.lockState = CursorLockMode.None;
				//}
		//else
		//{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				//dist = Mathf.Clamp(dist + Input.GetAxis("Mouse ScrollWheel") * scrollSencitivity, minDist, maxDist);
				
				//Vector3 temp = transform.parent.eulerAngles;
				//temp.x = Mathf.Clamp(Mathf.DeltaAngle(0, transform.eulerAngles.x - Input.GetAxis("Mouse Y") * mouseSensitivity.y * Time.fixedDeltaTime * (invertMouseY ? 1 : -1)), minX, maxX);
				//transform.parent.eulerAngles = temp;
			}
			if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
			{
				Cursor.lockState = CursorLockMode.None;
			}
			if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
			{
				if (Input.GetMouseButtonUp(0))
				{
					//Invoke("LockMouse", 0.2f);
					Cursor.lockState = CursorLockMode.Locked;

				}

			}
		//}




		if (Input.GetKey(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
		}


	}
}
