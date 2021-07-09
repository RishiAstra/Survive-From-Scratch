/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using UnityEngine;
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
using UnityEngine.EventSystems;
using Newtonsoft.Json.Linq;
//TODO: this class can do player actions unique to the player being controlled by this client in multiplayer, especially because this class knows which player is this client's player.
public class GameControl : MonoBehaviour
{
	public const int MAP_SCENE_INDEX_START = 1;

	public static string saveDirectory { 
		get { return Application.persistentDataPath + "/" + Application.version + "/"; }
	}
	public static string playerCharacterDirectory
	{
		get { return saveDirectory + "player characters/"; }
	}
	//public static string playerSavePath = Application.persistentDataPath + "/Players/";
	public static string itemTypePath = Application.streamingAssetsPath + @"/Items/item types.json";//@"Assets\Resources\item types.json";
	public const string itemPath = @"Assets/Items/";
	private const string ASSETS_FOLDER_NAME = "Assets/";

	//public static string versionFilePath = Application.streamingAssetsPath + @"version.txt";


	public static byte[] sessionId;
	public static string username = "default";

	public static bool initialized;

	public static bool inWorld;//are you in the world? used to know if can spawn player

	public static Camera mainCamera;

	//public static int curserFreeCount = 0;//use this to prevent the cursor from locking
	public static bool tempUnlockMouse;
	public static bool loading;//true if currently loading a scene
	public static bool loadedLocation;//is the main character existant yet? won't be if loading a scene or start screen etc

	public static List<ItemType> itemTypes;
	public static Dictionary<string, int> StringIdMap;

	public static GameControl main;

	public string serverIp;
	public string mapScenePath;
	public string controlSceneName;

	public string defaultMapLocation;
	public ItemInfoUI mainItemInfoUI;
	public int money;
	public TextMeshProUGUI moneyText;
	public TextMeshProUGUI mapLoadText;
	public RectTransform mapLoadBar;
	public Menu mapLoadScreen;
	public Menu mapScreen;
	public float grabDist = 4f;
	//private AsyncOperation mapSceneProg;
	private float mapLoadBarInitialWidth;
	private float mapSceneLoadProgress;

	//public LayerMask collectibleLayerMask;
	public LayerMask interactLayerMask;
	public LayerMask ground;
	public GameObject playerPrefab;
	public GameObject playerPrefab2;
	public bool usePlayerPrefab2;
	public GameObject camPref;
	public Transform camPos;//start the camera here
	public InventoryUI hotBarUI;
	public InventoryUI mainInventoryUI;
	public Menu craftInventory;
	public Menu helpMenu;
	public Menu shopMenu;
	public GameObject middleCursor;
	public GameObject itemHoverInfo;
	public TextMeshProUGUI itemHoverNameText;
	[Tooltip("Should the item hover info be on top of the item, or stay in it's position?")]
	public bool itemHoverPositionMatch;

	//public Image mainHpBar;
	//public TextMeshProUGUI mainHpText;
	//public Image mainMpBar;
	//public TextMeshProUGUI mainMpText;
	//public Image mainEngBar;
	//public TextMeshProUGUI mainEngText;

	//public Image mainXpBar;
	//public TextMeshProUGUI mainXpText;
	public TextMeshProUGUI mainLvlText;

	public HPBar mainHp;
	public HPBar mainMp;
	public HPBar mainEng;
	public HPBar mainXp;


	public Canvas mainCanvas;
	public Vector2 mouseSensitivity;	
	public float scrollSensitivity;
	[Tooltip("Used to show information about the item type")]public RectTransform itemInfoTransform;

	public GameObject camGameObject;
	public int savedSceneIndex = -1;
	public GameObject loadingLastSaveScreen;
	
//	public RPGCamera Camera;
	//private Player me;
	public NPCControl playerControl;
	//private long myPlayersId = -1;
	public Party myParty = new Party();
	public IMouseHoverable previouslyMouseHovered;
	public RectTransform itemInfoTarget;
	[HideInInspector] public Abilities myAbilities;

	private bool waitBeforeInteractEnabled;


	public Color armorColor, atkColor, hpColor, mpColor, engColor, morColor, xpColor;
	public Transform mapPlayerIcon;
	public GameObject partyMemberUIPrefab;
	public Transform partyMemberUIParent;
	public Transform UIPositionMatchersParent;

	public void RefreshPartyMemberUI()
	{
		//delete previous
		for(int i = partyMemberUIParent.childCount - 1; i >= 0; i--)
		{
			Destroy(partyMemberUIParent.GetChild(i).gameObject);
		}

		for (int i = 0; i < myParty.members.Count; i++)
		{
			PartyMember p = myParty.members[i];
			GameObject g = Instantiate(partyMemberUIPrefab, partyMemberUIParent);
			CharacterDisplayer characterDisplayer = g.GetComponent<CharacterDisplayer>();
			characterDisplayer.SetTarget(p);
			characterDisplayer.indexText.text = (i + 1).ToString();
		}
	}

	void Awake(){
		ItemInfoUI.main = mainItemInfoUI;
		if (usePlayerPrefab2) playerPrefab = playerPrefab2;
		if (main != null) Debug.LogError("two gameControls");
		main = this;
		TryUnlockCursor();
		mapLoadBarInitialWidth = mapLoadBar.sizeDelta.x;
		camGameObject = Instantiate(camPref, camPos.position, camPos.rotation);
		mainCamera = Camera.main;
		//craftInventory.SetActive(false);

		itemHoverInfo.SetActive(false);
		HideMenus();
		//mapScreen.TryActivateMenu();
		//helpMenu.TryActivateMenu();

		//TODO: show help if first time playing

		//mapScreen.SetActive(true);
		HideInfo();
		//TODO: consider setting mapScreen to active
		CheckItemTypes();
		//InitializeItemTypes();
		Save.Initialize();

		StartCoroutine(LoadItemDataAndPlayer());
		//StartCoroutine(LoadItemData());
		//StartCoroutine(LoadPlayerInitial());
	}

	IEnumerator LoadItemDataAndPlayer()
	{
		TimeControl.main.SetTimeScale(0, "INITIAL_PLAYER_AND_ITEM_DATA");
		loadingLastSaveScreen.SetActive(true);
		//print("loading......");
		yield return LoadItemData();
		yield return LoadPlayerInitial();
		loadingLastSaveScreen.SetActive(false);
		TimeControl.main.RemoveTimeScale("INITIAL_PLAYER_AND_ITEM_DATA");
	}

