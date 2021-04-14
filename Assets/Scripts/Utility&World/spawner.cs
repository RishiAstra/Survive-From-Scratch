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

	public enum SpawnShape
	{
		circle,
		rectangle,
	}

	public static List<spawner> spawners;

	//public GameObject spawnThis;
	public ThingType thingType;
	public string toSpawnType;
	public SpawnShape spawnShape;
	public float delay;
	public float radius;
	public float xs, zs;
	public int maxAmount;
	public int initialAmount;
	public int removeNullObjectsSpeed;
	public List<long> IDsOfSpawned;
	public float heightOffset;

	private float reload;
	public List<GameObject> spawnedThese = new List<GameObject>();
	private float removeNullTimeLeft;
	private GameObject spawnThis;
	private PersistantSaveID myId;

	private string savePath
	{
		get { return GameControl.saveDirectory + "/PersistantId Saves/"; }

	}
	private string saveFile
	{
		get { return savePath + myId.id + ".json"; }

	}
	// Use this for initialization
	void Awake ()
	{
		if (spawners == null) spawners = new List<spawner>();
		spawners.Add(this);
		Save.OnLoadedType += Save_OnLoadedType;
		myId = GetComponent<PersistantSaveID>();
		StartCoroutine(SetSpawnPrefab());
		if (File.Exists(saveFile))
		{
			LoadData(JsonConvert.DeserializeObject<SpawnerSave>(File.ReadAllText(saveFile)));
		}

		StartCoroutine(InitialSpawning());
	}

	private IEnumerator InitialSpawning()
	{
		while (spawnThis == null) yield return null;
		//spawn initial amount, considering the amount that already exists
		for (int i = spawnedThese.Count; i < initialAmount; i++)
		{
			SpawnThing();
		}
		yield return null;
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
		if(spawners != null)
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

		if(Save.readEverything && spawnThis != null)
		{
			int maxPerFrame = 10;
			while (reload < 0 && spawnedThese.Count < maxAmount && maxPerFrame >= 0)
			{
				SpawnThing();
				reload += delay;
				maxPerFrame--;
			}
		}

		
		//only reload if it needs to spawn more
		if (spawnedThese.Count < maxAmount) reload -= Time.deltaTime;
		else reload = 0;
	}

	private void SpawnThing()
	{
		Vector3 target = Vector3.zero;
		switch (spawnShape)
		{
			case SpawnShape.circle:
				target = Random.insideUnitCircle * radius;
				break;
			case SpawnShape.rectangle:
				target = new Vector3(Random.Range(-0.5f, 0.5f) * xs, 0, Random.Range(-0.5f, 0.5f) * zs);
				break;
		}
		target += transform.position + Vector3.up * 10;

		//float a = Random.Range(0, 360);
		//float dist = Random.Range(radius, 0);
		//Vector3 target = new Vector3(Mathf.Cos(a) * dist, transform.position.y + 10, Mathf.Sin(a) * dist) + transform.position;
		RaycastHit hit;
		if (Physics.Raycast(target, -Vector3.up, out hit))
		{
			target = hit.point;
		}
		target += Vector3.up * heightOffset;
		GameObject g = (GameObject)Instantiate(spawnThis, target, Quaternion.Euler(0, Random.Range(0, 360), 0));
		spawnedThese.Add(g);
		IDsOfSpawned.Add(g.GetComponent<Save>().id);
	}

	IEnumerator Spawn(string type, Vector3 position, Quaternion rotation)
	{
		yield return SaveEntity.GetEntityPrefab(type);

	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		switch (spawnShape)
		{
			case SpawnShape.circle:
				Gizmos.DrawWireSphere(transform.position, radius);
				break;
			case SpawnShape.rectangle:
				Gizmos.DrawWireCube(transform.position, new Vector3(xs, 2, zs));
				break;
		}
		
	}
}
