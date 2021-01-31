using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bobStuff;
using System;

public class gameControll : MonoBehaviour
{
	public static Dictionary<string, int> StringIdMap;
	public static gameControll main;

	public LayerMask raycastLayerMask;
	public GameObject player;
<<<<<<< Updated upstream
=======
	public GameObject camPref;
	public Transform camPos;//start the camera here
	public InventoryUI hotBarUI;
	public GameObject craftInventory;
	public GameObject middleCursor;
	public Image mainHpBar;
	public Canvas mainCanvas;

	public GameObject camGameObject;
>>>>>>> Stashed changes
	
	[HideInInspector()]public List<ItemType> itemTypes;
//	public RPGCamera Camera;
	private Player me;

	void Awake(){
		if (main != null) Debug.LogError("two gameControls");
		main = this;
<<<<<<< Updated upstream
=======
		camGameObject = Instantiate(camPref, camPos.position, camPos.rotation);
		//craftInventory.SetActive(false);
>>>>>>> Stashed changes
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
			item.icon =				Resources.Load<Texture2D>	(item.name + "/" + item.name + "-i");
			itemTypes[i] = item;
		}
	}

	//void OnJoinedRoom()
	//{
	//	CreatePlayerObject();
	//}

	void OnGUI(){
		if (me!=null && me.isDead)
		{// && me.ph.isMine
			if (GUI.Button (new Rect ((Screen.width - 100) / 2, (Screen.height - 25) / 2, 100, 25), "Respawn")) {
				me.gameObject.SetActive (true);
				me.Respawn ();
			}
		}
	}

	private void Update()
	{
		if (me.isDead) return;
		if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.C))
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
		Player.main = me;
<<<<<<< Updated upstream
		if (Player.main == null) {
			print ("UG");
		}


//		Camera.Target = newPlayerObject.transform;
=======
		myAbilities = newPlayerObject.GetComponent<Abilities>();
		hotBarUI.target = newPlayerObject.GetComponent<Inventory>();
		if (Player.main == null) Debug.LogError("Failed to create main character");
		Inventory myInv = newPlayerObject.GetComponent<Inventory>();
		myInv.take = true;
		myInv.put = true;
>>>>>>> Stashed changes
	}
}
