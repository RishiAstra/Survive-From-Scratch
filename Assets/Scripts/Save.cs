using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Save : MonoBehaviour
{
	const string spawnPath = "Assets/Spawnable/";
	public static string savePath = Application.streamingAssetsPath + "Entities/";


	public static long nextId;
    public long id;
	public string type;
	public bool saveAbilities;

	public Abilities a;

    // Start is called before the first frame update
    void Start()
    {
		if (saveAbilities) a = GetComponent<Abilities>();
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
		string path = savePath + type + "/" + id + ".json";
		File.WriteAllText(path, JsonConvert.SerializeObject(GetData(), Formatting.Indented));
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
			rotation = transform.rotation,
		};
	}

	public void SetData(SaveData data)
	{
		id = data.id;
		a.maxStat = data.maxStat;
		a.stat = data.stat;
		transform.position = data.position;
		transform.rotation = data.rotation;
	}


	public static IEnumerator LoadAll()
	{
		foreach(string typeString in Directory.GetDirectories(savePath))
		{
			string type = typeString.Substring(savePath.Length);
			AsyncOperationHandle<GameObject> toSpawnAsync = Addressables.LoadAssetAsync<GameObject>(spawnPath + type + "/" + type);
			yield return toSpawnAsync;
			GameObject toSpawn = toSpawnAsync.Result;
			foreach (string idPath in Directory.GetFiles(typeString))
			{
				GameObject g = Instantiate(toSpawn);
				SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(idPath));
				g.GetComponent<Save>().SetData(saveData);
			}
		}
		yield return null;
	}
}


[System.Serializable]
public class SaveData
{
    public long id;
    public Stat maxStat;
    public Stat stat;
    public Vector3 position;
    public Quaternion rotation;
}
