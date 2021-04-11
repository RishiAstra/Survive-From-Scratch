using bobStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class Save : MonoBehaviour
{
	public static bool readEverything;
	public static long nextId = 1;
	public static bool readNextId;
	public long id;

	public delegate void LoadedType(string type, List<Save> loaded);
	public static event LoadedType OnLoadedType;

	public static void CallOnLoadedtype(string type, List<Save> loaded)
	{
		if (OnLoadedType != null) OnLoadedType.Invoke(type, loaded);
		else Debug.LogWarning("No listeners for OnLoadedType");
	}

	public static void TryReadNextID()
	{
		//don't read the next id if already has
		if (readNextId) return;

		string path = Application.persistentDataPath + "/nextid.txt";
		//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		if (File.Exists(path)) nextId = int.Parse(File.ReadAllText(path));
		readNextId = true;
	}

	public static void SaveNextID()
	{
		//save next id
		string nextIdPath = Application.persistentDataPath + "/nextid.txt";
		File.WriteAllText(nextIdPath, nextId.ToString());
	}

	public static AsyncOperationHandle<GameObject> GetPrefab(string type, ThingType thingType)
	{
		if(thingType == ThingType.entity)
		{
			return SaveEntity.GetEntityPrefab(type);
		}else// if(thingType == ThingType.item)
		{
			return SaveItem.GetItemPrefab(type);
		}
	}

	public static void SaveAllData()
	{
		//save both entities and items
		SaveItem.SaveAll();
		SaveEntity.SaveAll();

		//remember the next id
		SaveNextID();
	}

	public static IEnumerator LoadAllData()
	{
		//start both
		IEnumerator itemLoad = SaveItem.LoadAll();
		IEnumerator entityLoad = SaveEntity.LoadAll();

		//wait for both
		yield return itemLoad;
		yield return entityLoad;


		//remember the next id
		TryReadNextID();
		readEverything = true;
	}

	public static void Initialize()
	{
		SaveItem.InitializeStatic();
		SaveEntity.InitializeStatic();
	}
}