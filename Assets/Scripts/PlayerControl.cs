using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: in all scripts if performance is bad, consider replacing List<> with HashSet<>
public class PlayerControl : MonoBehaviour
{
	public const float CAM_LERP_SPEED = 0.02f;

	public Transform camPos;
	public GameObject camPref;
	public Movement movement;
	public Abilities abilities;
	public Cam cam;
	public Vector2 sensitivity;
	public float scrollSencitivity;

	// Start is called before the first frame update
	void Start()
    {
		cam = Instantiate(camPref, camPos.position, camPos.rotation).GetComponentInChildren<Cam>();
    }

	private void FixedUpdate()
	{
		//chase position
		cam.pivot.position = camPos.position;// Vector3.Lerp(cam.pivot.position, camPos.position, CAM_LERP_SPEED);

	}


	// Update is called once per frame
	void Update()
    {
		//set y rotation (horizontal)
		Vector3 temp = cam.pivot.eulerAngles;
		temp.y += Input.GetAxis("Mouse X") * sensitivity.x * Time.deltaTime;
		cam.pivot.eulerAngles = temp;

		if (Input.GetKey(KeyCode.Space)) movement.AttemptJump();
		//cam.pivot.transform.Rotate(0, , 0);
			movement.SetAngle(cam.transform.eulerAngles.y);
		Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		dir = Quaternion.Euler(0, cam.pivot.eulerAngles.y, 0) * dir;
		movement.SetDirection(dir);

		if (Input.GetMouseButton(0))
		{
			abilities.UseSkill(0);
		}

		//change distance and pitch
		cam.AddDist(Input.GetAxis("Mouse ScrollWheel") * scrollSencitivity);
		cam.AddPitch(Input.GetAxis("Mouse Y") * sensitivity.y * Time.deltaTime);
    }
}