	public void ShowInfo(Item i, RectTransform target)
	{
		//StartCoroutine(ShowItemIEnumerator(i, target));

		itemInfoTransform.gameObject.SetActive(true);
		itemInfoTarget = target;//this must have a iteminfoUI on it!
		ItemInfoUI.main.SetInfo(i);
		itemInfoTransform.ForceUpdateRectTransforms();
		//itemInfoTransform.gameObject.SetActive(true);

		float scale = mainCanvas.scaleFactor;//.GetComponent<CanvasScaler>().scaleFactor;

		Vector2 position = target.position;
		Vector2 screensize = new Vector2(Screen.width, Screen.height);// * scale;// / scale;// * mainCanvas.scaleFactor;mainCanvas.GetComponent<CanvasScaler>().referenceResolution;// 
		Vector2 infosize = itemInfoTransform.sizeDelta * itemInfoTransform.lossyScale;
		Vector2 targetsize = target.sizeDelta * target.lossyScale;
		Vector2 size = infosize + targetsize;


		//these bools represent if the info text would fit there
		float right = position.x + size.x / 2f;
		float top = position.y + size.y / 2f;
		float left = position.x - size.x / 2f;
		float bottom = position.y - size.y / 2f;

		//the amount that it would overlap
		float ro = (right + infosize.x / 2f) - screensize.x;
		float to = (top + infosize.y / 2f) - screensize.y;
		float lo = -(left - infosize.x / 2f);
		float bo = -(bottom - infosize.y / 2f);

		bool rightoverlap = ro > 0f;// right + infosize.x / 2f> screensize.x;
		bool topoverlap = to > 0f;// top + infosize.y / 2f > screensize.y;
		bool leftoverlap = lo > 0f;// left - infosize.x / 2f < 0f;
		bool bottomoverlap = bo > 0f;// bottom - infosize.y / 2f < 0f;

		//print(position + "|" + targetsize + "|" + scale + "|" + rightoverlap + "|" + topoverlap + "|" + leftoverlap + "|" + bottomoverlap);


		//if the text should be placed to the bottom right of the target
		bool chooseRight = true;
		bool chooseBottom = true;

		if (rightoverlap && !leftoverlap) chooseRight = false;
		if (bottomoverlap && !topoverlap) chooseBottom = false;



		//make the final position based on the previous calculations
		Vector2 pos = new Vector2();
		if (chooseRight)
		{
			pos.x = right;
		}
		else
		{
			pos.x = left;
		}
		if (chooseBottom)
		{
			pos.y = bottom;
		}
		else
		{
			pos.y = top;
		}


		if (rightoverlap && leftoverlap)
		{
			if (ro > lo)
			{
				pos.x = infosize.x / 2f;
			}
			else
			{
				pos.x = screensize.x - infosize.x / 2f;
			}

		}

		if (topoverlap && bottomoverlap)
		{
			if (to > bo)
			{
				pos.y = infosize.y / 2f;
			}
			else
			{
				pos.y = screensize.y - infosize.y / 2f;
			}
		}

		//assign this position
		itemInfoTransform.position = pos;
	}

	private IEnumerator ShowItemIEnumerator(Item i, RectTransform target)
	{
		itemInfoTransform.gameObject.SetActive(false);
		itemInfoTarget = target;//this must have a iteminfoUI on it!
		ItemInfoUI.main.SetInfo(i);
		yield return null;//wait 1 frame
		itemInfoTransform.gameObject.SetActive(true);

		float scale = mainCanvas.scaleFactor;//.GetComponent<CanvasScaler>().scaleFactor;

		Vector2 position = target.position;
		Vector2 screensize = new Vector2(Screen.width, Screen.height);// * scale;// / scale;// * mainCanvas.scaleFactor;mainCanvas.GetComponent<CanvasScaler>().referenceResolution;// 
		Vector2 infosize = itemInfoTransform.sizeDelta * itemInfoTransform.lossyScale;
		Vector2 targetsize = target.sizeDelta * target.lossyScale;
		Vector2 size = infosize + targetsize;


		//these bools represent if the info text would fit there
		float right = position.x + size.x / 2f;
		float top = position.y + size.y / 2f;
		float left = position.x - size.x / 2f;
		float bottom = position.y - size.y / 2f;

		//the amount that it would overlap
		float ro = (right + infosize.x / 2f) - screensize.x;
		float to = (top + infosize.y / 2f) - screensize.y;
		float lo = -(left - infosize.x / 2f);
		float bo = -(bottom - infosize.y / 2f);

		bool rightoverlap = ro > 0f;// right + infosize.x / 2f> screensize.x;
		bool topoverlap = to > 0f;// top + infosize.y / 2f > screensize.y;
		bool leftoverlap = lo > 0f;// left - infosize.x / 2f < 0f;
		bool bottomoverlap = bo > 0f;// bottom - infosize.y / 2f < 0f;

		//print(position + "|" + targetsize + "|" + scale + "|" + rightoverlap + "|" + topoverlap + "|" + leftoverlap + "|" + bottomoverlap);


		//if the text should be placed to the bottom right of the target
		bool chooseRight = true;
		bool chooseBottom = true;

		if (rightoverlap && !leftoverlap) chooseRight = false;
		if (bottomoverlap && !topoverlap) chooseBottom = false;



		//make the final position based on the previous calculations
		Vector2 pos = new Vector2();
		if (chooseRight)
		{
			pos.x = right;
		}
		else
		{
			pos.x = left;
		}
		if (chooseBottom)
		{
			pos.y = bottom;
		}
		else
		{
			pos.y = top;
		}


		if (rightoverlap && leftoverlap)
		{
			if (ro > lo)
			{
				pos.x = infosize.x / 2f;
			}
			else
			{
				pos.x = screensize.x - infosize.x / 2f;
			}

		}

		if (topoverlap && bottomoverlap)
		{
			if (to > bo)
			{
				pos.y = infosize.y / 2f;
			}
			else
			{
				pos.y = screensize.y - infosize.y / 2f;
			}
		}

		//assign this position
		itemInfoTransform.position = pos;
	}

