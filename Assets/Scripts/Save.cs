﻿using Newtonsoft.Json;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Save : MonoBehaviour
{
	public static List<Save> saves;

	const string spawnPath = "Assets/Spawnable/";
	public static string savePath {
		get { return Application.persistentDataPath + "/Scenes/" + SceneManager.GetActiveScene().name + "/Entities/"; }
	}


	public static long nextId = 1;
    public long id;
	public string type;
	//public bool saveAbilities;

	public Abilities a;

    // Start is called before the first frame update
    void Start()
    {
		if (saves == null) saves = new List<Save>();
		saves.Add(this);
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
		return new SaveData
		{
			id = id,
			maxStat = a.maxStat,
			stat = a.stat,
			position = transform.position,
			rotation = transform.eulerAngles,
		};
	}

	public void SetData(SaveData data)
	{
		id = data.id;
		a.maxStat = data.maxStat;
		a.stat = data.stat;
		transform.position = data.position;
		transform.eulerAngles = data.rotation;
	}


	public static IEnumerator LoadAll()
	{
		string path = Application.persistentDataPath + "nextid.txt";
		//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		nextId = int.Parse(File.ReadAllText(path));

		foreach (string typeString in Directory.GetDirectories(savePath))
		{
			string type = typeString.Substring(savePath.Length);
			print("fetching entity prefab: " + type);
			AsyncOperationHandle<GameObject> toSpawnAsync = Addressables.LoadAssetAsync<GameObject>(spawnPath + type + "/" + type + ".prefab");
			yield return toSpawnAsync;
			GameObject toSpawn = toSpawnAsync.Result;
			foreach (string idPath in Directory.GetFiles(typeString))
			{
				GameObject g = Instantiate(toSpawn);
				SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
				g.GetComponent<Abilities>().resetOnStart = false;//prevent resetting of hp etc
				g.GetComponent<Save>().SetData(saveData);
			}
		}
		yield return null;
	}

	public static void SaveAll()
	{
		foreach(Save s in saves)
		{
			s.SaveDataToFile();
		}
		string path = Application.persistentDataPath + "nextid.txt";
		//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		File.WriteAllText(path, nextId.ToString());
	}
}


[System.Serializable]
public class SaveData
{
    public long id;
    public Stat maxStat;
    public Stat stat;
    public Vector3 position;
    public Vector3 rotation;
}
