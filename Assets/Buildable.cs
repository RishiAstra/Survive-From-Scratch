using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Equip))]
public class Buildable : MonoBehaviour
{
	private const float rotationSnap = 90f;
	private const float positionSnap = 2f;

	public Equip me;
	public GameObject ghostPrefab;
	public GameObject finalPrefab;
	public LayerMask buildOnTopOf;//TODO: move this to a centeral settings/constants class
	public float buildRange = 10f;//TODO: see above

	public GameObject ghost;

	private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
		me = GetComponent<Equip>();
		ghost = Instantiate(ghostPrefab);
		mainCamera = Camera.main;
		BuildControl.main.building = true;
    }

	void OnDestroy()
	{
		BuildControl.main.building = false;
	}

	// Update is called once per frame
	void Update()
    {
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			RaycastHit rh;
			bool hitAnything = Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rh, buildRange, buildOnTopOf);

			if (hitAnything)
			{
				if (Input.GetKeyDown(KeyCode.R))
				{
					float toRotate = rotationSnap;
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						toRotate *= -1;
					}

					if (BuildControl.main.a == Axis.x)
					{
						ghost.transform.Rotate(Vector3.right, toRotate);
					}
					else if (BuildControl.main.a == Axis.y)
					{
						ghost.transform.Rotate(Vector3.up, toRotate);
					}
					else if (BuildControl.main.a == Axis.z)
					{
						ghost.transform.Rotate(Vector3.forward, toRotate);
					}
				}


				Vector3 pos = rh.point;
				pos.x = Mathf.Round(pos.x / positionSnap) * positionSnap;
				pos.y = Mathf.Round(pos.y / positionSnap) * positionSnap;
				pos.z = Mathf.Round(pos.z / positionSnap) * positionSnap;


				//Vector3 relPos = rh.position - me.bob.transform.position;//
				//Quaternion r = Quaternion.LookRotation(relPos.normalized, Vector3.up);
				Vector3 euler = me.bob.transform.eulerAngles;
				euler.x = 0;// Mathf.Round(euler.x / 90f) * 90f;
				euler.y = Mathf.Round(euler.y / rotationSnap) * rotationSnap;
				euler.z = 0;// Mathf.Round(euler.z / 90f) * 90f;
				Quaternion rot = Quaternion.Euler(euler);

				ghost.transform.position = pos;
				ghost.transform.rotation = rot;
				BuildControl.main.transform.position = pos;

				if (Input.GetMouseButton(0))
				{
					GameObject g = Instantiate(finalPrefab, pos, rot);
					me.bob.RemoveItem(me.bob.invSel);
					if(me.bob.inv.items[me.bob.invSel].amount <= 0)
					{
						me.bob.RefreshSelected();
					}
				}
			}			
		}		
	}
}
