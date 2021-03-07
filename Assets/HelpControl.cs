using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HelpControl : MonoBehaviour
{
	//public string versionPath
	//{
	//	get { return Application.streamingAssetsPath + "/Versions/"; }
	//}
	//public string versionFilePath
	//{
	//	get { return Application.streamingAssetsPath + "/Versions/" + versions + ".txt"; }
	//}
	public string helpFolderPath
	{
		get { return Application.streamingAssetsPath + "/Help/"; }
	}

	public List<string> helps;
	public TextMeshProUGUI helpLogText;
	public RectTransform helpLogPanel;

	public int helpSelectedIndex;

	public string GetHelpFilePath(string help)
	{
		return helpFolderPath + help + ".txt";
	}

	// Start is called before the first frame update
	void OnEnable()
	{
		if (Directory.Exists(helpFolderPath))
		{
			string[] temp = Directory.GetFiles(helpFolderPath);
			for(int i = 0; i < temp.Length; i++)
			{
				string s = temp[i];
				if(s.IndexOf(".txt") == s.Length - ".txt".Length)
				{
					helps.Add(temp[i].Substring(0, temp[i].Length - ".txt".Length));
				}
			}
		}

		//if (File.Exists(helpFolderPath) && helpLogText != null)
		//{
		//	helps = File.ReadAllLines(helpFolderPath);
		//	StringBuilder sb = new StringBuilder();
		//	for (int i = 0; i < helps.Length; i++)
		//	{
		//		if (File.Exists(GetHelpFilePath(helps[i])))
		//		{
		//			sb.Append("<b>" + helps[i] + "</b>\n" + File.ReadAllText(GetHelpFilePath(helps[i])) + "\n");
		//		}
		//	}
		//	helpLogText.text = sb.ToString();
		//	LayoutRebuilder.ForceRebuildLayoutImmediate(helpLogPanel);
		//}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