	public void HideInfo(RectTransform target)
	{
		if(itemInfoTarget == target)
		{
			HideInfo();
		}
	}

	private void HideInfo()
	{
		itemInfoTarget = null;
		itemInfoTransform.gameObject.SetActive(false);
	}

	IEnumerator LoadPlayerInitial()
	{
		yield return new WaitForEndOfFrame();
		LoadPlayer();
		print("loaded player initial");


		//load the previous location or default if none
		string sceneName = defaultMapLocation;
		if (savedSceneIndex >= MAP_SCENE_INDEX_START && savedSceneIndex < SceneManager.sceneCountInBuildSettings)
		{
			sceneName = GetSceneNameFromIndex(savedSceneIndex);
		}

		yield return LoadMapLocation(sceneName);

		//load/spawn party here. Make sure that if they party isn't in this location, they are teleported
		//since this is initial, consider just keeping the whole party where they are
		//if they are in this location, load them
		//if(me == null) MakeAndSetUpPlayer(false, false);
		//MakeAndSetUpPlayer(false, false);

		yield return LoadPartyFromData(false, false);

		//for some reason ProgressTracker.main.prog.currentDialoguePart won't be null, but it will be a new and empty instance
		//therefore check that the title and texts exist
		if (ProgressTracker.main.prog.currentDialoguePart != null && 
			!string.IsNullOrEmpty(ProgressTracker.main.prog.currentDialoguePart.title)&&
			ProgressTracker.main.prog.currentDialoguePart.texts != null)
		{
			DialogueControl.main.StartDialoguePart(ProgressTracker.main.prog.currentDialoguePart, ProgressTracker.main.prog.currentDialoguePartSource);
		}


		print("loaded map location initial: " + sceneName + "," + savedSceneIndex);
	}

	private IEnumerator LoadPartyFromData(bool respawn, bool resetStats)
	{
		if (myParty.members.Count == 0)
		{

			yield return AddNewCharacterToParty("Player");

			//GameObject g = Instantiate(playerPrefab);

			//g.GetComponent<NPCControl>().playerControlled = true;

			//PartyMember p = new PartyMember()
			//{
			//	name = "Player",
			//	type = playerPrefab.name,
			//	id = g.GetComponent<SaveEntity>().id,
			//	g = g,//not a self-assignment
			//	control = g.GetComponent<NPCControl>(),
			//};
			//myParty.members.Add(p);
			//myParty.lastUsed = 0;

			//RespawnPartyMemberPosition(g);
		}
		else
		{

			foreach (PartyMember p in myParty.members)
			{
				//if party member gameobject exists, delete it (note: saveentity will auto-save it)
				if (p.g != null) Destroy(p.g);

				//string datapath = playerCharacterDirectory + p.id + "/";

				if (SaveEntity.playerEntities.Contains(p.id))
				{
					//get the gameobject to spawn
					GameObject toSpawn;
					bool succeed = SaveEntity.GetEntityPrefabCached(p.type, out toSpawn);
					if (!succeed)
					{
						AsyncOperationHandle<GameObject> a = SaveEntity.GetEntityPrefab(p.type);
						yield return a;
						toSpawn = a.Result;
					}


					JArray saveData = SaveEntity.GetPlayerDataFromFilePath(p.id.ToString());

					GameObject g = SaveEntity.LoadEntity(toSpawn, saveData);
					p.g = g;
					p.control = g.GetComponent<NPCControl>();
					SaveEntity saveEntity = g.GetComponent<SaveEntity>();


					//if not respawning the position and character is in wrong scene, respawn the position to prevent wierd wrong positions
					if (!respawn && saveEntity.savedSceneBuildIndex != SceneManager.GetActiveScene().buildIndex)
					{
						RespawnPartyMemberPosition(g);
					}

					//mark as player owned
					saveEntity.playerOwned = true;
					saveEntity.deleteOnDeath = false;

					if (resetStats)
					{
						StatScript ss = g.GetComponent<StatScript>();

						if (ss != null)
						{
							ss.ResetStats();
						}
					}

					if (respawn)
					{
						RespawnPartyMemberPosition(g);
					}

				}
				else
				{
					Debug.LogError("Party member gameobject null. id: " + p.id);
				}
			}
		}

		//bounds check
		if (myParty.lastUsed < 0) myParty.lastUsed = 0;
		if (myParty.lastUsed >= myParty.members.Count) myParty.lastUsed = myParty.members.Count - 1;

		SetControlledPartyMember(myParty.lastUsed);
		RefreshPartyMemberUI();
	}

	public void StartAddNewCharacterToParty(string type, bool specificPosition = false, Vector3 pos = new Vector3(), Vector3 rot = new Vector3())
	{
		StartCoroutine(AddNewCharacterToParty(type, specificPosition, pos, rot));
	}

	public IEnumerator AddNewCharacterToParty(string type, bool specificPosition = false, Vector3 pos = new Vector3(), Vector3 rot = new Vector3())
	{
		GameObject toSpawn;
		bool succeed = SaveEntity.GetEntityPrefabCached(type, out toSpawn);
		if (!succeed)
		{
			AsyncOperationHandle<GameObject> a = SaveEntity.GetEntityPrefab(type);
			yield return a;
			toSpawn = a.Result;
		}

		GameObject g = Instantiate(toSpawn);

		//g.GetComponent<NPCControl>().playerControlled = true;
		g.GetComponent<SaveEntity>().playerOwned = true;
		g.GetComponent<SaveEntity>().deleteOnDeath = false;

		PartyMember p = new PartyMember()
		{
			name = type,
			type = type,//not a self-assignment
			id = g.GetComponent<SaveEntity>().id,
			g = g,//not a self-assignment
			control = g.GetComponent<NPCControl>(),
		};
		myParty.members.Add(p);
		myParty.lastUsed = 0;
		if (specificPosition)
		{
			g.transform.position = pos;
			g.transform.eulerAngles = rot;
		}
		else
		{
			RespawnPartyMemberPosition(g);
		}
		RefreshPartyMemberUI();

		yield return null;
	}

	private void RespawnPartyMemberPosition(GameObject g)
	{
		Vector3 position;
		spawnPoint[] sp = GameObject.FindObjectsOfType<spawnPoint>();
		int chosen = UnityEngine.Random.Range(0, sp.Length);
		position = sp[chosen].transform.position;

		g.transform.position = position;
		g.transform.rotation = sp[chosen].transform.rotation;	
	}

