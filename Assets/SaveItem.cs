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
public class SaveItem : MonoBehaviour
{
	public static List<SaveItem> saves;

	const string spawnPath = "Assets/Items/";
	public static string savePath
	{
		get
		{
			//if(customSavePath == "")
			//{
			return Application.persistentDataPath + "/Scenes/" + SceneManager.GetActiveScene().name + "/Items/";
			//}
			//else
			//{
			//	return Application.persistentDataPath + "/" + customSavePath;
			//}
		}
	}

	public static long nextId = 1;
	public static bool readNextId;
	//public string playerOwnerName;

	public long id;
	//public string type;
	//public string customSavePath;

	public Abilities a;
	public ID myID;

	// Start is called before the first frame update
	void Start()
	{
		if (saves == null) saves = new List<SaveItem>();
		saves.Add(this);
		myID = GetComponent<ID>();

		if (!readNextId)
		{
			string path = Application.persistentDataPath + "/nextidItems.txt";
			//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
			if (File.Exists(path)) nextId = int.Parse(File.ReadAllText(path));
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
		string path = savePath + myID.idString + "/";
		Directory.CreateDirectory(path);
		File.WriteAllText(path + id + ".json", JsonConvert.SerializeObject(GetData(), Formatting.Indented));
	}

	//public void GetDataFromFile()
	//{

	//}

	public SaveDataItem GetData()
	{
		Inventory inventory = GetComponent<Inventory>();
		List<Item> tempItems = inventory != null ? GetComponent<Inventory>().items : null;
		return new SaveDataItem
		{
			id = id,
			position = transform.position,
			rotation = transform.eulerAngles,
		};
	}

	public void SetData(SaveDataItem data)
	{
		id = data.id;
		transform.position = data.position;
		transform.eulerAngles = data.rotation;
	}


	public static IEnumerator LoadAll()
	{
		int typeCount = 0;
		int itemCount = 0;
		if (!Directory.Exists(savePath))
		{
			Debug.LogWarning("Nothing to load for items!");
			yield break;
		}

		foreach (string typeString in Directory.GetDirectories(savePath))
		{
			typeCount++;
			string type = typeString.Substring(savePath.Length);
			print("fetching item prefab: " + type);
			//TODO: save change to auto do this maybe
			//AsyncOperationHandle<GameObject> toSpawnAsync = Addressables.LoadAssetAsync<GameObject>(spawnPath + type + "/" + type + ".prefab");
			//yield return toSpawnAsync;
			//GameObject toSpawn = toSpawnAsync.Result;
			//TODO: saving takes too long!
			GameObject toSpawn = gameControll.itemTypes[gameControll.StringIdMap[type]].prefab;
			foreach (string idPath in Directory.GetFiles(typeString))
			{
				itemCount++;
				GameObject g = Instantiate(toSpawn);
				SaveDataItem saveData = JsonConvert.DeserializeObject<SaveDataItem>(File.ReadAllText(idPath));
				g.GetComponent<SaveItem>().SetData(saveData);
			}
		}
		print("Loaded entities: " + itemCount + ", " + typeCount + "types");
		yield return null;
	}

	public static void SaveAll()
	{
		foreach (SaveItem s in saves)
		{
			s.SaveDataToFile();
		}
		string path = Application.persistentDataPath + "/nextidItems.txt";
		//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		File.WriteAllText(path, nextId.ToString());
	}
}

//TODO: split into inventory and abilities etc.
[System.Serializable]
public class SaveDataItem
{
	public long id;
	public Vector3 position;
	public Vector3 rotation;
}

