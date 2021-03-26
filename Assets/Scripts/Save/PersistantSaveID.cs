using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class PersistantSaveID : MonoBehaviour
{
    public static long nextId = 1;
	public static string filePath
	{
		get { return Application.streamingAssetsPath + "/PersistandSaveIDnextid.txt"; }//Application.persistentDataPath + "/PersistandSaveIDnextid.txt";
	}

    public static bool readNextId;
    public long id;
    // Start is called before the first frame update
    void Awake()
    {
		TryReadNextID();//make sure that this has been read
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
			id = nextId;
			nextId++;
			Debug.LogWarning("Initialized Persistant ID for \"" + gameObject.name + "\"");
			SaveNextID();
		}
	}

	public static void TryReadNextID()
	{
		//don't read the next id if already has
		//if (readNextId) return;

		//string path = Application.persistentDataPath + "/PersistandSaveIDnextid.txt";
		//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		if (File.Exists(filePath)) nextId = int.Parse(File.ReadAllText(filePath));
		readNextId = true;
		//print("loaded nextid");
	}

	public static void SaveNextID()
	{
		//save next id
		//string nextIdPath = ;
		File.WriteAllText(filePath, nextId.ToString());
		print("saved nextid");
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
