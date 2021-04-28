using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: in all scripts if performance is bad, consider replacing List<> with HashSet<>
public class PlayerControl : MonoBehaviour, ISaveable
{
	public const float CAM_LERP_SPEED = 0.02f;
	//public const float INPUT_THRESHOLD = 0.01f;

	public Transform camPos;
	public GameObject camPref;
	public Movement movement;
	public Abilities abilities;
	public Cam cam;
	//public Vector2 sensitivity;
	public float scrollSencitivity;
	public string playerOwnerName;

	void Awake()
	{
		
	}

	// Start is called before the first frame update
	void Start()
    {
		//cam = Instantiate(camPref, camPos.position, camPos.rotation).GetComponentInChildren<Cam>();
    }

	private void LateUpdate()
	{
		cam.pivot.position = camPos.position;
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			//set y rotation (horizontal)
			Vector3 temp = cam.pivot.eulerAngles;
			temp.y += Input.GetAxis("Mouse X") * GameControl.main.mouseSensitivity.x;
			//print(Input.GetAxis("Mouse X") * GameControl.main.mouseSensitivity.x + "|" + Input.GetAxis("Mouse X"));
			cam.pivot.eulerAngles = temp;

			//use the attack
			if (Input.GetMouseButton(0))
			{
				if (BuildControl.main.building)
				{
					//BuildControl.main.PlaceBuilding();
				}
				else
				{
					abilities.UseSkill(0);
				}
			}

			//change distance and pitch
			cam.AddDist(Input.GetAxis("Mouse ScrollWheel") * scrollSencitivity);
			cam.AddPitch(Input.GetAxis("Mouse Y") * GameControl.main.mouseSensitivity.y);
		}
	}


	// Update is called once per frame
	void Update()
    {
		if(Cursor.lockState == CursorLockMode.Locked)
		{
			////set y rotation (horizontal)
			//Vector3 temp = cam.pivot.eulerAngles;
			//temp.y += Input.GetAxis("Mouse X") * sensitivity.x * Time.deltaTime;
			//cam.pivot.eulerAngles = temp;

			//use the attack
			if (Input.GetMouseButton(0) && !BuildControl.main.building)
			{
				abilities.UseSkill(0);
			}

			////change distance and pitch
			//cam.AddDist(Input.GetAxis("Mouse ScrollWheel") * scrollSencitivity);
			//cam.AddPitch(Input.GetAxis("Mouse Y") * sensitivity.y * Time.deltaTime);
		}
		

		//jump
		if (Input.GetKey(KeyCode.Space)) movement.AttemptJump();
		//move		
		//movement.SetAngle(cam.transform.eulerAngles.y);
		Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		dir = Quaternion.Euler(0, cam.pivot.eulerAngles.y, 0) * dir;
		////rotate towards the direction if actually moving
		//if (dir.magnitude > INPUT_THRESHOLD) movement.SetAngle(Quaternion.LookRotation(dir, transform.up));
		movement.SetDirection(dir);
		movement.SetAngleFromDirection();
	}


	public string GetData()
	{
		SaveDataPlayerControl s = new SaveDataPlayerControl(playerOwnerName);
		return JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings);
	}

	public void SetData(string data)
	{
		SaveDataPlayerControl s = JsonConvert.DeserializeObject<SaveDataPlayerControl>(data);
		//TODO: warning, sceneindex not considered here
		this.playerOwnerName = s.playerOwnerName;


		if (playerOwnerName == GameControl.username)
		{
			GameControl.main.SetUpPlayer(gameObject);
		}
		else
		{
			Debug.LogWarning("spawned someone else's player, their username: " + playerOwnerName + ", my username: " + GameControl.username);
		}
	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "PlayerControlled";
	}
}
