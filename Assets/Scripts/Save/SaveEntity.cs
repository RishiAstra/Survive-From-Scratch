using Newtonsoft.Json;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using bobStuff;
using System;

//TODO: link to abilities to update when damaged or killed
//TODO: organize all of the paths in a well-defined mannar
//TODO: save the path to player-controlled to the player's data file
//TODO: save player data file including crafting inventory etc.
[RequireComponent(typeof(Abilities))]
public class SaveEntity : Save, ISaveable
{
	public static List<SaveEntity> saves;
	public static Dictionary<string, GameObject> spawnObjects;
	public static List<EntityMapData> toSaveMapData;

	const string spawnPath = "Assets/Spawnable/";
	public static string savePath {
		get {
			return Application.persistentDataPath + "/Save/Entities/";
		}
	}

	public static string entitySceneMapPath
	{
		get
		{
			return Application.persistentDataPath + "/Save/Map/";
		}
	}

	public string playerOwnerName;

	public string type;
	public string customSavePath;

	public Abilities a;
	public bool deleteOnDeath;

	public Component[] toSave;

	private int indexInSaves;
	private Stat pStat;

	public static void InitializeStatic()
	{
		saves = new List<SaveEntity>();
		spawnObjects = new Dictionary<string, GameObject>();
		toSaveMapData = new List<EntityMapData>();
	}

	// Start is called before the first frame update
	void Awake()
    {
		if (saves == null) InitializeStatic();
		indexInSaves = saves.Count;
		saves.Add(this);

		//add ISaveables
		//start with this SaveEntity
		int c = toSave.Length;//current length
		int s = c;//start length

		//increase current length
		c++;//yay the horrifying name has been written

		//resize array
		Array.Resize(ref toSave, c);
		//add this
		toSave[s] = this;


		//if (!readNextId)
		//{
		//	//TryReadNextID();
		//	//string path = Application.persistentDataPath + "/nextid.txt";
		//	////byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		//	//if(File.Exists(path)) nextId = int.Parse(File.ReadAllText(path));
		//	//readNextId = true;
		//}
		TryReadNextID();


		//if (saveAbilities) a = GetComponent<Abilities>();
		if (id == 0)
		{
			id = nextId;
			nextId++;
		}

		pStat = a.stat;
	}

    // Update is called once per frame
    void Update()
    {
		//TODO: find a better way to detect changes
		//Autosave the entity if stat was changed
		if (!Stat.StatEquals(pStat, a.stat))
		{
			//print("autosaved data for entity id: " + id);
			pStat = a.stat;
			SaveDataToFile();
		}
	}

	//TODO: warning: the dead body will get deleted even if it's supposed to stay as a dead body for a few seconds
	private void OnDestroy()
	{
		//TODO:GetPath broken
		//TODO: this will break with new saving
		string filePath = GetPath();// + id + ".json";
		if (a.dead){
			if (deleteOnDeath){
				if (type == "Player") Debug.LogError("no");
				//if (File.Exists(filePath))
				//{
				//	File.Delete(filePath);
				//	print("Deleted dead entity id: " + id + ", type: " + type);
				//}
				if (Directory.Exists(filePath))
				{
					Directory.Delete(filePath, true);
					print("Deleted dead entity id: " + id + ", type: " + type);
				}
			}
			else
			{
				toSaveMapData.Add(new EntityMapData(this.id, this.type, SceneManager.GetActiveScene().buildIndex));
				SaveDataToFile();
			}
		}
	}

	public string GetPath()
	{
		return savePath + type + "/" + id + "/";
	}

	public void SaveDataToFile()
	{
		//TODO:GetPath broken
		string path = GetPath();
		Directory.CreateDirectory(path);

		object[] data = GetAllData();

		File.WriteAllText(path + "data.json", JsonConvert.SerializeObject(data, Formatting.Indented));

		//File.WriteAllText(path + id + ".json", JsonConvert.SerializeObject(GetData(), Formatting.Indented));
		//TODO: GetData() above
		//File.WriteAllText(path + , SceneManager.GetActiveScene().name);
	}

