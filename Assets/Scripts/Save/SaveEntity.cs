/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
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
using System.Linq;
using Newtonsoft.Json.Linq;

//TODO: link to abilities to update when damaged or killed
//TODO: organize all of the paths in a well-defined mannar
//TODO: save the path to player-controlled to the player's data file
//TODO: save player data file including crafting inventory etc.
public class SaveEntity : Save, ISaveable
{
	private static JObject allEntities;
			
		
	public static JObject AllEntities { get { if (allEntities == null) allEntities = new JObject(); return allEntities; } set => allEntities = value; }

	private static JObject playerEntities;
	public static JObject PlayerEntities { get { if (playerEntities == null) playerEntities = new JObject(); return playerEntities; } set => playerEntities = value; }

	public static List<SaveEntity> saves;
	public static Dictionary<string, GameObject> spawnObjects;
	public static List<EntityMapData> toSaveMapData;

	public const string spawnPath = "Assets/Spawnable/";
	//public static string savePath {
	//	get {
	//		return GameControl.saveDirectory + "/Save/Entities/";
	//	}
	//}

	public static string allEntityFile
	{
		get
		{
			return GameControl.saveDirectory + "/Save/Entities/e.json";
		}
	}

	public static string playerEntityFile
	{
		get
		{
			return GameControl.playerCharacterDirectory + "e.json";
		}
	}

	public static string entitySceneMapPath
	{
		get
		{
			return GameControl.saveDirectory + "/Save/Map/";
		}
	}



	//public string playerOwnerName;

	public string type;
	public string customSavePath;

	public StatScript a;
	public bool deleteOnDeath;
	public bool playerOwned;

	public Component[] toSave;

	private int indexInSaves;
	private Stat pStat;
	public int savedSceneBuildIndex = -1;

	public static void InitializeStatic()
	{
		saves = new List<SaveEntity>();
		spawnObjects = new Dictionary<string, GameObject>();
		toSaveMapData = new List<EntityMapData>();
	}

	// Start is called before the first frame update
	void Awake()
	{
		a = GetComponent<StatScript>();
		if (saves == null) InitializeStatic();
		indexInSaves = saves.Count;
		saves.Add(this);
		InitializeToSave();


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
		//if (id == 0)
		//{
		//	id = nextId;
		//	nextId++;
		//}

		if (id == 0)
		{
			id = nextId;
			nextId++;
		}

		if (a!=null) pStat = a.stat;
	}

	void Start()
	{
		pStat = a.stat;
	}

	private void InitializeToSave()
	{
		//TODO: WARNING: order is imporant
		List<Component> toSaveTemp = toSave.ToList();

		toSaveTemp.Add(this);
		StatScript s = GetComponent<StatScript>();//might be null if none are attached, but null checks are used for "a"
		if (s != null) toSaveTemp.Add(s);
		Abilities a = GetComponent<Abilities>();
		if (a != null) toSaveTemp.Add(a);
		Inventory i = GetComponent<Inventory>();
		if (i != null) toSaveTemp.Add(i);
		NPCControl c = GetComponent<NPCControl>();
		if (c != null) toSaveTemp.Add(c);

		toSave = toSaveTemp.ToArray();

		////add ISaveables
		////start with this SaveEntity
		//int c = toSave.Length;//current length
		//int s = c;//start length

		////increase current length
		//c++;//yay the horrifying name has been written

		//Abilities a = GetComponent<Abilities>();
		//if (a != null) c++;
		//Inventory i = GetComponent<Inventory>();
		//if (i != null) c++;

		////resize array
		//Array.Resize(ref toSave, c);
		////add this
		//toSave[s] = this;
	}

	//// Update is called once per frame
	//void Update()
 //   {
	//	//TODO: find a better way to detect changes
	//	//Autosave the entity if stat was changed
	//	if (a != null &&!Stat.StatEquals(pStat, a.stat))
	//	{
	//		//print("autosaved data for entity id: " + id);
	//		pStat = a.stat;
	//		SaveDataToFile();
	//	}
	//}

