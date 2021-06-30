/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using bobStuff;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: add good wandering behavior
public class NPCControl : MonoBehaviour, ISaveable
{
	public LayerMask targetMask;
	public Movement movement;
	public float spotRange;
	public Transform checkAttack;
	public float checkAttackRadius;

	public List<StatScript> targets;

	public bool guard;
	public Vector3 guardPosition;
	public float maxGuardDist;

	private Abilities abilities;

	//migrated from PlayerControl
	public const float CAM_LERP_SPEED = 0.02f;

	public Transform camPos;
	public GameObject camPref;
	public Cam cam;
	public bool playerControlled;
	public string playerOwnerName;

	//migrated from player
	[HideInInspector()] public Rigidbody rig;
	public Inventory inv;
	public int invSel = -1;
	public Transform rightHand;
	public Transform camT;


	// Start is called before the first frame update
	void Start()
    {
		abilities = GetComponent<Abilities>();
		movement = GetComponent<Movement>();
		targets = new List<StatScript>();

		if (guard)
		{
			Minimap.main.AddArrowedPosition(transform);
		}

		if (playerControlled)
		{
			camT = Camera.main.transform;
			
			
		}

		invSel = -1;
		rig = GetComponent<Rigidbody>();
		inv = GetComponent<Inventory>();
		Invoke("SelectInvInitial", 0.01f);
	}

    // Update is called once per frame
    void Update()
	{
		if (playerControlled)
		{
			PlayerMovement();
			//CheckHotBar();
		}
		else
		{
			NPCMovement();
		}
	}

	private void PlayerMovement()
	{
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			//use the attack
			if (Input.GetMouseButton(0) && !BuildControl.main.building)
			{
				abilities.UseSkill(0);
			}
		}

