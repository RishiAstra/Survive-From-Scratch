using Newtonsoft.Json;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using bobStuff;

//TODO: link to abilities to update when damaged or killed
//TODO: organize all of the paths in a well-defined mannar
//TODO: save the path to player-controlled to the player's data file
//TODO: save player data file including crafting inventory etc.
[RequireComponent(typeof(Abilities))]
public class SaveEntity : Save, ISaveable
{
	public static List<SaveEntity> saves;
	
	const string spawnPath = "Assets/Spawnable/";
	public static string savePath {
		get {
			return Application.persistentDataPath + "/Save/Entities/";
		}
	}

	public string playerOwnerName;

	public string type;
	public string customSavePath;

	public Abilities a;
	public bool deleteOnDeath;

	public ISaveable[] toSave;

	private int indexInSaves;
	private Stat pStat;

	public static void InitializeStatic()
	{
		saves = new List<SaveEntity>();
	}

	// Start is called before the first frame update
	void Awake()
    {
		if (saves == null) InitializeStatic();
		indexInSaves = saves.Count;
		saves.Add(this);

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
		string filePath = GetPath() + id + ".json";
		if (a.dead){
			if (deleteOnDeath){
				if (type == "Player") Debug.LogError("no");
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
					print("Deleted dead entity id: " + id + ", type: " + type);
				}
			}
			else
			{
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

		foreach (string typeString in Directory.GetDirectories(savePath))
		{
			foreach (string idPath in Directory.GetFiles(typeString))
			{

				SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
				if (saveData.id == id)
				{
					return idPath;
				}
			}
		}

		return null;
	}

	public static string GetTypeFromPath(string path)
	{
		int i1 = path.LastIndexOf("/");
		if (i1 == -1) return null;
		string s2 = path.Substring(0, i1);
		int i2 = s2.LastIndexOf("/");
		if (i2 == -1) return null;
		return s2.Substring(i2 + 1);
	}

	public static void TeleportEntityBetweenScenes(long idToMove, string nextScene)
	{
		if (!Directory.Exists(savePath))
		{
			Debug.LogWarning("No save folder");
			return;
		}

		string pathOfThisEntity = GetPathFromId(idToMove);
		if (pathOfThisEntity == null)
		{
			Debug.LogError("This id could not be found to teleport: id: " + idToMove);
			return;
		}

		SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(pathOfThisEntity));
		if (saveData.id == idToMove)
		{
			saveData.scene = nextScene;
			File.WriteAllText(pathOfThisEntity, JsonConvert.SerializeObject(saveData, Formatting.Indented));
		}
		else
		{
			Debug.LogError("Somehow wrong id: expected: " + idToMove + ", found: " + saveData.id);
			return;
		}

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

	public static GameObject LoadEntity(GameObject typePrefab, SaveData saveData)
	{
		GameObject g = Instantiate(typePrefab);
		g.GetComponent<Abilities>().resetOnStart = false;//prevent resetting of hp etc
		//TODO://g.GetComponent<SaveEntity>().SetData(saveData);
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

		foreach (string typeString in Directory.GetDirectories(savePath))
		{
			//TODO: check if this type even exists (if it has a prefab)
			typeCount++;
			string type = typeString.Substring(savePath.Length);
			print("fetching entity prefab: " + type);
			AsyncOperationHandle<GameObject> toSpawnAsync = GetEntityPrefab(type);
			yield return toSpawnAsync;
			GameObject toSpawn = toSpawnAsync.Result;

			List<Save> loadedSaves = new List<Save>();
			foreach (string idPath in Directory.GetFiles(typeString))
			{
				
				SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
				if(saveData.scene == SceneManager.GetActiveScene().name)
				{
					entityCount++;
					//LoadEntity(toSpawn, saveData);
					GameObject g = LoadEntity(toSpawn, saveData); //Instantiate(toSpawn);
																  //g.GetComponent<Abilities>().resetOnStart = false;//prevent resetting of hp etc
																  //g.GetComponent<SaveEntity>().SetData(saveData);

					//TODO: warning: should check if null
					//add the save
					loadedSaves.Add(g.GetComponent<Save>());
				}				
			}
			Save.CallOnLoadedtype(type, loadedSaves);
		}
		print("Loaded entities: " + entityCount + ", " + typeCount + "types");
		yield return null;
	}

	public static AsyncOperationHandle<GameObject> GetEntityPrefab(string type)
	{
		return Addressables.LoadAssetAsync<GameObject>(spawnPath + type + "/" + type + ".prefab");
	}

	public static void SaveAll()
	{
		
		for(int i = 0; i < saves.Count; i++)
		{
			if (saves[i] != null) saves[i].SaveDataToFile();
		}

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
			data[i] = toSave[i].GetData();
		}
		return data;
	}

	public void SetAllData(object[] data)
	{
		if (data.Length != toSave.Length) Debug.LogError("wrong data, data.Length: " + data.Length + ", toSave.Length: " + toSave.Length);
		//object[] data = new object[toSave.Length];
		for (int i = 0; i < toSave.Length; i++)
		{
			toSave[i].SetData(data[i]);
		}
	}
}



#region Save Data Classes

//public interface SaveData
//{
//	void GetDataFromGameObject(ISaveable g);
//	void SetDataFromGameObject(ISaveable g);
//}

public interface ISaveable
{
	object GetData();
	void SetData(object data);
}

//TODO: split into inventory and abilities etc.
[System.Serializable]
public class SaveDataBasic
{
	public long id;
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

//TODO: split into inventory and abilities etc.
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