	//TODO: warning: the dead body will get deleted even if it's supposed to stay as a dead body for a few seconds
	private void OnDestroy()
	{
		//TODO:GetPath broken
		//TODO: this will break with new saving
		//string filePath = GetPath();// + id + ".json";
		if (a == null || a.dead)
		{
			if (deleteOnDeath){
				if (type == "Player") Debug.LogError("no");
				//if (File.Exists(filePath))
				//{
				//	File.Delete(filePath);
				//	print("Deleted dead entity id: " + id + ", type: " + type);
				//}

				if (playerOwned)
				{
					if(PlayerEntities.ContainsKey(id.ToString()))
					PlayerEntities.Remove(id.ToString());
				}
				else
				{
					if (AllEntities.ContainsKey(type) && ((JObject)AllEntities[type]).ContainsKey(id.ToString()))
					{
						((JObject)AllEntities[type]).Remove(id.ToString());
					}
					
				}

				//if (Directory.Exists(filePath))
				//{
				//	Directory.Delete(filePath, true);
				//	print("Deleted dead entity id: " + id + ", type: " + type);
				//}
			}
			else
			{
				if(!playerOwned) toSaveMapData.Add(new EntityMapData(this.id, this.type, SceneManager.GetActiveScene().buildIndex));
				SaveDataToFile();
			}
		}
	}

	//public string GetPath()
	//{
	//	if (playerOwned)
	//	{
	//		return GameControl.playerCharacterDirectory + id + "/";
	//	}
	//	else
	//	{
	//		return savePath + type + "/" + id + "/";
	//	}
	//}

	//public static string GetPathFromId(long id)
	//{
	//	if (!Directory.Exists(savePath))
	//	{
	//		Debug.LogWarning("No save folder");
	//		return null;
	//	}
	//	if (!Directory.Exists(entitySceneMapPath))
	//	{
	//		Debug.LogWarning("No entity scene map folder");
	//		return null;
	//	}
	//	//print(entitySceneMapPath);
	//	//find type of this by searching scene entity map, then calculate and return path
	//	foreach (string sceneString in Directory.GetFiles(entitySceneMapPath))
	//	{
	//		//print(sceneString);
	//		List<EntityMapData> mapData = JsonConvert.DeserializeObject<List<EntityMapData>>(File.ReadAllText(sceneString));

	//		for (int i = 0; i < mapData.Count; i++)
	//		{
	//			EntityMapData d = mapData[i];
	//			//print(d.id);
	//			if (d.id == id)
	//			{
	//				return GetFilePathFromEntity(d);// savePath + d.type + "/" + d.id + "/";//data.json
	//			}
				

	//			//load the data
				
	//		}
	//	}

	//	return null;

	//	//foreach (string typeString in Directory.GetDirectories(savePath))
	//	//{
	//	//	foreach (string idPath in Directory.GetFiles(typeString))
	//	//	{

	//	//		SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
	//	//		if (saveData.id == id)
	//	//		{
	//	//			return idPath;
	//	//		}
	//	//	}
	//	//}

	//	//return null;
	//}

	//public static string GetTypeFromPath(string path)
	//{
	//	//cut after last /
	//	int i1 = path.LastIndexOf("/");
	//	if (i1 == -1) return null;
	//	string s2 = path.Substring(0, i1);

	//	//cut after last /
	//	int i2 = s2.LastIndexOf("/");
	//	if (i2 == -1) return null;
	//	string s3 = s2.Substring(0, i2);

	//	//return from last / to end
	//	int i3 = s3.LastIndexOf("/");
	//	if (i3 == -1) return null;
	//	return s3.Substring(i3 + 1);
	//}

	//public static void TeleportEntityBetweenScenes(long idToMove, int nextScene)
	//{
	//	if (!Directory.Exists(savePath))
	//	{
	//		Debug.LogWarning("No save folder");
	//		return;
	//	}
	//	//TODO:warning, requires entitySceneMapPath to exist in file system

	//	EntityMapData toMove;
	//	//TODO:won't work
	//	//find type of this by searching scene entity map, then calculate and return path
	//	foreach (string sceneString in Directory.GetFiles(entitySceneMapPath))
	//	{
	//		List<EntityMapData> mapData = JsonConvert.DeserializeObject<List<EntityMapData>>(File.ReadAllText(sceneString));

