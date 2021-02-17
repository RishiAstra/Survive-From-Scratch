﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using bobStuff;
using System;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
//TODO: this class can do player actions unique to the player being controlled by this client in multiplayer, especially because this class knows which player is this client's player.
public class GameControl : MonoBehaviour
{
	//public static string playerSavePath = Application.persistentDataPath + "/Players/";
	public static string itemTypePath = Application.streamingAssetsPath + @"/Items/item types.json";//@"Assets\Resources\item types.json";
	public const string itemPath = @"Assets/Items/";


	public static byte[] sessionId;
	public static string username = "default";

	public static bool initialized;

	public static Camera mainCamera;

	//public static int curserFreeCount = 0;//use this to prevent the cursor from locking
	public static bool tempUnlockMouse;
	public static bool loading;//true if currently loading a scene
	public static bool playerExists;//is the main character existant yet? won't be if loading a scene or start screen etc

	public static List<ItemType> itemTypes;
	public static Dictionary<string, int> StringIdMap;

	public static GameControl main;

	public string serverIp;
	public string mapScenePath;
	public string controlSceneName;

	public TextMeshProUGUI mapLoadText;
	public RectTransform mapLoadBar;
	public GameObject mapLoadScreen;
	public GameObject mapScreen;
	//private AsyncOperation mapSceneProg;
	private float mapLoadBarInitialWidth;
	private float mapSceneLoadProgress;

	public LayerMask collectibleLayerMask;
	public GameObject player;
	public GameObject camPref;
	public Transform camPos;//start the camera here
	public InventoryUI hotBarUI;
	public GameObject craftInventory;
	public GameObject middleCursor;
	public GameObject itemHoverInfo;
	[Tooltip("Should the item hover info be on top of the item, or stay in it's position?")]
	public bool itemHoverPositionMatch;
	public Image mainHpBar;
	public TMPro.TextMeshProUGUI mainHpText;
	public Canvas mainCanvas;

	public GameObject camGameObject;
	
//	public RPGCamera Camera;
	private Player me;
	[HideInInspector] public Abilities myAbilities;

	void Awake(){
		if (main != null) Debug.LogError("two gameControls");
		main = this;
		TryUnlockCursor();
		mapLoadBarInitialWidth = mapLoadBar.sizeDelta.x;
		camGameObject = Instantiate(camPref, camPos.position, camPos.rotation);
		mainCamera = Camera.main;
		//craftInventory.SetActive(false);

		itemHoverInfo.SetActive(false);
		HideMenus();
		mapScreen.SetActive(true);

		//TODO: consider setting mapScreen to active
		CheckItemTypes();
		InitializeItemTypes();
	}

	private void HideMenus()
	{
		mapLoadScreen.SetActive(false);
		mapScreen.SetActive(false);
		craftInventory.SetActive(false);
	}

	/// <summary>
	/// returns if any menu is open (crafting, map, etc.)
	/// </summary>
	/// <returns></returns>
	private bool MenuActive()
	{
		return
			mapLoadScreen.activeInHierarchy ||
			mapScreen.activeInHierarchy ||
			craftInventory.activeInHierarchy;
	}

	#region Map Functions

	private void SetMapLoadProgress(float amount)
    {
		mapSceneLoadProgress = amount;
		if (loading)
		{
			mapLoadBar.sizeDelta = new Vector2(mapSceneLoadProgress * mapLoadBarInitialWidth, mapLoadBar.sizeDelta.y);
		}
	}

	public void BeginLoadMapLocation(string name)
    {
		StartCoroutine(LoadMapLocation(name));
    }

