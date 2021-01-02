using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bobStuff;
using System;
using UnityEngine.UI;
//TODO: this class can do player actions unique to the player being controlled by this client in multiplayer, especially because this class knows which player is this client's player.
public class gameControll : MonoBehaviour
{
	//public static int curserFreeCount = 0;//use this to prevent the cursor from locking
	public static bool tempUnlockMouse;

	public static Dictionary<string, int> StringIdMap;
	public static gameControll main;

	public LayerMask raycastLayerMask;
	public LayerMask weaponHit;
	public GameObject player;
	public InventoryUI hotBarUI;
	public Inventory craftInventory;
	public Image mainHpBar;
	public Canvas mainCanvas;
	
	[HideInInspector()]public List<ItemType> itemTypes;
//	public RPGCamera Camera;
	private Player me;
	[HideInInspector]public Abilities myAbilities;

	void Awake(){
		
		main = this;
		craftInventory.gameObject.SetActive(false);
		InitializeItemTypes();
		CreatePlayerObject();
	}


	public static int NameToId(string s)
	{
		return StringIdMap[s];
	}

	private void InitializeItemTypes()
	{
		StringIdMap = new Dictionary<string, int>(itemTypes.Count);

		for(int i = 0; i < itemTypes.Count; i++)
		{

			ItemType item = itemTypes[i];

			//add to the dictionary to convert names to ids
			StringIdMap.Add(item.name, i);

			//WARNING: Resources.Load returns null if not found
			//load data for this item type
			item.prefab =			Resources.Load<GameObject>	(item.name + "/" + item.name + "-p");
			item.equipPrefab =		Resources.Load<GameObject>	(item.name + "/" + item.name + "-e");
			item.icon =				Resources.Load<Sprite>	(item.name + "/" + item.name + "-i");
			itemTypes[i] = item;
		}
	}

	//void OnJoinedRoom()
	//{
	//	CreatePlayerObject();
	//}

	void Respawn()
	{
		//TODO: clear all statis effects, maybe just delete player and spawn a new one
		myAbilities.Reset();
		if (GUI.Button(new Rect((Screen.width - 100) / 2, (Screen.height - 25) / 2, 100, 25), "Respawn"))
		{
			me.gameObject.SetActive(true);
			me.Respawn();
		}
	}

	void OnGUI(){
		if (me!=null && myAbilities.dead)
		{// && me.ph.isMine
			Respawn();
		}
	}

	void TryLockCursor()
	{
		if(
			!craftInventory.gameObject.activeSelf &&
			!tempUnlockMouse
			)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	void TryUnlockCursor()
	{
		Cursor.lockState = CursorLockMode.None;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
		{
			tempUnlockMouse = true;
			TryUnlockCursor();
		}
		//TODO: this glitches when ctrl to mouse exit, then mouse enter it stuck till ctrl again
		if (tempUnlockMouse && (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl) || Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)))
		{
			tempUnlockMouse = false;
			TryLockCursor();
		}

		if (Cursor.lockState == CursorLockMode.Locked)
		{
			
			if (Input.GetKey(KeyCode.Escape))
			{
				TryUnlockCursor();
			}
		}
		else
		{
			if (Input.GetMouseButtonUp(0))
			{
				TryLockCursor();
			}
		}		

		if (Input.GetKeyDown(KeyCode.E))
		{
			if (craftInventory.gameObject.activeSelf)
			{
				craftInventory.gameObject.SetActive(false);
				TryLockCursor();
			}
			else
			{
				craftInventory.gameObject.SetActive(true);
				TryUnlockCursor();
			}
		}

		if (myAbilities.dead) {
			//deactivate crafting if dead
			if (craftInventory.gameObject.activeSelf)
			{
				craftInventory.gameObject.SetActive(false);
			}
			TryUnlockCursor();
		}
		else
		{
			LiveFunctions();
		}

	}

	private void LiveFunctions()
	{
		if (Input.GetKey(KeyCode.F))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Player.main.grabDist, raycastLayerMask))
			{
				GameObject g = hit.collider.gameObject;
				Collectible c = g.GetComponent<Collectible>();
				if (c != null)
				{
					//print("click me");
					c.MouseClickMe();
				}
			}
			else
			{
				//print("no hit");
			}
		}
	}

	void CreatePlayerObject()
	{
		Vector3 position;
		spawnPoint[] sp = GameObject.FindObjectsOfType<spawnPoint> ();
		int chosen = UnityEngine.Random.Range (0, sp.Length);
		position = sp [chosen].transform.position;

		GameObject newPlayerObject = Instantiate(player, position, Quaternion.identity);//PhotonNetwork.
		me = newPlayerObject.GetComponent<Player> ();
		newPlayerObject.GetComponent<HPBar>().hpBarImage = mainHpBar;//TODO: check taht this works
		Player.main = me;
		myAbilities = newPlayerObject.GetComponent<Abilities>();
		hotBarUI.target = newPlayerObject.GetComponent<Inventory>();
		if (Player.main == null) {
			print ("UG");
		}
	}
}