	//		for (int i = 0; i < mapData.Count; i++)
	//		{
	//			EntityMapData d = mapData[i];
	//			if (d.id == idToMove)
	//			{
	//				d.sceneIndex = nextScene;
	//				toMove = d;
	//				//remove this one and write the updated
	//				mapData.RemoveAt(i);
	//				File.WriteAllText(sceneString, JsonConvert.SerializeObject(mapData));// + ".json"
	//				goto FoundID;
	//			}
	//			//load the data

	//		}
	//	}
	//	//if the goto wasn't activated, failed
	//	Debug.LogError("This id could not be found to teleport: id: " + idToMove);
	//	return;
	//	//goto was activated, success
	//	FoundID:
	//	//btw because of the goto and for loops, the directory entitySceneMapPath must exist or function would have returned or errored

	//	//put the data in target scene
	//	//use SceneUtility instead of SceneManager because SceneManager doesn't know about unloaded scenes
	//	string newPath = entitySceneMapPath + nextScene + ".json";//Path.GetFileName(SceneUtility.GetScenePathByBuildIndex(nextScene))
	//	List<EntityMapData> targetMapData;

	//	//if there is a file for entities in this scene, load it, otherwise create it
	//	if (File.Exists(newPath))
	//	{
	//		targetMapData = JsonConvert.DeserializeObject<List<EntityMapData>>(File.ReadAllText(newPath));
	//	}
	//	else
	//	{
	//		targetMapData = new List<EntityMapData>();
	//	}
	//	targetMapData.Add(toMove);
	//	File.WriteAllText(newPath, JsonConvert.SerializeObject(targetMapData));


	//}
	
