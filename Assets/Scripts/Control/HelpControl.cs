/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpControl : MonoBehaviour
{
	private const float SHOW_SOON_DELAY = 1f;
	public static HelpControl main;
	//public string versionPath
	//{
	//	get { return Application.streamingAssetsPath + "/Versions/"; }
	//}
	//public string versionFilePath
	//{
	//	get { return Application.streamingAssetsPath + "/Versions/" + versions + ".txt"; }
	//}
	//public string helpFolderPath
	//{
	//	get { return Application.streamingAssetsPath + "/Help/helptxts/"; }
	//}

	//public string helpCtrlFile
	//{
	//	get { return Application.streamingAssetsPath + "/Help/helplist.txt"; }
	//}

	//public List<string> helps;
	//public TextMeshProUGUI helpLogText;
	//public RectTransform helpLogPanel;
	//public GameObject helpPrefab;
	//public RectTransform helpSelecterParent;
	//public Vector3 helpSelecterSpacing;
	public int helpSelectedIndex;
	public Menu helpParent;
	public List<HelpMenuSelectorUI> helps;


	private void Awake()
	{
		if (main != null) Debug.LogError("2 HelpControl");
		main = this;
		for (int i = 0; i < helps.Count; i++)
		{
			string discard = helps[i].helpName;
			helps[i].GetComponent<Button>().onClick.AddListener(() => ShowHelpMenu(discard));
		}
		ActivateHelpMenuByIndex(0);//make sure that only 1 help menu is open
	}

	/// <summary>
	/// show a help menu with a small delay
	/// </summary>
	/// <param name="name"></param>
	public void ShowHelpMenuSoon(string name)
	{
		StartCoroutine(ShowHelpMenuSoonCoroutine(name));
	}

	private IEnumerator ShowHelpMenuSoonCoroutine(string name)
	{
		yield return new WaitForSeconds(SHOW_SOON_DELAY);
		ShowHelpMenu(name);
	}

	public void ShowHelpMenu(string name)
	{

		//find the help
		int index = -1;
		for (int i = 0; i < helps.Count; i++)
		{
			if (helps[i].helpName == name)
			{
				index = i;
				break;
			}
		}

		//check for error
		if (index == -1)
		{
			Debug.LogError("Couldn't find help menu: " + name);
			return;
		}

		//set active
		ActivateHelpMenuByIndex(index);

		//show the help menu
		helpParent.TryActivateMenu();
	}

	private void ActivateHelpMenuByIndex(int index)
	{
		for (int i = 0; i < helps.Count; i++)
		{
			helps[i].SetSelected(false);
		}
		helps[index].SetSelected(true);
	}

	//public string GetHelpFilePath(string help)
	//{
	//	return helpFolderPath + help + ".txt";
	//}

	//// Start is called before the first frame update
	//void OnEnable()
	//{
	//	helps = new List<string>();

	//	if (File.Exists(helpCtrlFile) && helpPrefab != null && helpSelecterParent != null && helpLogText != null)
	//	{
	//		string[] temp = File.ReadAllLines(helpCtrlFile);

	//		for(int j = helpSelecterParent.childCount - 1; j > 0; j--)
	//		{
	//			DestroyImmediate(helpSelecterParent.GetChild(j).gameObject);
	//		}

	//		if (Directory.Exists(helpFolderPath))
	//		{
	//			//string[] temp = Directory.GetFiles(helpFolderPath);
	//			int amountMade = 0;
	//			List<int> tempIndexes = new List<int>();
	//			for (int i = 0; i < temp.Length; i++)
	//			{
	//				if (File.Exists(GetHelpFilePath(temp[i])))
	//				{
	//					//sb.Append("<b>" + temp[i] + "</b>\n" + File.ReadAllText(GetHelpFilePath(temp[i])) + "\n");
	//					helps.Add(temp[i]);
	//					tempIndexes.Add(i);
	//					int? tempInt = i;

	//					GameObject g = Instantiate(helpPrefab, helpSelecterParent);
	//					g.transform.localPosition = helpSelecterSpacing * amountMade;
	//					g.GetComponentInChildren<TextMeshProUGUI>().text = temp[i];
	//					g.GetComponentInChildren<Button>().onClick.AddListener(() => ChangeIndexSelected((int)tempInt));
	//					amountMade++;
	//				}
	//			}

	//			//for (int i = 0; i < temp.Length; i++)
	//			//{
	//			//	string s = temp[i];
	//			//	if (s.IndexOf(".txt") == s.Length - ".txt".Length)
	//			//	{
	//			//		helps.Add(temp[i].Substring(0, temp[i].Length - ".txt".Length));
	//			//	}

	//			//	GameObject g = Instantiate(helpPrefab, helpSelecterParent);
	//			//	g.transform.localPosition = helpSelecterSpacing * amountMade;
	//			//	amountMade++;
	//			//}
	//		}
	//		LayoutRebuilder.ForceRebuildLayoutImmediate(helpSelecterParent);
	//		ChangeIndexSelected(0);
	//	}
	//}

	//public void ChangeIndexSelected(int index)
	//{
	//	helpSelectedIndex = index;
	//	helpLogText.text = File.ReadAllText(GetHelpFilePath(helps[helpSelectedIndex]));
	//	LayoutRebuilder.ForceRebuildLayoutImmediate(helpLogPanel);
	//}

	//// Update is called once per frame
	//void Update()
	//{

	//}
}
