using Newtonsoft.Json;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using bobStuff;

//TODO: link to collectible or something to remove it from list automatically
//TODO: save the path to player-controlled to the player's data file
//TODO: save player data file including crafting inventory etc.
[RequireComponent(typeof(ID))]
public class SaveItem : Save
{
	public static List<SaveItem> saves;

	const string spawnPath = "Assets/Items/";
	public static string savePath
	{
		get
		{
			return Application.persistentDataPath + "/Scenes/" + SceneManager.GetActiveScene().name + "/Items/";
		}
	}

	public Abilities a;
	public ID myID;

	private int indexInSaves;

	// Start is called before the first frame update
	void Start()
	{
		if (saves == null) saves = new List<SaveItem>();
		indexInSaves = saves.Count;
		saves.Add(this);

		myID = GetComponent<ID>();

		if (!readNextId)
		{
			string path = Application.persistentDataPath + "/nextidItems.txt";
			if (File.Exists(path)) nextId = int.Parse(File.ReadAllText(path));
			readNextId = true;
		}

		if (id == 0)
		{
			id = nextId;
			nextId++;
		}
	}

	//public void SaveDataToFile()
	//{
	//	string path = savePath + myID.idString + "/";
	//	Directory.CreateDirectory(path);
	//	File.WriteAllText(path + id + ".json", JsonConvert.SerializeObject(GetData(), Formatting.Indented));
	//}

	public SaveDataItem GetData()
	{
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

	//TODO: make all save to 1 file to have better performance
	//TODO: save on a different thread
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
			GameObject toSpawn = GameControl.itemTypes[GameControl.StringIdMap[type]].prefab;
			string typeFilePath = typeString + "/" + type + ".json";
			if (File.Exists(typeFilePath))
			{
				List<SaveDataItem> tempData = JsonConvert.DeserializeObject<List<SaveDataItem>>(File.ReadAllText(typeFilePath));

				foreach (SaveDataItem s in tempData)
				{
					itemCount++;
					GameObject g = Instantiate(toSpawn);
					g.GetComponent<SaveItem>().SetData(s);
				}
			}
			else
			{
				Debug.LogWarning("No file for type:" + type);
			}			
		}
		print("Loaded items: " + itemCount + ", " + typeCount + "types");
		yield return null;
	}

	public static void SaveAll()
	{
		int saveCount = saves.Count;//keep this constant during saving i guess

		//keep track of data to be saved
		List<List<SaveDataItem>> toSave = new List<List<SaveDataItem>>();
		Dictionary<string, int> typeToIndex = new Dictionary<string, int>();

		//go through all the things to be saved
		for (int i = 0; i < saveCount; i++)
		{
			//get item and type
			SaveItem temp = saves[i];
			//don't save items that have already been picked up
			if (temp == null) continue;


			string tempId = temp.myID.idString;

			int index;
			//find list for this type, otherwise add new one for this type
			if (!typeToIndex.TryGetValue(tempId, out index))
			{
				index = toSave.Count;
				toSave.Add(new List<SaveDataItem>());
				typeToIndex.Add(tempId, index);
			}

			//add this data to the type list
			toSave[index].Add(saves[i].GetData());
		}

		//save each to the right name
		foreach (KeyValuePair<string, int> i in typeToIndex)
		{
			string path = savePath + i.Key + "/";
			Directory.CreateDirectory(path);
			File.WriteAllText(path + i.Key + ".json", JsonConvert.SerializeObject(toSave[i.Value], Formatting.Indented));
		}

		//save next id
		string nextIdPath = Application.persistentDataPath + "/nextidItems.txt";
		File.WriteAllText(nextIdPath, nextId.ToString());
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