	public static GameObject LoadEntity(GameObject typePrefab, JArray saveData)
	{
		GameObject g = Instantiate(typePrefab);

		StatScript a = g.GetComponent<StatScript>();
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
		if (File.Exists(allEntityFile))
		{
			AllEntities = JObject.Parse(File.ReadAllText(allEntityFile));
			PlayerEntities = JObject.Parse(File.ReadAllText(playerEntityFile));
		}
		else
		{
			Debug.LogWarning("Nothing to load!");
			yield break;
		}

		if (!File.Exists(entitySceneMapPath + SceneManager.GetActiveScene().buildIndex + ".json"))
		{
			Debug.LogWarning("No entity scene map file to load");
			yield break;
		}

		List<EntityMapData> mapData = JsonConvert.DeserializeObject<List<EntityMapData>>(File.ReadAllText(entitySceneMapPath + SceneManager.GetActiveScene().buildIndex + ".json"));
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		List<List<Save>> newSaves = new List<List<Save>>();
		List<string> typesLoaded = new List<string>();

		for (int i = 0; i < mapData.Count; i++)
		{
			EntityMapData d = mapData[i];
			if (d.sceneIndex != currentSceneIndex)
			{
				Debug.LogError("entity wrong scene, probably due to teleportation. Continueing anyways");
				//continue;
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

			//this will check if the entity exists, otherwise it'll skip.
			//TODO: delete entities in the scene map if they don't exist
			if (AllEntities.ContainsKey(d.type) && ((JObject)AllEntities[d.type]).ContainsKey(d.id.ToString())){
				JArray saveData = GetSaveDataFromFilePath(d.type, d.id.ToString());

				//string[] saveData = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(thisEntityPath));
				entityCount++;
				GameObject g = LoadEntity(toSpawn, saveData);

				//add to newSaves
				int newSavesIndex = typesLoaded.IndexOf(d.type);
				if (newSavesIndex == -1)
				{
					newSavesIndex = newSaves.Count;

					typesLoaded.Add(d.type);
					newSaves.Add(new List<Save>());
				}
				newSaves[newSavesIndex].Add(g.GetComponent<Save>());
			}
			//string thisEntityPath = GetFilePathFromEntity(d);// data.json";
			//if (Directory.Exists(thisEntityPath))
			//{
				
			//}
			
		}

		//do this now
		for (int i = 0; i < typesLoaded.Count; i++)
		{
			Save.CallOnLoadedtype(typesLoaded[i], newSaves[i]);
		}
		typeCount = typesLoaded.Count;

		
		print("Loaded entities: " + entityCount + ", " + typeCount + "types");
		yield return null;
	}

	//public static string GetFilePathFromEntity(EntityMapData d)
	//{

	//	//load the data
	//	return savePath + d.type + "/" + d.id + "/";
	//}

	public static JArray GetSaveDataFromFilePath(string type, string id)
	{
		//DirectoryInfo di = new DirectoryInfo(thisEntityPath);
		////print(thisEntityPath);
		//FileSystemInfo[] files = di.GetFileSystemInfos();
		//IOrderedEnumerable<FileSystemInfo> orderedFiles = files.OrderBy(
		//	(f) => int.Parse(new string(f.Name.Where(Char.IsDigit).ToArray()))
		//);


		//string[] saveData = new string[orderedFiles.Count()];
		//for (int j = 0; j < orderedFiles.Count(); j++)
		//{
		//	saveData[j] = File.ReadAllText(orderedFiles.ElementAt(j).FullName);// JsonConvert.DeserializeObject<string>(File.ReadAllText(orderedFiles.ElementAt(j).FullName));
		//}

		//warning: if data doesn't exist, this will erorr
		return (JArray)AllEntities[type][id.ToString()];

		//return saveData;
	}

	public static JArray GetPlayerDataFromFilePath(string id)
	{
		//DirectoryInfo di = new DirectoryInfo(thisEntityPath);
		////print(thisEntityPath);
		//FileSystemInfo[] files = di.GetFileSystemInfos();
		//IOrderedEnumerable<FileSystemInfo> orderedFiles = files.OrderBy(
		//	(f) => int.Parse(new string(f.Name.Where(Char.IsDigit).ToArray()))
		//);


		//string[] saveData = new string[orderedFiles.Count()];
		//for (int j = 0; j < orderedFiles.Count(); j++)
		//{
		//	saveData[j] = File.ReadAllText(orderedFiles.ElementAt(j).FullName);// JsonConvert.DeserializeObject<string>(File.ReadAllText(orderedFiles.ElementAt(j).FullName));
		//}

		return (JArray)PlayerEntities[id.ToString()];

		//return saveData;
	}

	public void SaveDataToFile()
	{
		//TODO:GetPath broken
		//string path = GetPath();
		//Directory.CreateDirectory(path);

		JArray data = GetAllData();
		if (playerOwned)
		{
			PlayerEntities[id.ToString()] = data;
		}
		else
		{
			if (!AllEntities.ContainsKey(type))
			{
				AllEntities[type] = new JObject();
			}
			AllEntities[type][id.ToString()] = data;
		}
		//for(int i = 0; i < data.Length; i++)
		//{
		//	File.WriteAllText(path + ((ISaveable)toSave[i]).GetFileNameBaseForSavingThisComponent() + "_Component_" + i + ".json", data[i]);// JsonConvert.SerializeObject(data, Formatting.Indented));
		//}

		//File.WriteAllText(path + "data.json", JsonConvert.SerializeObject(data, Formatting.Indented));

		//File.WriteAllText(path + id + ".json", JsonConvert.SerializeObject(GetData(), Formatting.Indented));
		//TODO: GetData() above
		//File.WriteAllText(path + , SceneManager.GetActiveScene().name);
	}

	public static void SaveAll()
	{
		//print(toSaveMapData.ToString());
		List<EntityMapData> mapData = new List<EntityMapData>();
		mapData.AddRange(toSaveMapData);
		//print("dead save count: " + toSaveMapData.Count);
		//these will now be saved, so delete them to prevent double saving later on when this function is called again
		toSaveMapData = new List<EntityMapData>();

		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

		for (int i = 0; i < saves.Count; i++)
		{
			if (saves[i] != null)
			{
				saves[i].SaveDataToFile();
				//don't put player-owned in map data
				if (!saves[i].playerOwned) mapData.Add(new EntityMapData(saves[i].id, saves[i].type, currentSceneIndex));
			}
		}

		Directory.CreateDirectory(entitySceneMapPath);
		File.WriteAllText(entitySceneMapPath + SceneManager.GetActiveScene().buildIndex + ".json", JsonConvert.SerializeObject(mapData, Formatting.Indented, Save.jsonSerializerSettings));

		FileInfo allInfo = new FileInfo(allEntityFile);
		FileInfo playerInfo = new FileInfo(playerEntityFile);

		Directory.CreateDirectory(allInfo.Directory.FullName);
		Directory.CreateDirectory(playerInfo.Directory.FullName);

		if(AllEntities != null && AllEntities.Count > 0) File.WriteAllText(allEntityFile, AllEntities.ToString());
		if(PlayerEntities != null && PlayerEntities.Count > 0) File.WriteAllText(playerEntityFile, PlayerEntities.ToString());
		//string path = Application.persistentDataPath + "/nextid.txt";
		////byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		//File.WriteAllText(path, nextId.ToString());
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

	const string baseDataPath = "";

	public JObject GetData()
	{
		SaveDataBasic s = new SaveDataBasic()
		{
			id = id,
			sceneIndex = SceneManager.GetActiveScene().buildIndex,
			position = transform.position,
			rotation = transform.eulerAngles,
		};
		return JObject.FromObject(s, Save.jsonSerializer);// JsonConvert.SerializeObject(s, Formatting.Indented, jsonSerializerSettings);
	}

	public void SetData(JObject data)
	{
		SaveDataBasic s = data.ToObject<SaveDataBasic>();// JsonConvert.DeserializeObject<SaveDataBasic>(data);
		//TODO: warning, sceneindex not considered here
		id = s.id;
		transform.position = s.position;
		transform.eulerAngles = s.rotation;
		savedSceneBuildIndex = s.sceneIndex;
	}

	public JArray GetAllData()
	{
		JArray data = new JArray();// new string[toSave.Length];
		for (int i = 0; i < toSave.Length; i++)
		{
			data.Add(((ISaveable)toSave[i]).GetData());
			//data[i] = ((ISaveable)toSave[i]).GetData();
		}
		return data;
	}

	public void SetAllData(JArray data)
	{
		if (data.Count != toSave.Length) Debug.LogError("wrong data, data.Length: " + data.Count + ", toSave.Length: " + toSave.Length);
		//object[] data = new object[toSave.Length];
		for (int i = 0; i < toSave.Length; i++)
		{
			((ISaveable)toSave[i]).SetData((JObject)data[i]);
		}
	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "SaveData";
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
	JObject GetData();
	void SetData(JObject data);
	string GetFileNameBaseForSavingThisComponent();
}

[System.Serializable]
public class SaveDataBasic
{
	public long id;
	/// <summary>
	/// unused
	/// </summary>
	public int sceneIndex;
	public SaveFloat3 position;
	public SaveFloat3 rotation;
}

public struct SaveFloat3
{
	float x, y, z;

	public static implicit operator Vector3(SaveFloat3 s)
	{
		return new Vector3(s.x, s.y, s.z);
	}

	public static implicit operator SaveFloat3(Vector3 v)
	{
		return new SaveFloat3()
		{
			x = v.x,
			y = v.y,
			z = v.z
		};
	}
}

[System.Serializable]
public class SaveDataNPCControl
{
	public bool guard;
	public SaveFloat3 guardPosition;
	public float maxGuardDist;
	public bool playerControlled;
	public string playerOwnerName;
}

[System.Serializable]
public class SaveDataStat
{
	public Stat initialMaxStat;
	public Stat stat;
	public float xp;
	public List<DamageRecord> dmgs;
	public List<string> statSkills;
	public List<int> skillLvls;
	public List<StatRestore> statRestores;

	public SaveDataStat(Stat stat, Stat initialMaxStat, float xp, List<DamageRecord> dmgs, List<string> statSkills, List<int> skillLvls, List<StatRestore> statRestores)
	{
		this.stat = stat;
		this.initialMaxStat = initialMaxStat;
		this.xp = xp;
		this.dmgs = dmgs;
		this.statSkills = statSkills;
		this.skillLvls = skillLvls;
		this.statRestores = statRestores;
	}
}

[System.Serializable]
public class SaveDataAbilities
{
	public List<string> skills;
	public List<int> skillLvls;

	public SaveDataAbilities(List<string> statSkills, List<int> skillLvls)
	{
		this.skills = statSkills;
		this.skillLvls = skillLvls;
	}
}

[System.Serializable]
public class SaveDataInventory
{
	public List<Item> items;

	public SaveDataInventory(List<Item> items)
	{
		this.items = items;
	}
}

public class SaveDataPlayerControl
{
	public string playerOwnerName;//TODO: use player id

	public SaveDataPlayerControl(string playerOwnerName)
	{
		this.playerOwnerName = playerOwnerName;
	}
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