	public static string GetPathFromId(long id)
	{
		if (!Directory.Exists(savePath))
		{
			Debug.LogWarning("No save folder");
			return null;
		}
		if (!Directory.Exists(entitySceneMapPath))
		{
			Debug.LogWarning("No entity scene map folder");
			return null;
		}
		print(entitySceneMapPath);
		//find type of this by searching scene entity map, then calculate and return path
		foreach (string sceneString in Directory.GetFiles(entitySceneMapPath))
		{
			print(sceneString);
			List<EntityMapData> mapData = JsonConvert.DeserializeObject<List<EntityMapData>>(File.ReadAllText(sceneString));

			for (int i = 0; i < mapData.Count; i++)
			{
				EntityMapData d = mapData[i];
				print(d.id);
				if (d.id == id)
				{
					return savePath + d.type + "/" + d.id + "/data.json";
				}
				

				//load the data
				
			}
		}

		return null;

		//foreach (string typeString in Directory.GetDirectories(savePath))
		//{
		//	foreach (string idPath in Directory.GetFiles(typeString))
		//	{

		//		SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
		//		if (saveData.id == id)
		//		{
		//			return idPath;
		//		}
		//	}
		//}

		//return null;
	}

	public static string GetTypeFromPath(string path)
	{
		//cut after last /
		int i1 = path.LastIndexOf("/");
		if (i1 == -1) return null;
		string s2 = path.Substring(0, i1);

		//cut after last /
		int i2 = s2.LastIndexOf("/");
		if (i2 == -1) return null;
		string s3 = s2.Substring(0, i2);

		//return from last / to end
		int i3 = s3.LastIndexOf("/");
		if (i3 == -1) return null;
		return s3.Substring(i3 + 1);
	}

	public static void TeleportEntityBetweenScenes(long idToMove, int nextScene)
	{
		if (!Directory.Exists(savePath))
		{
			Debug.LogWarning("No save folder");
			return;
		}


		//TODO:won't work
		//find type of this by searching scene entity map, then calculate and return path
		foreach (string sceneString in Directory.GetFiles(entitySceneMapPath))
		{
			List<EntityMapData> mapData = JsonConvert.DeserializeObject<List<EntityMapData>>(File.ReadAllText(sceneString));

			for (int i = 0; i < mapData.Count; i++)
			{
				EntityMapData d = mapData[i];
				if (d.id == idToMove)
				{
					d.sceneIndex = nextScene;
					mapData[i] = d;
					File.WriteAllText(sceneString + ".json", JsonConvert.SerializeObject(mapData));
					return;
				}
				//load the data

			}
		}

		Debug.LogError("This id could not be found to teleport: id: " + idToMove);


		//string pathOfThisEntity = GetPathFromId(idToMove);
		//if (pathOfThisEntity == null)
		//{
		//	Debug.LogError("This id could not be found to teleport: id: " + idToMove);
		//	return;
		//}
		//SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(pathOfThisEntity));
		//if (saveData.id == idToMove)
		//{
		//	saveData.scene = nextScene;
		//	File.WriteAllText(pathOfThisEntity, JsonConvert.SerializeObject(saveData, Formatting.Indented));
		//}
		//else
		//{
		//	Debug.LogError("Somehow wrong id: expected: " + idToMove + ", found: " + saveData.id);
		//	return;
		//}

		//foreach (string typeString in Directory.GetDirectories(savePath))
		//{
		//	string type = typeString.Substring(savePath.Length);
		//	//print("fetching entity prefab: " + type);

		//	List<Save> loadedSaves = new List<Save>();
		//	foreach (string idPath in Directory.GetFiles(typeString))
		//	{

		//		SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
		//		if (saveData.id == idToMove)
		//		{
		//			saveData.scene = nextScene;
		//			File.WriteAllText(idPath, JsonConvert.SerializeObject(saveData, Formatting.Indented));
		//		}
		//	}
		//}
	}

	//public void GetDataFromFile()
	//{

	//}

	//public SaveData GetData()
	//{
	//	Inventory inventory = GetComponent<Inventory>();
	//	List<Item> tempItems = inventory != null ? GetComponent<Inventory>().items : null;
	//	return new SaveData
	//	{
	//		id = id,
	//		scene = SceneManager.GetActiveScene().name,
	//		maxStat = a.maxStat,
	//		stat = a.stat,
	//		position = transform.position,
	//		rotation = transform.eulerAngles,
	//		playerOwnerName = playerOwnerName,
	//		items = tempItems,
	//	};
	//}

