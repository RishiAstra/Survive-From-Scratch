using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Equip))]
public class Buildable : MonoBehaviour
{
	private const float rotationSnap = 90f;
	private const float positionSnap = 1f;

	public Equip me;
	public GameObject ghostPrefab;
	public GameObject finalPrefab;
	public LayerMask buildOnTopOf;//TODO: move this to a centeral settings/constants class
	public float buildRange = 10f;//TODO: see above

	public GameObject ghost;
	public BuildingGhost gh;

	private Camera mainCamera;
	private Vector3 pRot;
    // Start is called before the first frame update
    void Start()
    {
		me = GetComponent<Equip>();
		ghost = Instantiate(ghostPrefab);
		gh = ghost.GetComponent<BuildingGhost>();
		mainCamera = Camera.main;
		BuildControl.main.building = true;
		BuildControl.main.ghostFollower.gameObject.SetActive(true);

	}

	void OnDestroy()
	{
		BuildControl.main.building = false;
		Destroy(ghost);
		if(BuildControl.main.ghostFollower != null) BuildControl.main.ghostFollower.gameObject.SetActive(false);

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
				ghost.SetActive(true);
				if (Input.GetKeyDown(KeyCode.R))
				{
					float toRotate = rotationSnap;
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						toRotate *= -1;
					}

					if (BuildControl.main.a == Axis.x)
					{
						ghost.transform.Rotate(Vector3.right, toRotate, Space.World);
					}
					else if (BuildControl.main.a == Axis.y)
					{
						ghost.transform.Rotate(Vector3.up, toRotate, Space.World);
					}
					else if (BuildControl.main.a == Axis.z)
					{
						ghost.transform.Rotate(Vector3.forward, toRotate, Space.World);
					}
				}


				Vector3 pos = rh.point;

				Vector3 relPos = pos - me.bob.cam.transform.position;
				//relPos.Normalize();
				//relPos *= 0.1f * positionSnap;

				//if a dimension of the building is smaller than the snap size, round towards the player's position. 
				//This will prevent floors from being underground and prevent walls from being in cliffs

				Vector3 size = gh.GetSize();
				Vector3 n = rh.normal;
				float normalThreshold = 0.8f;

				//if placing against a flat surface, move building so it rests against the flat surface rather than centering it at the point
				if (Mathf.Abs(Vector3.Dot(n, Vector3.right)		) > normalThreshold) pos.x += (relPos.x > 0 ? -size.x / 2 : size.x / 2);
				if (Mathf.Abs(Vector3.Dot(n, Vector3.up)		) > normalThreshold) pos.y += (relPos.y > 0 ? -size.y / 2 : size.y / 2);
				if (Mathf.Abs(Vector3.Dot(n, Vector3.forward)	) > normalThreshold) pos.z += (relPos.z > 0 ? -size.z / 2 : size.z / 2);
				//print(size);
				BuildControl.main.ghostFollower.position = pos;

				//if (size.x < positionSnap) pos.x = relPos.x > 0 ? Mathf.Floor(pos.x / positionSnap) * positionSnap : Mathf.Ceil(pos.x / positionSnap) * positionSnap;
				//if (size.y < positionSnap) pos.y = relPos.y > 0 ? Mathf.Floor(pos.y / positionSnap) * positionSnap : Mathf.Ceil(pos.y / positionSnap) * positionSnap;
				//if (size.z < positionSnap) pos.z = relPos.z > 0 ? Mathf.Floor(pos.z / positionSnap) * positionSnap : Mathf.Ceil(pos.z / positionSnap) * positionSnap;

				//if (size.x < positionSnap) pos.x = relPos.x > 0 ? pos.x - size.x / 2 : pos.x + size.x / 2;
				//if (size.y < positionSnap) pos.y = relPos.y > 0 ? pos.y - size.y / 2 : pos.y + size.y / 2;
				//if (size.z < positionSnap) pos.z = relPos.z > 0 ? pos.z - size.z / 2 : pos.z + size.z / 2;

				pos = new Vector3(
					Mathf.Round(pos.x / positionSnap) * positionSnap,
					Mathf.Round(pos.y / positionSnap) * positionSnap,
					Mathf.Round(pos.z / positionSnap) * positionSnap
				);

				////favor closer positions when rounding
				//pos = roundedValues;


				//pos.x = relPos.x > 0 ? (Mathf.Floor(pos.x / positionSnap) * positionSnap) : (Mathf.Ceil(pos.x / positionSnap) * positionSnap);
				//pos.y = relPos.y > 0 ? (Mathf.Floor(pos.y / positionSnap) * positionSnap) : (Mathf.Ceil(pos.y / positionSnap) * positionSnap);
				//pos.z = relPos.z > 0 ? (Mathf.Floor(pos.z / positionSnap) * positionSnap) : (Mathf.Ceil(pos.z / positionSnap) * positionSnap);


				Vector3 rp = rh.point - me.bob.transform.position;
				rp.y = 0;
				//Quaternion r = Quaternion.LookRotation(rp.normalized, Vector3.up);

				//offset the rotation by the player's current rotation
				Vector3 euler = Quaternion.LookRotation(rp.normalized, Vector3.up).eulerAngles;// me.bob.transform.eulerAngles;
				euler.x = 0;// Mathf.Round(euler.x / 90f) * 90f;
				euler.y = Mathf.Round(euler.y / rotationSnap) * rotationSnap;
				euler.z = 0;// Mathf.Round(euler.z / 90f) * 90f;

				if (euler != pRot)
				{
					//Quaternion rot = Quaternion.Euler(euler) * Quaternion.Inverse(Quaternion.Euler(pRot));
					//ghost.transform.rotation *= rot;
					ghost.transform.Rotate(Vector3.up, euler.y - pRot.y, Space.World);
					pRot = euler;
				}
				//Quaternion rot = Quaternion.Euler(euler) * Quaternion.Inverse(relRot);
				//relRot = rot;

				ghost.transform.position = pos;
				BuildControl.main.transform.position = pos;

				if (Input.GetMouseButtonUp(0) && ! gh.overlapping)
				{
					GameObject g = Instantiate(finalPrefab, pos, ghost.transform.rotation);
					me.bob.RemoveItem(me.bob.invSel);
					if(me.bob.inv.items[me.bob.invSel].amount <= 0)
					{
						me.bob.RefreshSelected();
					}
				}
			}
			else
			{
				//clear overlaps because it will now sleep and won't receive ontriggerexit
				gh.overlaps = new List<Transform>();
				gh.overlapping = false;
				ghost.SetActive(false);
			}			
		}		
	}
}
