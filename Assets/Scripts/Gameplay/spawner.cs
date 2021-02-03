using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour {
	public GameObject spawnThis;
	public float delay;
	public float radius;
	public int maxAmount;
	public int removeNullObjectsSpeed;

	private float reload;
	private List<GameObject> spawnedThese = new List<GameObject>();
	private float removeNullTimeLeft;
	// Use this for initialization
	void Start () {
		
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
		//only reload if it needs to spawn more
		if (spawnedThese.Count < maxAmount) reload -= Time.deltaTime;
		else reload = 0;
	}
}
