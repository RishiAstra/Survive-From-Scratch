using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class VersionHandler : MonoBehaviour
{
	//public string versionPath
	//{
	//	get { return Application.streamingAssetsPath + "/Versions/"; }
	//}
	//public string versionFilePath
	//{
	//	get { return Application.streamingAssetsPath + "/Versions/" + versions + ".txt"; }
	//}
	public string versionInfoPath
	{
		get { return Application.streamingAssetsPath + "/Versions/current.txt"; }
	}

	public string[] versions;
	public TextMeshProUGUI changeLogText;
	public RectTransform changeLogPanel;

	public string GetVersionFilePath(string version)
	{
		return Application.streamingAssetsPath + "/Versions/" + version + ".txt";
	}

    // Start is called before the first frame update
    void OnEnable()
    {
		if (File.Exists(versionInfoPath) && changeLogText != null)
		{
			versions = File.ReadAllLines(versionInfoPath);
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < versions.Length; i++)
			{
				if (File.Exists(GetVersionFilePath(versions[i])))
				{
					sb.Append("<b>" + versions[i] + "</b>\n" + File.ReadAllText(GetVersionFilePath(versions[i])) + "\n");
				}
			}
			changeLogText.text = sb.ToString();
			LayoutRebuilder.ForceRebuildLayoutImmediate(changeLogPanel);
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
