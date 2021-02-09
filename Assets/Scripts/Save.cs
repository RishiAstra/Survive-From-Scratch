using Newtonsoft.Json;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using bobStuff;

//TODO: save the path to player-controlled to the player's data file
//TODO: save player data file including crafting inventory etc.
public class Save : MonoBehaviour
{
	public static List<Save> saves;

	const string spawnPath = "Assets/Spawnable/";
	public static string savePath {
		get {
			//if(customSavePath == "")
			//{
				return Application.persistentDataPath + "/Scenes/" + SceneManager.GetActiveScene().name + "/Entities/";
			//}
			//else
			//{
			//	return Application.persistentDataPath + "/" + customSavePath;
			//}
		}
	}

	public static long nextId = 1;
	public static bool readNextId;
	public string playerOwnerName;

    public long id;
	public string type;
	public string customSavePath;

	public Abilities a;

    // Start is called before the first frame update
    void Start()
    {
		if (saves == null) saves = new List<Save>();
		saves.Add(this);

		if (!readNextId)
		{
			string path = Application.persistentDataPath + "/nextid.txt";
			//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
			if(File.Exists(path)) nextId = int.Parse(File.ReadAllText(path));
			readNextId = true;
		}

		//if (saveAbilities) a = GetComponent<Abilities>();
		if (id == 0)
		{
			id = nextId;
			nextId++;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	//public string GetPath()
	//{
	//	return  + type + "/" + type;
	//}

	public void SaveDataToFile()
	{
		string path = savePath + type + "/";
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
		if(playerOwnerName == gameControll.username)
		{
			gameControll.main.SetUpPlayer(gameObject);
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
			typeCount++;
			string type = typeString.Substring(savePath.Length);
			print("fetching entity prefab: " + type);
			AsyncOperationHandle<GameObject> toSpawnAsync = Addressables.LoadAssetAsync<GameObject>(spawnPath + type + "/" + type + ".prefab");
			yield return toSpawnAsync;
			GameObject toSpawn = toSpawnAsync.Result;
			foreach (string idPath in Directory.GetFiles(typeString))
			{
				entityCount++;
				GameObject g = Instantiate(toSpawn);
				SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
				g.GetComponent<Abilities>().resetOnStart = false;//prevent resetting of hp etc
				g.GetComponent<Save>().SetData(saveData);
			}
		}
		print("Loaded entities: " + entityCount + ", " + typeCount + "types");
		yield return null;
	}

	public static void SaveAll()
	{
		foreach(Save s in saves)
		{
			s.SaveDataToFile();
		}
		string path = Application.persistentDataPath + "/nextid.txt";
		//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		File.WriteAllText(path, nextId.ToString());
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