	public IEnumerator LoadMapLocation(string name)
    {
		if(!initialized)
		//TODO:test this
		//remove other scenes if they aren't the control scene
		loading = true;
		TryUnlockCursor();
		mapLoadText.text = "Unloading scenes";
		SetMapLoadProgress(0);
		mapLoadScreen.SetActive(true);
		playerExists = false;
		yield return null;//wait a frame before starting

		int sceneCount = SceneManager.sceneCount;
		float m = 0.5f / sceneCount;
		for (int i = sceneCount - 1; i >= 0; i--)
        {
			mapLoadText.text = "Unloading scene " + (sceneCount - i) + " of " + sceneCount;
			Scene scene = SceneManager.GetSceneAt(i);
			if(scene.name != controlSceneName)
            {
				AsyncOperation b = SceneManager.UnloadSceneAsync(scene);
                while (!b.isDone)
                {

					SetMapLoadProgress((sceneCount - i - 1) * m + b.progress * m);//TODO: test
					yield return null;
                }				
			}
			SetMapLoadProgress((sceneCount - i) * m);
			yield return null;
		}

		string path = mapScenePath + "/" + name;
		
		AsyncOperation a = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
		//don't load the scene fully
		a.allowSceneActivation = false;
		mapLoadText.text = "Loading scene...";
		//update the progress, but stop once the scene is ready (0.9)
		do
		{
			SetMapLoadProgress(0.5f + a.progress/2);
			yield return null;
		} while (a.progress < 0.9f);
		mapLoadText.text = "Activating Scene";
		SetMapLoadProgress(0.95f);
		//allow the last step and wait for it
		a.allowSceneActivation = true;
		yield return a;

		Scene toLoad = SceneManager.GetSceneByName(name);
		if (toLoad == null || toLoad.buildIndex == -1)
		{
			throw new Exception("toLoad scene is not good. Exists: " + (toLoad != null).ToString() + ". Path: " + path + ". Name: " + toLoad.name);
		}

		SceneManager.SetActiveScene(toLoad);

		//LoadPlayer(this);
		//TODO: save by map
		yield return LoadSavedStuffHelper();// Save.LoadAll();//load everything


		HideMenus();

		if(me == null) SetUpPlayer(CreatePlayerObject());

		playerExists = true;
		loading = false;
    }

	#endregion

	public static int NameToId(string s)
	{
		return StringIdMap[s];
	}

	#region Item Types

	private void InitializeItemTypes()
	{
		StringIdMap = new Dictionary<string, int>(itemTypes.Count);
		for(int i = 0; i < itemTypes.Count; i++)
		{
			//add to the dictionary to convert names to ids
			StringIdMap.Add(itemTypes[i].name, i);
		}
		StartCoroutine(LoadItemData());
	}

	private IEnumerator LoadItemData()
	{
		for (int i = 0; i < itemTypes.Count; i++)
		{

			ItemType item = itemTypes[i];

			//WARNING: Resources.Load returns null if not found
			//load data for this item type

			AsyncOperationHandle<GameObject> prefabAsync = Addressables.LoadAssetAsync<GameObject>(itemPath + item.name + "/" + item.name + " p.prefab");
			AsyncOperationHandle<GameObject> equipPrefabAsync = Addressables.LoadAssetAsync<GameObject>(itemPath + item.name + "/" + item.name + " e.prefab");
			AsyncOperationHandle<Sprite> iconAsync = Addressables.LoadAssetAsync<Sprite>(itemPath + item.name + "/" + item.name + " i.png");
			//wait for operations to complete
			if (!prefabAsync.IsDone) yield return prefabAsync;
			if (!equipPrefabAsync.IsDone) yield return equipPrefabAsync;
			if (!iconAsync.IsDone) yield return iconAsync;

			item.prefab = prefabAsync.Result;// Addressables.LoadAssetAsync<GameObject>(item.name + "/" + item.name + "-p");
			item.equipPrefab = equipPrefabAsync.Result;// Resources.Load<GameObject>(item.name + "/" + item.name + "-e");
			item.icon = iconAsync.Result;// Resources.Load<Sprite>(item.name + "/" + item.name + "-i");
			itemTypes[i] = item;
		}
		initialized = true;
		craftInventory.SetActive(true);
		GetComponent<Crafting>().InitializeUI();
		craftInventory.SetActive(false);
		//print("reached");
		yield return null;
	}

	#endregion

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

