using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class PersistantSaveID : MonoBehaviour
{
    public static long nextId = 1;

    public static bool readNextId;
    public long id;
    // Start is called before the first frame update
    void Awake()
    {
		TryReadNextID();//make sure that this has been read
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
		if (readNextId) return;

		string path = Application.persistentDataPath + "/PersistandSaveIDnextid.txt";
		//byte[] toWrite = System.Text.Encoding.UTF8.GetBytes(nextId.ToString());
		if (File.Exists(path)) nextId = int.Parse(File.ReadAllText(path));
		readNextId = true;
	}

	public static void SaveNextID()
	{
		//save next id
		string nextIdPath = Application.persistentDataPath + "/PersistandSaveIDnextid.txt";
		File.WriteAllText(nextIdPath, nextId.ToString());
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