	private void UnsetControlledPartyMember(int index)
	{

		GameObject g = myParty.members[index].g;
		if (g == null) Debug.LogError("No gameobject for this party member to be uncontrolled");

		g.GetComponent<NPCControl>().cam = null;
		g.GetComponent<NPCControl>().playerControlled = false;
		g.GetComponent<HPBar>().SetWorldHpBarVisible(true);
	}

	public void SetControlledPartyMember(int index)
	{
		for(int i = 0; i < myParty.members.Count; i++)
		{
			//return to AI-control etc. if character is on field
			if (myParty.members[i].g != null) UnsetControlledPartyMember(i);
		}

		myParty.lastUsed = index;

		GameObject g = myParty.members[index].g;
		if (g == null) Debug.LogError("No gameobject for this party member to be controlled");


		//me = g.GetComponent<NPCControl>();


		playerControl = myParty.members[index].control;// g.GetComponent<NPCControl>();

		StatScript stat = g.GetComponent<StatScript>();
		g.GetComponent<HPBar>().SetWorldHpBarVisible(false);

		mainHp.SetTarget(stat);
		mainMp.SetTarget(stat);
		mainEng.SetTarget(stat);
		mainXp.SetTarget(stat);

		/*
		//HPBar hpBar = g.GetComponent<HPBar>();
		//hpBar.hpBarImage = mainHpBar;//TODO: check taht this works
		//hpBar.hpTextUI = mainHpText;
		//hpBar.SetWorldHpBarVisible(false);

		//HPBar mpBar = newPlayerObject.AddComponent<HPBar>();
		//mpBar.type = HPBar.StatType.mp;
		//mpBar.hpBarImage = mainMpBar;
		//mpBar.hpTextUI = mainMpText;
		//mpBar.autoHide = false;
		//mpBar.changeHpBarColor = false;
		//mpBar.useNewLine = true;

		//HPBar engBar = newPlayerObject.AddComponent<HPBar>();
		//engBar.type = HPBar.StatType.eng;
		//engBar.hpBarImage = mainEngBar;
		//engBar.hpTextUI = mainEngText;
		//engBar.autoHide = false;
		//engBar.changeHpBarColor = false;
		//engBar.useNewLine = true;

		//HPBar xpBar = newPlayerObject.AddComponent<HPBar>();
		//xpBar.type = HPBar.StatType.xp;
		//xpBar.hpBarImage = mainXpBar;
		//xpBar.hpTextUI = mainXpText;
		//xpBar.autoHide = false;
		//xpBar.changeHpBarColor = false;
		*/

		//playerControl = me;
		//Player.main = me;
		g.GetComponent<NPCControl>().cam = camGameObject.GetComponentInChildren<Cam>();
		g.GetComponent<NPCControl>().playerOwnerName = username;
		g.GetComponent<NPCControl>().playerControlled = true;
		//print("set up player camera");
		myAbilities = g.GetComponent<Abilities>();

		g.GetComponent<SaveEntity>().playerOwned = true;

		//bind hotbar to character and initialize
		hotBarUI.target = g.GetComponent<Inventory>();
		hotBarUI.InitializeSlots();

		//bind inventory to character and initialize
		mainInventoryUI.target = mainInventoryUI.GetComponent<Inventory>();
		mainInventoryUI.InitializeSlots();


		Inventory myInv = g.GetComponent<Inventory>();
		myInv.take = true;
		myInv.put = true;
		myInv.take = true;
		myInv.put = true;

	}

	private static string GetSceneNameFromIndex(int BuildIndex)
	{
		string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
		int i = path.IndexOf(ASSETS_FOLDER_NAME);
		if(i == 0)
		{
			path = path.Substring(ASSETS_FOLDER_NAME.Length);
		}
		if (path.IndexOf(main.mapScenePath) != 0)
		{
			Debug.LogError("map scene doesn't exist: " + path);
		}
		string name = path.Substring(main.mapScenePath.Length);
		int dot = name.LastIndexOf('.');
		return name.Substring(0, dot);

		//int slash = path.LastIndexOf('/');
		//string name = path.Substring(slash + 1);

	}

	private void HideMenus()
	{
		mapLoadScreen.TryDeactivateMenu();
		mapScreen.TryDeactivateMenu();
		craftInventory.TryDeactivateMenu();
		shopMenu.TryDeactivateMenu();
		helpMenu.TryDeactivateMenu();
		HideInfo();
		//mapLoadScreen.SetActive(false);
		//mapScreen.SetActive(false);
		//craftInventory.SetActive(false);
	}

	
	/// <summary>
	/// returns if any menu is open (crafting, map, etc.)
	/// </summary>
	/// <returns></returns>
	private bool MenuActive()
	{
		return Menu.openMenuCount > 0;
		
		//return
		//	mapLoadScreen.activeInHierarchy ||
		//	mapScreen.activeInHierarchy ||
		//	craftInventory.activeInHierarchy;
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

	//used by buttons
	public void BeginLoadMapLocation(string sceneName)
	{
		StartCoroutine(LoadMapLocationAndResetPosition(sceneName));
	}

	IEnumerator LoadMapLocationAndResetPosition(string sceneName)
	{
		yield return LoadMapLocation(sceneName);

		//reset party positions here

		yield return LoadPartyFromData(true, false);


		//if (me == null)
		//{
			//MakeAndSetUpPlayer(true, false);
		//}
		//else
		//{
		//	//TODO:warning: this supposes that the player component is on the root of player gameobject
		//	ResetPositionOfPlayer(me.gameObject);
		//}
	}

	public IEnumerator LoadMapLocation(string sceneName)
	{
		if (!initialized) yield break;
		//TODO:test this
		TimeControl.main.SetTimeScale(0, "MAP_LOAD");
		//remove other scenes if they aren't the control scene
		loading = true;
		TryUnlockCursor();
		mapLoadText.text = "Saving";
		yield return null;//wait a frame
		SaveStuff();
		//yield return null;
		
		yield return null;//wait a frame
		mapLoadText.text = "Unloading scenes";
		SetMapLoadProgress(0);
		mapLoadScreen.TryActivateMenu();
		mapScreen.TryDeactivateMenu();
		//mapLoadScreen.SetActive(true);
		loadedLocation = false;
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

		string path = mapScenePath + sceneName;

		//myParty.TeleportAll(sceneName);

		//if (myPlayersId == -1)
		//{
		//	Debug.LogWarning("Didn't teleport player because player doesn't exist");
		//}
		//else
		//{
		//	SaveEntity.TeleportEntityBetweenScenes(myPlayersId, SceneUtility.GetBuildIndexByScenePath(path));
		//}

		yield return null;

		
		
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

		//Scene toLoad = SceneManager.GetSceneByName(name);//TODO: warning, this will fail if the scene isn't loaded
		int toLoad = SceneUtility.GetBuildIndexByScenePath(path);
		//if (toLoad == null || toLoad.buildIndex == -1)
		//{
		//	throw new Exception("toLoad scene is not good. Exists: " + (toLoad != null).ToString() + ". Path: " + path + ". Name: " + toLoad.name);
		//}
		if (toLoad == -1)
		{
			throw new Exception("toLoad scene is not good. Index: " + toLoad + ". Path: " + path);
		}

		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(toLoad));

		//LoadPlayer(this);
		//TODO: save by map
		yield return LoadSavedStuffHelper();// Save.LoadAll();//load everything


		HideMenus();

		//if(me == null)
		//{
		//	//if (respawn)
		//	//{
		//	//	MakeAndSetUpPlayer(true, true);
		//	//}
		//	//else
		//	//{

		//	//teleport but don't restore
		//	MakeAndSetUpPlayer(true, false);
		//	//}
		//}

		TimeControl.main.RemoveTimeScale("MAP_LOAD");
		loadedLocation = true;
		inWorld = true;
		loading = false;

		ProgressTracker.main.RegisterSceneEntry(sceneName);

		print("loaded map location");
	}

