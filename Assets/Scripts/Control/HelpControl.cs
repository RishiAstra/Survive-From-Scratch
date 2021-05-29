/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
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
		get { return Application.streamingAssetsPath + "/Help/helptxts/"; }
	}

	public string helpCtrlFile
	{
		get { return Application.streamingAssetsPath + "/Help/helplist.txt"; }
	}

	public List<string> helps;
	public TextMeshProUGUI helpLogText;
	public RectTransform helpLogPanel;
	public GameObject helpPrefab;
	public RectTransform helpSelecterParent;
	public Vector3 helpSelecterSpacing;

	public int helpSelectedIndex;

	public string GetHelpFilePath(string help)
	{
		return helpFolderPath + help + ".txt";
	}

	// Start is called before the first frame update
	void OnEnable()
	{
		helps = new List<string>();

		if (File.Exists(helpCtrlFile) && helpPrefab != null && helpSelecterParent != null && helpLogText != null)
		{
			string[] temp = File.ReadAllLines(helpCtrlFile);

			for(int j = helpSelecterParent.childCount - 1; j > 0; j--)
			{
				DestroyImmediate(helpSelecterParent.GetChild(j).gameObject);
			}

			if (Directory.Exists(helpFolderPath))
			{
				//string[] temp = Directory.GetFiles(helpFolderPath);
				int amountMade = 0;
				List<int> tempIndexes = new List<int>();
				for (int i = 0; i < temp.Length; i++)
				{
					if (File.Exists(GetHelpFilePath(temp[i])))
					{
						//sb.Append("<b>" + temp[i] + "</b>\n" + File.ReadAllText(GetHelpFilePath(temp[i])) + "\n");
						helps.Add(temp[i]);
						tempIndexes.Add(i);
						int? tempInt = i;

						GameObject g = Instantiate(helpPrefab, helpSelecterParent);
						g.transform.localPosition = helpSelecterSpacing * amountMade;
						g.GetComponentInChildren<TextMeshProUGUI>().text = temp[i];
						g.GetComponentInChildren<Button>().onClick.AddListener(() => ChangeIndexSelected((int)tempInt));
						amountMade++;
					}
				}

				//for (int i = 0; i < temp.Length; i++)
				//{
				//	string s = temp[i];
				//	if (s.IndexOf(".txt") == s.Length - ".txt".Length)
				//	{
				//		helps.Add(temp[i].Substring(0, temp[i].Length - ".txt".Length));
				//	}

				//	GameObject g = Instantiate(helpPrefab, helpSelecterParent);
				//	g.transform.localPosition = helpSelecterSpacing * amountMade;
				//	amountMade++;
				//}
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(helpSelecterParent);
			ChangeIndexSelected(0);
		}
	}

	public void ChangeIndexSelected(int index)
	{
		helpSelectedIndex = index;
		helpLogText.text = File.ReadAllText(GetHelpFilePath(helps[helpSelectedIndex]));
		LayoutRebuilder.ForceRebuildLayoutImmediate(helpLogPanel);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