	//public void SetData(SaveData data)
	//{
	//	id = data.id;
	//	a.maxStat = data.maxStat;
	//	a.stat = data.stat;
	//	transform.position = data.position;
	//	transform.eulerAngles = data.rotation;
	//	playerOwnerName = data.playerOwnerName;
	//	//TODO:this assignes this player to the gamecontrol
	//	if(playerOwnerName == GameControl.username)
	//	{
	//		GameControl.main.SetUpPlayer(gameObject);
	//	}
	//	List<Item> tempItems = data.items;
	//	Inventory inventory = GetComponent<Inventory>();
	//	if (tempItems != null)
	//	{
	//		if (inventory == null)
	//		{
	//			Debug.LogError("An Inventory was loaded, but there is not one to load it to on this GameObject");
	//		}
	//		else
	//		{
	//			inventory.items = tempItems;
	//		}
	//	}
	//}

	public static GameObject LoadEntity(GameObject typePrefab, object[] saveData)
	{
		GameObject g = Instantiate(typePrefab);

		Abilities a = g.GetComponent<Abilities>();
		if(a != null) a.resetOnStart = false;//prevent resetting of hp etc

		//TODO:see below
		g.GetComponent<SaveEntity>().SetAllData(saveData);
		//saves.Add(g.GetComponent<SaveEntity>());
		return g;
	}

	public static IEnumerator LoadAll()
	{
		int typeCount = 0;
		int entityCount = 0;
		if (!Directory.Exists(savePath))
		{
			Debug.LogWarning("Nothing to load!");
			yield break;
		}

		if (!Directory.Exists(entitySceneMapPath))
		{
			Debug.LogWarning("No entity scene map to load");
			yield break;
		}

		List<EntityMapData> mapData = JsonConvert.DeserializeObject<List<EntityMapData>>(File.ReadAllText(entitySceneMapPath + SceneManager.GetActiveScene().buildIndex + ".json"));
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		List<List<Save>> newSaves = new List<List<Save>>();
		List<string> typesLoaded = new List<string>();

		for (int i = 0; i < mapData.Count; i++)
		{
			EntityMapData d = mapData[i];
			if(d.sceneIndex != currentSceneIndex)
			{
				Debug.LogError("entity wrong scene");
				continue;
			}


			//get the gameobject to spawn
			GameObject toSpawn;
			bool succeed = GetEntityPrefabCached(d.type, out toSpawn);
			if (!succeed)
			{
				AsyncOperationHandle<GameObject> a = GetEntityPrefab(d.type);
				yield return a;
				spawnObjects.Add(d.type, a.Result);
				toSpawn = a.Result;
			}

			//load the data
			string thisEntityPath = savePath + d.type + "/" + d.id + "/data.json";
			object[] saveData = JsonConvert.DeserializeObject<object[]>(File.ReadAllText(thisEntityPath));
			entityCount++;
			GameObject g = LoadEntity(toSpawn, saveData);

			//add to newSaves
			int newSavesIndex = typesLoaded.IndexOf(d.type);
			if(newSavesIndex == -1)
			{
				newSavesIndex = newSaves.Count;

				typesLoaded.Add(d.type);
				newSaves.Add(new List<Save>());				
			}
			newSaves[newSavesIndex].Add(g.GetComponent<Save>());
		}

		//do this now
		for(int i = 0; i < typesLoaded.Count; i++)
		{
			Save.CallOnLoadedtype(typesLoaded[i], newSaves[i]);
		}
		typeCount = typesLoaded.Count;

		


		//foreach (string typeString in Directory.GetDirectories(savePath))
		//{
		//	//TODO: check if this type even exists (if it has a prefab)
		//	typeCount++;
		//	string type = typeString.Substring(savePath.Length);
		//	print("fetching entity prefab: " + type);
		//	AsyncOperationHandle<GameObject> toSpawnAsync = GetEntityPrefab(type);
		//	yield return toSpawnAsync;
		//	GameObject toSpawn = toSpawnAsync.Result;

		//	List<Save> loadedSaves = new List<Save>();
		//	foreach (string idPath in Directory.GetDirectories(typeString))//GetFiles()
		//	{
				
		//		//SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
		//		object[] saveData = JsonConvert.DeserializeObject<object[]>(File.ReadAllText(idPath));



		//		if (saveData.scene == SceneManager.GetActiveScene().name)
		//		{
		//			entityCount++;
		//			//LoadEntity(toSpawn, saveData);
		//			GameObject g = LoadEntity(toSpawn, saveData); //Instantiate(toSpawn);
		//														  //g.GetComponent<Abilities>().resetOnStart = false;//prevent resetting of hp etc
		//														  //g.GetComponent<SaveEntity>().SetData(saveData);

		//			//TODO: warning: should check if null
		//			//add the save
		//			loadedSaves.Add(g.GetComponent<Save>());
		//		}				
		//	}
		//	Save.CallOnLoadedtype(type, loadedSaves);
		//}
		print("Loaded entities: " + entityCount + ", " + typeCount + "types");
		yield return null;
	}

