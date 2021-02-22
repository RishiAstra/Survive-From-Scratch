using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;
using System.IO;

public enum ThingType
{
	item,
	entity,
}

[System.Serializable]
public class SpawnerSave
{
	public List<long> ids;
}

[RequireComponent(typeof(PersistantSaveID))]
public class spawner : MonoBehaviour {
	public static List<spawner> spawners;

	//public GameObject spawnThis;
	public ThingType thingType;
	public string toSpawnType;
	public float delay;
	public float radius;
	public int maxAmount;
	public int removeNullObjectsSpeed;
	public List<long> IDsOfSpawned;

	private float reload;
	public List<GameObject> spawnedThese = new List<GameObject>();
	private float removeNullTimeLeft;
	private GameObject spawnThis;
	private PersistantSaveID myId;

	private string savePath
	{
		get { return Application.persistentDataPath + "/PersistantId Saves/"; }

	}
	private string saveFile
	{
		get { return savePath + myId.id + ".json"; }

	}
	// Use this for initialization
	void Awake () {
		if (spawners == null) spawners = new List<spawner>();
		spawners.Add(this);
		Save.OnLoadedType += Save_OnLoadedType;
		myId = GetComponent<PersistantSaveID>();
		StartCoroutine(SetSpawnPrefab());
		if (File.Exists(saveFile))
		{
			LoadData(JsonConvert.DeserializeObject<SpawnerSave>(File.ReadAllText(saveFile)));
		}
	}

	void LoadData(SpawnerSave s)
	{
		IDsOfSpawned = s.ids;
	}

	SpawnerSave SaveData()
	{
		return new SpawnerSave
		{
			ids = IDsOfSpawned,
		};
	}

	void SaveDataToFile()
	{
		if (!Directory.Exists(savePath))
		{
			Directory.CreateDirectory(savePath);
		}

		File.WriteAllText(saveFile, JsonConvert.SerializeObject(SaveData(), Formatting.Indented));
	}

	public static void SaveAllSpawners()
	{
		foreach(spawner s in spawners)
		{
			s.SaveDataToFile();
		}
	}

	IEnumerator SetSpawnPrefab()
	{
		AsyncOperationHandle<GameObject> handle = Save.GetPrefab(toSpawnType, thingType);
		yield return handle;
		spawnThis = handle.Result;
	}

	private void Save_OnLoadedType(string type, List<Save> loaded)
	{
		int amountRemembered = 0;
		if (type != toSpawnType) return;//only interested if they're the same type
		foreach(Save s in loaded)
		{
			if (IDsOfSpawned.Contains(s.id))
			{
				spawnedThese.Add(s.gameObject);
				amountRemembered++;
			}
		}
		print("Remembered " + amountRemembered + " of type: " + type);
	}

	// Update is called once per frame
	void Update () {
		removeNullTimeLeft -= Time.deltaTime;
		if (removeNullTimeLeft < 0) {
			for (int i = 0; i < spawnedThese.Count; i++) {
				if (spawnedThese [i] == null) {
					spawnedThese.RemoveAt (i);
					i--;
				}
			}
			removeNullTimeLeft += removeNullObjectsSpeed;
		}

		if(spawnThis != null)
		{
			int maxPerFrame = 10;
			while (reload < 0 && spawnedThese.Count < maxAmount-1 && maxPerFrame >= 0) {
				float a = Random.Range (0, 360);
				float dist = Random.Range (radius, 0);
				Vector3 target = new Vector3(Mathf.Cos(a) * dist, transform.position.y + 10, Mathf.Sin(a) * dist) + transform.position;
				RaycastHit hit;
				if (Physics.Raycast (target, -Vector3.up, out hit)) {
					target = hit.point;		
				}
				GameObject g = (GameObject)Instantiate(spawnThis, target, Quaternion.Euler(0, Random.Range(0, 360), 0));
				spawnedThese.Add(g);
				IDsOfSpawned.Add(g.GetComponent<Save>().id);
				reload += delay;
				maxPerFrame--;
			}
		}

		
		//only reload if it needs to spawn more
		if (spawnedThese.Count < maxAmount) reload -= Time.deltaTime;
		else reload = 0;
	}

	IEnumerator Spawn(string type, Vector3 position, Quaternion rotation)
	{
		yield return SaveEntity.GetEntityPrefab(type);

	}
}