	void OnGUI()
	{
		if (me != null && myAbilities.dead)
		{// && me.ph.isMine
			Respawn();
		}
	}

	#region Mouse

	void TryLockCursor()
	{
		if (
			!craftInventory.activeSelf &&
			!mapScreen.activeSelf &&
			!tempUnlockMouse
			)
		{
			Cursor.lockState = CursorLockMode.Locked;
			middleCursor.SetActive(true);
		}
	}

	void TryUnlockCursor()
	{
		Cursor.lockState = CursorLockMode.None;
		middleCursor.SetActive(false);
	}

	private void CursorLockUpdate()
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
	}

	#endregion

	private void Update()
    {
		if (playerExists)
		{
			CursorLockUpdate();

			if (Input.GetKeyDown(KeyCode.E))
			{
				if (craftInventory.activeSelf)
				{
					craftInventory.SetActive(false);
					TryLockCursor();
				}
				else
				{
					craftInventory.SetActive(true);
					TryUnlockCursor();
				}
			}

			if (Input.GetKeyDown(KeyCode.M))
			{
				if (mapScreen.activeSelf)
				{
					mapScreen.SetActive(false);
					TryLockCursor();
				}
				else
				{
					mapScreen.SetActive(true);
					TryUnlockCursor();
				}
			}

			if (myAbilities.dead)
			{
				//deactivate crafting if dead
				if (craftInventory.activeSelf)
				{
					craftInventory.SetActive(false);
				}
				TryUnlockCursor();
			}
			else
			{
				LiveFunctions();
			}
		}
    }

    private void LiveFunctions()
	{
		if (!MenuActive())
		{
			//To make something collectible, a collider attached to it must match collectibleLayerMask
			RaycastHit hit;
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			bool foundSomething = Physics.Raycast(ray, out hit, Player.main.grabDist, collectibleLayerMask, QueryTriggerInteraction.Collide);
			bool foundCollectible = false;
			Collectible c = null;//apparantly this has to be initialized, even if it is guarenteed to be initialized later on before use

			if (foundSomething)
			{
				GameObject g = hit.collider.gameObject;
				c = g.GetComponentInParent<Collectible>();
				if (c != null)
				{
					foundCollectible = true;
					//print("click me");
					//c.MouseClickMe();
				}
			}

			itemHoverInfo.SetActive(foundCollectible);

			if (foundCollectible)
			{
				if(itemHoverPositionMatch) itemHoverInfo.transform.position = Input.mousePosition;
				if (Input.GetKey(KeyCode.F))
				{
					c.MouseClickMe();
				}
			}

			
		}
		if (Input.GetKey(KeyCode.F))
		{
			RaycastHit hit;
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Player.main.grabDist, collectibleLayerMask))
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

	public void SetUpPlayer(GameObject newPlayerObject)
	{
		SaveEntity save = newPlayerObject.GetComponent<SaveEntity>();
		save.playerOwnerName = username;

		me = newPlayerObject.GetComponent<Player>();
		HPBar hPBar = newPlayerObject.GetComponent<HPBar>();
		hPBar.hpBarImage = mainHpBar;//TODO: check taht this works
		hPBar.hpTextUI = mainHpText;
		hPBar.SetWorldHpBarVisible(false);

		Player.main = me;
		newPlayerObject.GetComponent<PlayerControl>().cam = camGameObject.GetComponentInChildren<Cam>();
		myAbilities = newPlayerObject.GetComponent<Abilities>();

		//bind hotbar to character and initialize
		hotBarUI.target = newPlayerObject.GetComponent<Inventory>();
		hotBarUI.InitializeSlots();

		if (Player.main == null) Debug.LogError("Failed to create main character");
		Inventory myInv = newPlayerObject.GetComponent<Inventory>();
		myInv.take = true;
		myInv.put = true;
		myInv.take = true;
		myInv.put = true;
	}

	GameObject CreatePlayerObject()
	{
		Vector3 position;
		spawnPoint[] sp = GameObject.FindObjectsOfType<spawnPoint> ();
		int chosen = UnityEngine.Random.Range (0, sp.Length);
		position = sp [chosen].transform.position;

		//GameObject newPlayerObject = Instantiate(player, position, Quaternion.identity);
		return Instantiate(player, position, Quaternion.identity);

		//Save save = newPlayerObject.GetComponent<Save>();
		//save.playerOwnerName = username;

		//me = newPlayerObject.GetComponent<Player> ();
		//HPBar hPBar = newPlayerObject.GetComponent<HPBar>();
		//hPBar.hpBarImage = mainHpBar;//TODO: check taht this works
		//hPBar.hpTextUI = mainHpText;
		//hPBar.SetWorldHpBarVisible(false);

		//Player.main = me;
		//newPlayerObject.GetComponent<PlayerControl>().cam = camGameObject.GetComponentInChildren<Cam>();
		//myAbilities = newPlayerObject.GetComponent<Abilities>();

		////bind hotbar to character and initialize
		//hotBarUI.target = newPlayerObject.GetComponent<Inventory>();
		//hotBarUI.InitializeSlots();

		//if (Player.main == null) Debug.LogError("Failed to create main character");
		//Inventory myInv = newPlayerObject.GetComponent<Inventory>();
		//myInv.take = true;
		//myInv.put = true;
		//myInv.take = true;
		//myInv.put = true;
	}

	#region save
	public static void SavePlayer(GameControl p)
	{
		Crafting crafting = p.GetComponent<Crafting>();
		PlayerSaveData s = new PlayerSaveData()
		{
			username = username,
			craftInventoryItems = crafting.craftInventory.items,
		};
		string path1 = Authenticator.GetAccountPath(username);
		if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);//TODO: warning this is bad, allows making account folders without registering
		File.WriteAllText(path1 + "data.json", JsonConvert.SerializeObject(s, Formatting.Indented));
		Debug.Log("Saved player data for username: " + username);
	}

	public static void LoadPlayer(GameControl p)
	{
		Crafting crafting = p.GetComponent<Crafting>();
		string path = Authenticator.GetAccountPath(username) + "data.json";
		if (File.Exists(path))
		{
			PlayerSaveData s = JsonConvert.DeserializeObject<PlayerSaveData>(File.ReadAllText(path));
			if (username != s.username) Debug.LogError("Username doesn't match, current name: " + username + ", saved: " + s.username);
			crafting.craftInventory.items = s.craftInventoryItems;
			print("Loaded player: " + username);
		}

	}

	public static void CheckItemTypes()
	{
		if (itemTypes == null)
		{
			if (File.Exists(itemTypePath))
			{
				Debug.Log("read ItemTypes");
				itemTypes = JsonConvert.DeserializeObject<ItemType[]>(File.ReadAllText(itemTypePath)).ToList();
			}
			else
			{
				itemTypes = new List<ItemType>();
				itemTypes.Add(new ItemType());
				Debug.LogError("No itemtypes list, made a new one");
			}
		}
	}

	public static void SaveItemTypes()
	{
		File.WriteAllText(itemTypePath, JsonConvert.SerializeObject(itemTypes.ToArray(), Formatting.Indented));
		Debug.Log("Saved ItemTypes");
	}

	public void LoadSavedStuff()
	{
		StartCoroutine(LoadSavedStuffHelper());
	}

	public IEnumerator LoadSavedStuffHelper()
	{
		LoadPlayer(this);
		yield return SaveItem.LoadAll();//load items (inc. buildings maybe) first
		yield return SaveEntity.LoadAll();
		print("loaded saved entities and items...");
	}

	public void SaveStuff()
	{
		SavePlayer(this);
		SaveEntity.SaveAll();
		SaveItem.SaveAll();
		print("saved entities and items...");
	}

	#endregion

	private void OnApplicationQuit()
	{
		SaveStuff();
	}
}

[System.Serializable]
public class PlayerSaveData
{
	public string username;
	public List<Item> craftInventoryItems;
}