	public static bool GetEntityPrefabCached(string type, out GameObject result)
	{
		bool succeed = spawnObjects.TryGetValue(type, out result);
		return succeed;
	}

	public static AsyncOperationHandle<GameObject> GetEntityPrefab(string type)
	{
		return Addressables.LoadAssetAsync<GameObject>(spawnPath + type + "/" + type + ".prefab");
	}

	public static void SaveAll()
	{
		List<EntityMapData> mapData = new List<EntityMapData>();
		mapData.AddRange(toSaveMapData);
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

		for (int i = 0; i < saves.Count; i++)
		{
			if (saves[i] != null)
			{
				saves[i].SaveDataToFile();
				mapData.Add(new EntityMapData(saves[i].id, saves[i].type, currentSceneIndex));
			}
		}

		Directory.CreateDirectory(entitySceneMapPath);
		File.WriteAllText(entitySceneMapPath + SceneManager.GetActiveScene().buildIndex + ".json", JsonConvert.SerializeObject(mapData, Formatting.Indented));

		//string path = Application.persistentDataPath + "/nextid.txt";
		////byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		//File.WriteAllText(path, nextId.ToString());
	}

	const string baseDataPath = "";

	public object GetData()
	{
		return new SaveDataBasic()
		{
			id = id,
			sceneIndex = SceneManager.GetActiveScene().buildIndex,
			position = transform.position,
			rotation = transform.eulerAngles,
		};
	}

	public void SetData(object data)
	{
		SaveDataBasic s = (SaveDataBasic)data;
		//TODO: warning, sceneindex not considered here
		id = s.id;
		transform.position = s.position;
		transform.eulerAngles = s.rotation;
	}

	public object[] GetAllData()
	{
		object[] data = new object[toSave.Length];
		for (int i = 0; i < toSave.Length; i++)
		{
			data[i] = ((ISaveable)toSave[i]).GetData();
		}
		return data;
	}

	public void SetAllData(object[] data)
	{
		if (data.Length != toSave.Length) Debug.LogError("wrong data, data.Length: " + data.Length + ", toSave.Length: " + toSave.Length);
		//object[] data = new object[toSave.Length];
		for (int i = 0; i < toSave.Length; i++)
		{
			((ISaveable)toSave[i]).SetData(data[i]);
		}
	}
}



#region Save Data Classes

//public interface SaveData
//{
//	void GetDataFromGameObject(ISaveable g);
//	void SetDataFromGameObject(ISaveable g);
//}

public struct EntityMapData
{
	public long id;
	public string type;
	public int sceneIndex;

	public EntityMapData(long id, string type, int sceneIndex)
	{
		this.id = id;
		this.type = type;
		this.sceneIndex = sceneIndex;
	}
}

public interface ISaveable
{
	object GetData();
	void SetData(object data);
}

//TODO: [in progress] split into inventory and abilities etc.
[System.Serializable]
public class SaveDataBasic
{
	public long id;
	/// <summary>
	/// unused
	/// </summary>
	public int sceneIndex;
	public Vector3 position;
	public Vector3 rotation;
}

[System.Serializable]
public class SaveDataAbilities
{
	public Stat maxStat;
	public Stat stat;
}

[System.Serializable]
public class SaveDataInventory
{
	public List<Item> items;

}

public class SaveDataControl
{
	public string playerOwnerName;//TODO: use player id
}

//[System.Serializable]
//public class SaveData
//{
//	public long id;
//	public string scene;
//	public Stat maxStat;
//	public Stat stat;
//	public Vector3 position;
//	public Vector3 rotation;
//	public string playerOwnerName;
//	public List<Item> items;
//}

#endregion