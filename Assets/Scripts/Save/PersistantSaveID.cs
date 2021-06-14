/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PersistantSaveID : MonoBehaviour
{
    public static List<long> takenIDs;
	public static string filePath
	{
		get { return Application.streamingAssetsPath + "/PersistandSaveIDnextid.txt"; }//Application.persistentDataPath + "/PersistandSaveIDnextid.txt";
	}

    public static bool readNextId;
    public long id;
    // Start is called before the first frame update
    void Awake()
    {
		//TryReadNextID();//make sure that this has been read
		CheckID();
		//else
		//{
		//	if (id >= nextId)
		//	{
		//		nextId = id + 1;//correct the count
		//	}
		//}
	}

	private void Reset()
	{
		CheckID();
	}

	void CheckID()
	{
		if (id == 0)
		{
			if (File.Exists(filePath)) takenIDs = Array.ConvertAll(File.ReadAllLines(filePath), s => long.Parse(s)).ToList();
			else takenIDs = new List<long>();

			for (int i = 1; i < 10000; i++)
			{
				if (!takenIDs.Contains(i))
				{
					id = i;
					takenIDs.Add(i);
					File.WriteAllLines(filePath, Array.ConvertAll<long, string>(takenIDs.ToArray(), s => s.ToString()));
					break;
				}
				if(i == 9999)
				{
					Debug.LogError("Can't find available persistant id");
				}
			}
			//id = takenIDs;
			//takenIDs++;
			Debug.LogWarning("Initialized Persistant ID for \"" + gameObject.name + "\"");
			//SaveNextID();
		}
	}

	//public static void TryReadNextID()
	//{
	//	//don't read the next id if already has
	//	//if (readNextId) return;

	//	//string path = Application.persistentDataPath + "/PersistandSaveIDnextid.txt";
	//	//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
	//	if (File.Exists(filePath)) takenIDs = Array.ConvertAll(File.ReadAllLines(filePath), s => long.Parse(s)).ToList();
	//	readNextId = true;
	//	//print("loaded nextid");
	//}

	//public static void SaveNextID()
	//{
	//	//save next id
	//	//string nextIdPath = ;
	//	File.WriteAllText(filePath, takenIDs.ToString());
	//	print("saved nextid");
	//}

	// Update is called once per frame
	void Update()
    {
        
    }
}
