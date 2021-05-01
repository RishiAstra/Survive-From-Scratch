using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ID))]
public class Collectible : MonoBehaviour, IMouseHoverable
{
	//public static Transform cam;
	//public string idString;
	//public int id;
	public int amount;

	public ID myID;
	//public LayerMask layerMask;

	//private Rigidbody rig;
	//public float sleepThreshold;
	//public int contacts;
	//public int ac;
	//public float checkRad;
	//public GameObject sign;
	//public float signDist;

	// Use this for initialization
	void Awake()
	{
		myID = GetComponent<ID>();
		//id = gameControll.NameToId(idString);
		//rig = GetComponent<Rigidbody>();
		//rig.sleepThreshold = sleepThreshold;
		//cam = Camera.main.transform;
	}

	// Update is called once per frame
	//void Update()
	//{
	//	//if (Time.frameCount % 10 == 0)
	//	//{
	//		//if (contacts <= 0 && (rig.velocity.magnitude < sleepThreshold) && ac != 0)//.IsSleeping())
	//		//{

	//		//	//rig.isKinematic = true;
	//		//}
	//		//else
	//		//{
	//		//	//rig.isKinematic = false;
	//		//}
	//	//if (ac == 0) rig.isKinematic = false;
	//	//}
	//	//Vector3 temp = cam.position - transform.position;
	//	//sign.SetActive(temp.sqrMagnitude < signDist * signDist);
	//}

	public void MouseClickMe()
	{
		if (Player.main != null)
		{
			if(GameControl.main.GetItem(myID.id, amount))//Player
			{
				Destroy(gameObject);
			}
			else
			{
				Debug.LogWarning("Inventory full");
			}
			return;
		}
		else
		{
			Debug.LogError("Can't find player owned by this device");
		}
	}

	public void OnMouseHoverFromRaycast()
	{

		GameControl.main.itemHoverInfo.SetActive(true);
		//if (itemHoverPositionMatch) itemHoverInfo.transform.position = Input.mousePosition;
		if (Input.GetKey(KeyCode.F))
		{
			MouseClickMe();
		}
	}

	public void OnMouseStopHoverFromRaycast()
	{
		GameControl.main.itemHoverInfo.SetActive(false);
		//don't care if mouse isn't over this, but need function for interface
	}

	//public void OnCollisionEnter(Collision collision)
	//{
	//	if (layerMask == (layerMask | (1 << collision.gameObject.layer)))//))(layerMask.value & collision.gameObject.layer) != int.MinValue)
	//	{
	//		contacts++;
	//		//rig.isKinematic = false;
	//	}
	//	ac++;
	//	//if (rig.velocity.magnitude > sleepThreshold)
	//	//{
	//	//	rig.isKinematic = false;
	//	//	//Physics.IgnoreCollision(rig.GetComponent<Collider>(), collision.rigidbody.GetComponent<Collider>());
	//	//	//rig.velocity = Vector3.zero;
	//	//	//rig.Sleep();
	//	//	//collision.rigidbody.velocity = Vector3.zero;
	//	//	//collision.rigidbody.Sleep();
	//	//}
	//	//}
	//	//collision.relativeVelocity
	//}
	//public void OnCollisionExit(Collision collision)
	//{
	//	if (layerMask == (layerMask | (1 << collision.gameObject.layer)))//))(layerMask.value & collision.gameObject.layer) != int.MinValue)
	//	{
	//		contacts--;

	//	}

	//	ac--;
	//	if (collision.rigidbody!=null&&ac<=3)
	//	{
	//		//rig.isKinematic = false;
	//	}
	//	//if (ac <= 3)
	//	//{
	//	//	rig.isKinematic = false;
	//	//}
	//}
}
