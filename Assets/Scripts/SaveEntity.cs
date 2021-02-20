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
public class SaveEntity : Save
{
	public static List<SaveEntity> saves;
	
	const string spawnPath = "Assets/Spawnable/";
	public static string savePath {
		get {
			return Application.persistentDataPath + "/Scenes/" + SceneManager.GetActiveScene().name + "/Entities/";
		}
	}

	public string playerOwnerName;

	public string type;
	public string customSavePath;

	public Abilities a;
	public bool deleteOnDeath;


	private int indexInSaves;
	private Stat pStat;


	// Start is called before the first frame update
	void Start()
    {
		if (saves == null) saves = new List<SaveEntity>();
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
			print("autosaved data for entity id: " + id);
			pStat = a.stat;
			SaveDataToFile();
		}
	}

	//TODO: warning: the dead body will get deleted even if it's supposed to stay as a dead body for a few seconds
	private void OnDestroy()
	{
		string filePath = GetPath() + id + ".json";
		if (a.dead && deleteOnDeath && File.Exists(filePath))
		{
			File.Delete(filePath);
			print("Deleted dead entity id: " + id);
		}
	}

	public string GetPath()
	{
		return savePath + type + "/";
	}

	public void SaveDataToFile()
	{
		string path = GetPath();
		Directory.CreateDirectory(path);
		File.WriteAllText(path + id + ".json", JsonConvert.SerializeObject(GetData(), Formatting.Indented));
	}

	//public void GetDataFromFile()
	//{

	//}

	public SaveData GetData()
	{
		Inventory inventory = GetComponent<Inventory>();
		List<Item> tempItems = inventory != null ? GetComponent<Inventory>().items : null;
		return new SaveData
		{
			id = id,
			maxStat = a.maxStat,
			stat = a.stat,
			position = transform.position,
			rotation = transform.eulerAngles,
			playerOwnerName = playerOwnerName,
			items = tempItems,
		};
	}

	public void SetData(SaveData data)
	{
		id = data.id;
		a.maxStat = data.maxStat;
		a.stat = data.stat;
		transform.position = data.position;
		transform.eulerAngles = data.rotation;
		playerOwnerName = data.playerOwnerName;
		if(playerOwnerName == GameControl.username)
		{
			GameControl.main.SetUpPlayer(gameObject);
		}
		List<Item> tempItems = data.items;
		Inventory inventory = GetComponent<Inventory>();
		if (tempItems != null)
		{
			if (inventory == null)
			{
				Debug.LogError("An Inventory was loaded, but there is not one to load it to on this GameObject");
			}
			else
			{
				inventory.items = tempItems;
			}
		}
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
				entityCount++;
				GameObject g = Instantiate(toSpawn);
				SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
				g.GetComponent<Abilities>().resetOnStart = false;//prevent resetting of hp etc
				g.GetComponent<SaveEntity>().SetData(saveData);

				//TODO: warning: should check if null
				//add the save
				loadedSaves.Add(g.GetComponent<Save>());
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
}

//TODO: split into inventory and abilities etc.
[System.Serializable]
public class SaveData
{
    public long id;
    public Stat maxStat;
    public Stat stat;
    public Vector3 position;
    public Vector3 rotation;
	public string playerOwnerName;
	public List<Item> items;
}