	#endregion

	public static int NameToId(string s)
	{
		if (StringIdMap == null) InitializeItemTypeMap();
		return StringIdMap[s];
	}

	#region Item Types
	public static void InitializeItemTypeMap()
	{
		StringIdMap = new Dictionary<string, int>(itemTypes.Count);
		for (int i = 0; i < itemTypes.Count; i++)
		{
			//add to the dictionary to convert names to ids
			StringIdMap.Add(itemTypes[i].name, i);
		}
	}

	//private void InitializeItemTypes()
	//{
	//	StartCoroutine(LoadItemData());
	//}

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
		craftInventory.TryActivateMenu();
		//craftInventory.SetActive(true);
		Crafting.main.InitializeUI();
		craftInventory.TryDeactivateMenu();
		//craftInventory.SetActive(false);
		//print("reached");
		yield return null;
	}

	#endregion

	//void OnJoinedRoom()
	//{
	//	CreatePlayerObject();
	//}
	//void MakeAndSetUpPlayer(bool respawnPosition = false, bool restoreStats = false)
	//{
		
	//	CreatePlayerObject(respawnPosition, restoreStats);
	//	//SetUpPlayer(CreatePlayerObject());//setupplayer is called by the new player PlayerControl.SetData

	//}

	void RespawnGUIOption()
	{
		//TODO: clear all statis effects, maybe just delete player and spawn a new one
		//myAbilities.Reset();
		if (GUI.Button(new Rect((Screen.width - 100) / 2, (Screen.height - 25) / 2, 100, 25), "Respawn"))
		{
			Respawn();
			//me.gameObject.SetActive(true);
			//me.Respawn();
		}
	}

	private void Respawn()
	{
		SaveStuff();
		//respawn party
		StartCoroutine(LoadPartyFromData(true, true));
		//MakeAndSetUpPlayer(true, true);
	}

	void OnGUI()
	{
		//TODO: probably won't work with multiple player characters
		//addressing the above todo, this has been redesigned to support multiple players and swap between them unless all are dead
		if (inWorld && loadedLocation && playerControl == null) {

			bool otherPartyMemberActivated = false;

			for (int i = 0; i < myParty.members.Count; i++)
			{
				if (myParty.members[i].g != null)
				{
					SetControlledPartyMember(i);
					otherPartyMemberActivated = true;
					break;
				}
			}

			if (!otherPartyMemberActivated)
			{
				TryUnlockCursor();
				RespawnGUIOption();
			}
		}
	}

	#region Mouse

	public void TryLockCursor()
	{
		if (Menu.openMenuCount == 0 &&
			//!craftInventory.activeSelf &&
			//!mapScreen.activeSelf &&
			!tempUnlockMouse
			)
		{
			PointerEventData data = new PointerEventData(EventSystem.current);
			data.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			mainCanvas.GetComponent<GraphicRaycaster>().Raycast(data, results);
			if(results.Count == 0)
			{
				Cursor.lockState = CursorLockMode.Locked;
				middleCursor.SetActive(true);
			}
			else
			{
				//print("failed to lock mouse: over ui");
			}
		}
	}

	public void TryUnlockCursor()
	{
		Cursor.lockState = CursorLockMode.None;
		middleCursor.SetActive(false);
	}

	private void CursorLockUpdate()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
		{
			tempUnlockMouse = true;
			TryUnlockCursor();
		}
		//TODO: this glitches when ctrl to mouse exit, then mouse enter it stuck till ctrl again
		bool ctrlReleased = Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl);
		bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		bool releaseBecauseMouseClickAndNotKey = !ctrlHeld && Input.GetMouseButtonDown(0);
		if (tempUnlockMouse && (ctrlReleased || releaseBecauseMouseClickAndNotKey))
		{
			tempUnlockMouse = false;
			TryLockCursor();
		}

		if (Cursor.lockState == CursorLockMode.Locked)
		{

			//if (Input.GetKeyDown(KeyCode.Escape))
			//{
			//	TryUnlockCursor();
			//}
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

	public void Wait1FrameBeforeInteract()
	{
		waitBeforeInteractEnabled = true;
	}

	private void LateUpdate()
	{
		waitBeforeInteractEnabled = false;
	}

	private void Update()
	{
		moneyText.text = "Money: " + money;
		if((itemInfoTarget == null || itemInfoTarget.gameObject.activeInHierarchy == false) && itemInfoTransform.gameObject.activeSelf == true) HideInfo();
		if (loadedLocation)
		{
			CursorLockUpdate();

			if (Input.GetKeyDown(KeyCode.E) && !Menu.IsOtherMenuActive(craftInventory))
			{
				//if (inWorld && MenuActive() && !craftInventory.gameObject.activeSelf)
				//{
				//	HideMenus();
				//}
				//else
				//{
					craftInventory.ToggleMenu();
					if (!craftInventory.gameObject.activeSelf)
					{
						//try transfering to the hotbar first, then to the main inventory
						Inventory.TransferAllItems(Crafting.main.craftInventory, hotBarUI.target);
						Inventory.TransferAllItems(Crafting.main.craftInventory, mainInventoryUI.target);
					}
				//}				
				
				
				//if (craftInventory.activeSelf)
				//{
				//	craftInventory.SetActive(false);
				//	TryLockCursor();
				//}
				//else
				//{
				//	craftInventory.SetActive(true);
				//	TryUnlockCursor();
				//}
			}

			if (Input.GetKeyDown(KeyCode.M) && !Menu.IsOtherMenuActive(mapScreen))
			{
				//if (!mapScreen.gameObject.activeSelf) HideMenus();
				mapScreen.ToggleMenu();

				//if (mapScreen.activeSelf)
				//{
				//	mapScreen.SetActive(false);
				//	TryLockCursor();
				//}
				//else
				//{
				//	mapScreen.SetActive(true);
				//	TryUnlockCursor();
				//}
			}

			//TODO:check this, party stuff might have messed it up
			if (myAbilities != null && myAbilities.myStat.dead)
			{
				//TODO: MOVE ON TO THE NEXT PARTY MEMBER
							


				//craftInventory.TryDeactivateMenu();
				////deactivate crafting if dead
				////if (craftInventory.activeSelf)
				////{
				////	craftInventory.SetActive(false);
				////}
				//TryUnlockCursor();
			}
			else
			{
				LiveFunctions();
			}



			//TODO:REMOVE THIS
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.RightArrow))
			{
				myAbilities.myStat.GiveXp(1000);
			}

			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.UpArrow)) money += 5000;
			if(myAbilities != null) mainLvlText.text = "Level " + myAbilities.myStat.lvl;
			//TODO:REMOVE ABOVE

		}
	}

	private void LiveFunctions()
	{
		//null check because interface isn't set to null when destroyed
		UnityEngine.Object o = previouslyMouseHovered as UnityEngine.Object;
		if (o == null || o.Equals(null)) previouslyMouseHovered = null;
		if (!waitBeforeInteractEnabled && !MenuActive() && Cursor.lockState == CursorLockMode.Locked)
		{
			//To make something collectible, a collider attached to it must match collectibleLayerMask
			RaycastHit hit;
			Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));// mainCamera.ScreenPointToRay(Input.mousePosition);
			bool foundSomething = Physics.Raycast(ray, out hit, grabDist + playerControl.cam.dist, interactLayerMask, QueryTriggerInteraction.Collide);
			if (hit.distance < playerControl.cam.dist) foundSomething = false;//this means that the item was found by placing it between the player and the camera. This is abusing camera distance to grab stuff from futher distances
			//bool foundIMouseHoverable = false;
			IMouseHoverable c = null;//apparantly this has to be initialized, even if it is guarenteed to be initialized later on before use

			if (foundSomething)
			{
				GameObject g = hit.collider.gameObject;
				c = g.GetComponentInParent<IMouseHoverable>();

				//tell the previous one that it's not hovered over anymore
				if (c != previouslyMouseHovered)
				{
					if (previouslyMouseHovered != null) previouslyMouseHovered.OnMouseStopHoverFromRaycast();
					previouslyMouseHovered = c;
				}

				//tell the current one that it's hovered over
				if (c != null)
				{
					//foundIMouseHoverable = true;
					c.OnMouseHoverFromRaycast();
					//print("click me");
					//c.MouseClickMe();
				}

				//if not collectible, don't shot item info
				if (g.GetComponentInParent<Collectible>() == null) itemHoverInfo.SetActive(false);

			}
			else
			{
				if (previouslyMouseHovered != null) previouslyMouseHovered.OnMouseStopHoverFromRaycast();
				itemHoverInfo.SetActive(false);
			}

			//itemHoverInfo.SetActive(foundIMouseHoverable);

			//if (foundIMouseHoverable)
			//{
			//	if(itemHoverPositionMatch) itemHoverInfo.transform.position = Input.mousePosition;
			//	if (Input.GetKey(KeyCode.F))
			//	{
			//		c.MouseClickMe();
			//	}
			//}

			
		}
		//if (Input.GetKey(KeyCode.F))
		//{
		//	RaycastHit hit;
		//	Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		//	if (Physics.Raycast(ray, out hit, Player.main.grabDist, collectibleLayerMask))
		//	{
		//		GameObject g = hit.collider.gameObject;
		//		Collectible c = g.GetComponent<Collectible>();
		//		if (c != null)
		//		{
		//			//print("click me");
		//			c.MouseClickMe();
		//		}
		//	}
		//	else
		//	{
		//		//print("no hit");
		//	}
		//}
	}

	//public void SetUpPlayer(GameObject newPlayerObject)
	//{
	//	SaveEntity save = newPlayerObject.GetComponent<SaveEntity>();
	//	//save.playerOwnerName = username;
	//	myPlayersId = save.id;

	//	//me = newPlayerObject.GetComponent<Player>();
	//	playerControl = newPlayerObject.GetComponent<NPCControl>();
	//	HPBar hpBar = newPlayerObject.GetComponent<HPBar>();
	//	hpBar.hpBarImage = mainHpBar;//TODO: check taht this works
	//	hpBar.hpTextUI = mainHpText;
	//	hpBar.SetWorldHpBarVisible(false);

	//	HPBar mpBar = newPlayerObject.AddComponent<HPBar>();
	//	mpBar.type = HPBar.StatType.mp;
	//	mpBar.hpBarImage = mainMpBar;
	//	mpBar.hpTextUI = mainMpText;
	//	mpBar.autoHide = false;
	//	mpBar.changeHpBarColor = false;
	//	mpBar.useNewLine = true;

	//	HPBar engBar = newPlayerObject.AddComponent<HPBar>();
	//	engBar.type = HPBar.StatType.eng;
	//	engBar.hpBarImage = mainEngBar;
	//	engBar.hpTextUI = mainEngText;
	//	engBar.autoHide = false;
	//	engBar.changeHpBarColor = false;
	//	engBar.useNewLine = true;

	//	HPBar xpBar = newPlayerObject.AddComponent<HPBar>();
	//	xpBar.type = HPBar.StatType.xp;
	//	xpBar.hpBarImage = mainXpBar;
	//	xpBar.hpTextUI = mainXpText;
	//	xpBar.autoHide = false;
	//	xpBar.changeHpBarColor = false;

	//	//replace
	//	Player.main = me;
	//	newPlayerObject.GetComponent<NPCControl>().cam = camGameObject.GetComponentInChildren<Cam>();
	//	newPlayerObject.GetComponent<NPCControl>().playerOwnerName = username;
	//	//print("set up player camera");
	//	myAbilities = newPlayerObject.GetComponent<Abilities>();

	//	//bind hotbar to character and initialize
	//	hotBarUI.target = newPlayerObject.GetComponent<Inventory>();
	//	hotBarUI.InitializeSlots();

	//	//bind inventory to character and initialize
	//	mainInventoryUI.target = mainInventoryUI.GetComponent<Inventory>();
	//	mainInventoryUI.InitializeSlots();

	//	if (playerControl == null) Debug.LogError("Failed to create main character");
	//	Inventory myInv = newPlayerObject.GetComponent<Inventory>();
	//	myInv.take = true;
	//	myInv.put = true;
	//	myInv.take = true;
	//	myInv.put = true;
	//}

	//void ResetPositionOfPlayer(GameObject player)
	//{
	//	Vector3 position;
	//	spawnPoint[] sp = GameObject.FindObjectsOfType<spawnPoint>();
	//	int chosen = UnityEngine.Random.Range(0, sp.Length);
	//	position = sp[chosen].transform.position;

	//	player.transform.position = position;
	//	player.transform.rotation = sp[chosen].transform.rotation;
	//}

	/// <summary>
	/// Creates a player GameObject
	/// </summary>
	/// <param name="respawnPosition">should the player's position be reset? NOTE: position is reset if teleported</param>
	/// <param name="restoreStats">should the player's hp etc be reset?</param>
	/// <returns></returns>
	//GameObject CreatePlayerObject(bool respawnPosition = false, bool restoreStats = false)
	//{
	//	//Vector3 position;
	//	//spawnPoint[] sp = GameObject.FindObjectsOfType<spawnPoint> ();
	//	//int chosen = UnityEngine.Random.Range (0, sp.Length);
	//	//position = sp [chosen].transform.position;

	//	//finds existing player data, makes a gameobject for it, and teleports it. SetupPlayer is called when loading the player
	//	if(myPlayersId != -1)
	//	{
	//		string pathOfThisEntity = SaveEntity.GetPathFromId(myPlayersId);
	//		if (pathOfThisEntity == null || !Directory.Exists(pathOfThisEntity))
	//		{
	//			Debug.LogError("This id could not be found to load player: id: " + myPlayersId);
	//		}
	//		else
	//		{
	//			string[] saveData = SaveEntity.GetSaveDataFromFilePath(pathOfThisEntity);//JsonConvert.DeserializeObject<string[]>(File.ReadAllText(pathOfThisEntity));
	//			//if (saveData.id == myPlayersId)
	//			//{
	//			string type = SaveEntity.GetTypeFromPath(pathOfThisEntity);
	//			GameObject g = SaveEntity.LoadEntity(playerPrefab, saveData);
	//			//TODO: consider below and if it should be also for loading player

	//			//if player is in a different map location than saved (meaning that the player teleported)
	//			if (respawnPosition)
	//			{
	//				ResetPositionOfPlayer(g);
	//				//g.transform.position = position;
	//				//g.transform.rotation = Quaternion.identity;
	//			}
	//			else if (g.GetComponent<SaveEntity>().savedSceneBuildIndex != SceneManager.GetActiveScene().buildIndex)
	//			{
	//				ResetPositionOfPlayer(g);
	//				//g.transform.position = position;
	//				//g.transform.rotation = Quaternion.identity;
	//				//g.GetComponent<StatScript>().resetOnStart = false;//don't give full hp etc every time the game starts
	//			}

	//			if (restoreStats)
	//			{
	//				g.GetComponent<StatScript>().ResetStats();//restore hp etc when respawning

	//			}


	//			//resetting the stats is not good
	//			//g.GetComponent<StatScript>().ResetStats();

	//			return g;
	//				//GameObject g = Instantiate(playerPrefab, position, Quaternion.identity);
	//				//GameObject toSpawn = SaveEntity.GetPrefab(type, ThingType.entity);
	//				//saveData.scene = SceneManager.GetActiveScene().name;
	//				//File.WriteAllText(pathOfThisEntity, JsonConvert.SerializeObject(saveData, Formatting.Indented));
	//			//}
	//			//else
	//			//{
	//			//	Debug.LogError("Somehow wrong id: expected: " + myPlayersId + ", found: " + saveData.id);
	//			//	//return null;
	//			//}
	//		}			
	//	}

	//	Debug.LogWarning("Failed to find player, making new one");

	//	//make a new player gameobject from no saved data

	//	//GameObject newPlayerObject = Instantiate(player, position, Quaternion.identity);
	//	GameObject temp = Instantiate(playerPrefab);
	//	ResetPositionOfPlayer(temp);
	//	SetUpPlayer(temp);//manually set up player since SaveEntity.LoadEntity was not called above due to no player to load
	//	return temp;

	//	//Save save = newPlayerObject.GetComponent<Save>();
	//	//save.playerOwnerName = username;

	//	//me = newPlayerObject.GetComponent<Player> ();
	//	//HPBar hPBar = newPlayerObject.GetComponent<HPBar>();
	//	//hPBar.hpBarImage = mainHpBar;//TODO: check taht this works
	//	//hPBar.hpTextUI = mainHpText;
	//	//hPBar.SetWorldHpBarVisible(false);

	//	//Player.main = me;
	//	//newPlayerObject.GetComponent<PlayerControl>().cam = camGameObject.GetComponentInChildren<Cam>();
	//	//myAbilities = newPlayerObject.GetComponent<Abilities>();

	//	////bind hotbar to character and initialize
	//	//hotBarUI.target = newPlayerObject.GetComponent<Inventory>();
	//	//hotBarUI.InitializeSlots();

	//	//if (Player.main == null) Debug.LogError("Failed to create main character");
	//	//Inventory myInv = newPlayerObject.GetComponent<Inventory>();
	//	//myInv.take = true;
	//	//myInv.put = true;
	//	//myInv.take = true;
	//	//myInv.put = true;
	//}

	//public void PutInitialCharacterInParty()
	//{
	//	GameObject temp = Instantiate(playerPrefab);
	//	RespawnPartyMemberPosition(temp);
	//	SetControlledPartyMember(0);
	//	//ResetPositionOfPlayer(temp);
	//	//SetUpPlayer(temp);//manually set up player since SaveEntity.LoadEntity was not called above due to no player to load
	//}

	#region save
	public static void SavePlayer(GameControl p)
	{
		Crafting crafting = Crafting.main;
		PlayerSaveData s = new PlayerSaveData()
		{
			username = username,
			//myId = GameControl.main.myPlayersId,
			party = GameControl.main.myParty,
			money = GameControl.main.money,
			craftInventoryItems = crafting.craftInventory.items,
			mainInventoryItems = GameControl.main.mainInventoryUI.target.items,
			sceneIndex = SceneManager.GetActiveScene().buildIndex,
		};
		string path1 = Authenticator.GetAccountPath(username);
		if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);//TODO: warning this is bad, allows making account folders without registering
		File.WriteAllText(path1 + "data.json", JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings));
		Debug.Log("Saved player data for username: " + username);
	}

	public static void LoadPlayer()
	{
		Crafting crafting = Crafting.main;// p.GetComponent<Crafting>();
		string path = Authenticator.GetAccountPath(username) + "data.json";
		if (File.Exists(path))
		{
			PlayerSaveData s = JsonConvert.DeserializeObject<PlayerSaveData>(File.ReadAllText(path));
			if (username != s.username) Debug.LogError("Username doesn't match, current name: " + username + ", saved: " + s.username);
			crafting.craftInventory.items = s.craftInventoryItems;
			crafting.craftInventory.InitializeIfEmpty();
			//GameControl.main.myPlayersId = s.myId;
			GameControl.main.myParty = s.party;
			GameControl.main.money = s.money;
			GameControl.main.mainInventoryUI.target.items = s.mainInventoryItems;
			GameControl.main.savedSceneIndex = s.sceneIndex;
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
			InitializeItemTypeMap();
		}
	}

	public static void SaveItemTypes()
	{
		File.WriteAllText(itemTypePath, JsonConvert.SerializeObject(itemTypes.ToArray(), Formatting.Indented, Save.jsonSerializerSettings));
		Debug.Log("Saved ItemTypes");
	}

	public void LoadSavedStuff()
	{
		StartCoroutine(LoadSavedStuffHelper());
	}

	public IEnumerator LoadSavedStuffHelper()
	{
		//string savedVersion = File.ReadAllText(versionFilePath);
		//string currentVersion = Application.version;

		//if (savedVersion != currentVersion)
		//{
		//	Directory.CreateDirectory(Application.persistentDataPath + );
		//}

		LoadPlayer();
		yield return Save.LoadAllData();

		//yield return SaveItem.LoadAll();//load items (inc. buildings maybe) first
		//yield return SaveEntity.LoadAll();

		print("loaded saved entities and items...");
	}

	public void SaveStuff()
	{
		SavePlayer(this);

		Save.SaveAllData();

		spawner.SaveAllSpawners();
		TowerControl.main.SaveTowers();
		//SaveEntity.SaveAll();
		//SaveItem.SaveAll();

		print("saved entities and items...");
	}

	#endregion

	#region inventory
	public bool GetItem(int id) { return GetItem(id, 1); }
	//TODO: this won't account for durability etc.
	public bool GetItem(Item i) { return GetItem(i.id, i.amount); }
	public bool GetItem(int id, int amount)
	{
		//check if it can be put in hotbar
		Inventory inv = hotBarUI.target;
		if (inv == false) return false;
		for (int i = 0; i < inv.items.Count; i++)
		{
			if (inv.items[i].id == id && Mathf.Abs(inv.items[i].currentStrength - inv.items[i].strength) < 0.01f)
			{
				Item temp = inv.items[i];
				temp.amount += amount;
				inv.items[i] = temp;
				//inv.items [i].amount += amount;

				//TODO: this might require inventory to be refreshed
				//if (invSel == i) {
				//	RefreshSelected();
				//	//SelectInv (invSel);
				//}
				ProgressTracker.main.RegisterItemObtained(new Item() { id = id, amount = amount });
				return true;
			}
		}
		for (int i = 0; i < inv.items.Count; i++)
		{
			if (inv.items[i].id == 0)
			{
				inv.items[i] = new Item(id, amount, GameControl.itemTypes[id].strength, GameControl.itemTypes[id].strength);
				if (GameControl.main.playerControl.invSel == i)
				{

					GameControl.main.playerControl.RefreshSelected();
					//SelectInv (invSel);
				}
				ProgressTracker.main.RegisterItemObtained(new Item() { id = id, amount = amount });
				return true;
			}
		}

		//check if it can be put in main inventory
		inv = mainInventoryUI.target;

		for (int i = 0; i < inv.items.Count; i++)
		{
			if (inv.items[i].id == id && Mathf.Abs(inv.items[i].currentStrength - inv.items[i].strength) < 0.01f)
			{
				Item temp = inv.items[i];
				temp.amount += amount;
				inv.items[i] = temp;
				ProgressTracker.main.RegisterItemObtained(new Item() { id = id, amount = amount });
				return true;
			}
		}
		for (int i = 0; i < inv.items.Count; i++)
		{
			if (inv.items[i].id == 0)
			{
				inv.items[i] = new Item(id, amount, GameControl.itemTypes[id].strength, GameControl.itemTypes[id].strength);
				ProgressTracker.main.RegisterItemObtained(new Item() { id = id, amount = amount });
				return true;
			}
		}

		return false;//failed to find space for the item
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
	//public long myId;//the id of the player owned, use this to load it
	public Party party;
	public int money;
	public int sceneIndex = -1;
	public List<Item> craftInventoryItems;
	public List<Item> mainInventoryItems;
}

public interface IMouseHoverable
{
	void OnMouseHoverFromRaycast();
	void OnMouseStopHoverFromRaycast();
}
