using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
	public string versionInfoPath
	{
		get { return Application.streamingAssetsPath + "/Versions/current.txt"; }
	}

	public string version;
	public TextMeshProUGUI changeLogText;
	public RectTransform changeLogPanel;
    // Start is called before the first frame update
    void OnEnable()
    {
		if (File.Exists(versionInfoPath))
		{
			version = File.ReadAllText(versionInfoPath);
			if (changeLogText != null && File.Exists(versionFilePath))
			{
				changeLogText.text = File.ReadAllText(versionFilePath);
				LayoutRebuilder.ForceRebuildLayoutImmediate(changeLogPanel);
			}
		}
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
