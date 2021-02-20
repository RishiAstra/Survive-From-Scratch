using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum ThingType
{
	item,
	entity,
}

[RequireComponent(typeof(PersistantSaveID))]
public class spawner : MonoBehaviour {

	//public GameObject spawnThis;
	public ThingType thingType;
	public string toSpawnType;
	public float delay;
	public float radius;
	public int maxAmount;
	public int removeNullObjectsSpeed;
	public List<int> IDsOfSpawned;

	private float reload;
	private List<GameObject> spawnedThese = new List<GameObject>();
	private float removeNullTimeLeft;
	private GameObject spawnThis;
	// Use this for initialization
	void Awake () {
		Save.OnLoadedType += Save_OnLoadedType;
	}

	IEnumerator SetSpawnPrefab()
	{
		AsyncOperationHandle<GameObject> handle = Save.GetPrefab(toSpawnType, thingType);
		yield return handle;
		spawnThis = handle.Result;
	}

	private void Save_OnLoadedType(string type, List<Save> loaded)
	{
		if (type != toSpawnType) return;//only interested if they're the same type
		foreach(Save s in loaded)
		{
			if (IDsOfSpawned.Contains(s.id))
			{
				throw new NotImplimentasdfadfas
			}
		}
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
			while (reload < 0 && spawnedThese.Count < maxAmount && maxPerFrame >= 0) {
				float a = Random.Range (0, 360);
				float dist = Random.Range (radius, 0);
				Vector3 target = new Vector3(Mathf.Cos(a) * dist, transform.position.y + 10, Mathf.Sin(a) * dist) + transform.position;
				RaycastHit hit;
				if (Physics.Raycast (target, -Vector3.up, out hit)) {
					target = hit.point;		
				}
				spawnedThese.Add((GameObject)Instantiate(spawnThis, target, Quaternion.Euler(0, Random.Range(0, 360), 0)));
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