		//jump
		if (Input.GetKey(KeyCode.Space)) movement.AttemptJump();
		//move		
		Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		dir = Quaternion.Euler(0, cam.pivot.eulerAngles.y, 0) * dir;
		movement.SetDirection(dir);
		movement.SetAngleFromDirection();
	}

	private void LateUpdate()
	{
		if (playerControlled)
		{
			if (cam != null) cam.pivot.position = camPos.position;
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				//set y rotation (horizontal)
				Vector3 temp = cam.pivot.eulerAngles;
				temp.y += Input.GetAxis("Mouse X") * GameControl.main.mouseSensitivity.x;

				cam.pivot.eulerAngles = temp;

				//change distance and pitch
				cam.AddDist(Input.GetAxis("Mouse ScrollWheel") * GameControl.main.scrollSensitivity);
				cam.AddPitch(Input.GetAxis("Mouse Y") * GameControl.main.mouseSensitivity.y);
			}
		}
		
	}

	private void NPCMovement()
	{
		//TODO: can spot through things
		//float angle = abilities.skills[0].useAngle;
		//TODO: optimize spotting
		if (Time.frameCount % 5 == 0) Spot();//only spot sometimes
		if (targets.Count > 0)
		{
			int index = -1;
			for (int i = 0; i < targets.Count; i++)
			{
				if (targets[i] != null)
				{
					index = i;
				}
			}
			if (index != -1)
			{
				StatScript t = targets[index];//TODO: find optimal target, depending on intelligence
				Transform attackPlace = checkAttack;// abilities.attackTranforms[0];
				Vector3 off = t.transform.position - attackPlace.position;
				Quaternion targetAngle = Quaternion.LookRotation(off, transform.up);
				//float angleOff = Quaternion.Angle(transform.rotation, targetAngle);
				//movement.SetAngle(targetAngle);
				bool usedAttack = false;
				foreach (Collider col in Physics.OverlapSphere(checkAttack.position, checkAttackRadius, targetMask))//TODO: this can change depending on intelligence, dumb can attack at wrong distance. This should be based off of the skill's attack area/dist.
				{
					TagScript tagScript = col.GetComponent<TagScript>();//TODO: consider GetComponentInParent

					//TODO: make targets a struct or something that has tagscript and abilities
					if (tagScript != null && tagScript.ContainsTag(abilities.enemyString))
					{

						StatScript temp = col.GetComponent<StatScript>();
						if (temp != null)
						{

							usedAttack = true;
							break;
						}
					}

				}
				if (usedAttack)
				{
					abilities.UseSkill(0);
					movement.SetDirection(Vector3.zero);
				}
				else
				{

					movement.SetDirection(off.normalized);
					//movement.SetAngleFromDirection();
					Vector3 dir = t.transform.position - transform.position;
					dir.y = 0;
					Quaternion ta = Quaternion.LookRotation(dir, Vector3.up);
					movement.SetAngle(ta);
				}
			}

		}
		else
		{
			//if guarding a position, move back if too far
			if (guard && (transform.position - guardPosition).sqrMagnitude > maxGuardDist * maxGuardDist)
			{
				movement.SetDirection((guardPosition - transform.position).normalized);
				movement.SetAngleFromDirection();
			}
			else
			{
				//movement.SetAngle(targetAngle);
				movement.SetDirection(Vector3.zero);
			}
		}
	}

	void Spot()
	{
		targets = new List<StatScript>();
		foreach(Collider col in Physics.OverlapSphere(transform.position, spotRange, targetMask))
		{
			//below code was removed because the enemies simply wouldn't do anything when the player stood outside the distance and attacked them, resulting in easy kills with no danger
			//if(guard && (col.transform.position - guardPosition).sqrMagnitude > maxGuardDist * maxGuardDist){
			//	continue;//don't target stuff too far
			//}
			TagScript tagScript = col.GetComponent<TagScript>();
			if(tagScript != null && tagScript.ContainsTag(abilities.enemyString))
			{

				StatScript temp = col.GetComponent<StatScript>();
				if (temp != null)
				{
					targets.Add(temp);
				}
			}
		}
	}


	public string GetData()
	{
		SaveDataNPCControl s = new SaveDataNPCControl()
		{
			guard = this.guard,
			guardPosition = this.guardPosition,
			maxGuardDist = this.maxGuardDist,
			playerControlled = this.playerControlled,
			playerOwnerName = this.playerOwnerName
		};

		return JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings);
	}

	public void SetData(string data)
	{
		SaveDataNPCControl s = JsonConvert.DeserializeObject<SaveDataNPCControl>(data);
		this.guard = s.guard;
		this.guardPosition = s.guardPosition;
		this.maxGuardDist = s.maxGuardDist;
		this.playerControlled = s.playerControlled;
		this.playerOwnerName = s.playerOwnerName;

		//WARNING:
		//if (playerOwnerName == GameControl.username)
		//{
		//	GameControl.main.SetUpPlayer(gameObject);
		//}
		//else
		//{
		//	Debug.LogWarning("spawned someone else's player, their username: " + playerOwnerName + ", my username: " + GameControl.username);
		//}
	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "NPC_control";
	}



















	void SelectInvInitial()
	{
		SelectInv(0);//TODO: this will cause eating a consumable
		//note for above TODO; actually right now consumables aren't used like that, so it's ok
	}

	public void RefreshSelected()
	{
		if (inv == null) return;
		//TODO: should this really destroy the current equip gameobjects, or should it check if they changed first
		if (invSel >= inv.items.Count || invSel < 0) return;
		if (rightHand.childCount != 0)
		{
			for (int i = 0; i < rightHand.childCount; i++)
			{
				Destroy(rightHand.GetChild(i).gameObject);
			}
		}
		//TODO: is this necessary?
		if (GameControl.itemTypes[inv.items[invSel].id].tags.Contains(TagScript.rhEquip))
		{
			if (GameControl.itemTypes[inv.items[invSel].id].equipPrefab != null)
			{
				MakeEquipGameObject(inv.items[invSel]);
				//GameObject g = (GameObject)Instantiate(GameControl.itemTypes[inv.items[invSel].id].equipPrefab);
				//g.transform.SetParent(rightHand, false);
				//g.GetComponent<Equip>().bob = this;
				//print("Selected equipable object");
			}
		}
	}

	private void SetSelected(int index)
	{
		if (inv == null) return;

		if (playerControlled)
		{
			List<ItemIcon> tempI = GameControl.main.hotBarUI.slotI;
			foreach (ItemIcon i in tempI)
			{
				i.selected = false;
				//TODO:update it here (and below)
			}

			invSel = index;

			if (index >= 0 && index < tempI.Count)
			{
				tempI[index].selected = true;
			}
		}
		else
		{
			invSel = index;
		}
	}

	/// <summary>
	/// Select an Item in the inventory. Some actions such as eating a consumable item may happen. Returns instantly if inv ==  null
	/// </summary>
	/// <param name="index">The index in inventory of the item to select</param>
	public void SelectInv(int index)
	{
		if (inv == null) return;
		if (index >= inv.items.Count)
		{
			Debug.LogWarning("tried to select inventory slot out of range. slot count: " + inv.items.Count + ", index: " + index);
			return;// || invSel < 0
		}
		//destroy thing if previously placing thing, see todos below
		//Destroy(placeing);

		if (rightHand.childCount != 0)
		{
			//TODO: this might cause errors if children are deleted instantly
			for (int i = 0; i < rightHand.childCount; i++)
			{
				Destroy(rightHand.GetChild(i).gameObject);
			}
		}

		//Right Hand Equippable item
		if (GameControl.itemTypes[inv.items[index].id].tags.Contains(TagScript.rhEquip))
		{
			if (invSel != index)
			{
				MakeEquipGameObject(inv.items[index]);
				SetSelected(index);
				//invSel = index;
			}
			else
			{
				MakeEquipGameObject(new Item(0, 0, 0, 0));//equip fists
				SetSelected(-1);
				//invSel = -1;
			}
		}
	}

	private void MakeEquipGameObject(Item i)
	{
		int id = i.id;
		abilities.myStat.itemsEquipped = new List<Item>();
		if (GameControl.itemTypes[id].equipPrefab != null)
		{
			print("equipped something");
			GameObject g = (GameObject)Instantiate(GameControl.itemTypes[id].equipPrefab);
			g.transform.SetParent(rightHand, false);
			g.GetComponent<Equip>().bob = this;
			abilities.myStat.itemsEquipped.Add(i);
			//print("Selected equipable object");
		}
	}




	/// <summary>
	/// Removes 1 item from stack at given index
	/// </summary>
	/// <param name="index">The index to remove from</param>
	public void RemoveItem(int index) { RemoveItem(index, 1); }
	/// <summary>
	/// Removes amount from stack at <code>hissssssss</code><
	/// </summary>
	/// <param name="index">The index</param>
	/// <param name="amount">Amount to remove</param>
	public void RemoveItem(int index, int amount)
	{
		if (inv == null) return;
		if (index == -1) return;
		Item temp = inv.items[index];
		temp.amount -= amount;
		inv.items[index] = temp;
		//inv.items [index].amount -= amount;
		if (inv.items[index].amount <= 0)
		{
			if (inv.items[index].amount < 0) Debug.LogError("removed too much of item");
			inv.items[index] = new Item(0, 0, 0, 0);
			if (invSel == index)
			{
				RefreshSelected();
			}
		}
	}
	/// <summary>
	/// If player presses button from 0-9, select the corisponding index in inventory
	/// </summary>
	//void CheckHotBar()
	//{
	//	if (Input.GetKeyDown("0")) SelectInv(9);
	//	for (int i = 1; i < 10; i++)
	//	{
	//		if (Input.GetKeyDown(i.ToString()))
	//		{
	//			SelectInv(i - 1);
	//			//print("select " + i);
	//		}
	//	}
	//}
}
