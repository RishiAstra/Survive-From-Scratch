using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class VersionHandler : MonoBehaviour
{
	public string versionPath
	{
		get { return Application.streamingAssetsPath + "/Versions/"; }
	}
	public string versionFilePath
	{
		get { return Application.streamingAssetsPath + "/Versions/" + version + ".txt"; }
	}

	public string version;
	public TextMeshProUGUI text;
    // Start is called before the first frame update
    void OnEnable()
    {
		if (text != null && File.Exists(versionFilePath))
		{
			text.text = File.ReadAllText(versionFilePath);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
