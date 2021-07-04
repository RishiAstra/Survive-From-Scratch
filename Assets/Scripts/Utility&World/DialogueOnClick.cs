/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogueOnClick : MonoBehaviour, IMouseHoverable
{
	//use this to assign new dialogue paths, even if the dialogueonclick isn't in the currently loaded scene
	public static Dictionary<string, string> newDialoguePaths = new Dictionary<string, string>();
	public static List<DialogueOnClick> instances = new List<DialogueOnClick>();


	public GameObject onHover;
	public DialoguePart dialogue;
	[Tooltip("path relative to Streaming Assets/Dialogue/ or as set in DialogueControl.cs")]
	public string dialoguePath;
	public string storagePath;
	public string myName;

	public string dataPath { get { return GameControl.saveDirectory + "Dialogues/" + storagePath; } }
	private void Start()
	{
		instances.Add(this);
		if (File.Exists(dataPath))
		{
			dialoguePath = File.ReadAllText(dataPath);
		}

		if (onHover != null) onHover.SetActive(false);
		UpdateDialogue();
	}

	void OnDestroy()
	{
		instances.Remove(this);
		Directory.CreateDirectory(dataPath.Substring(0, dataPath.LastIndexOf("/")));
		File.WriteAllText(dataPath, dialoguePath);
	}

	public static DialogueOnClick GetInstance(string nameToGet)
	{
		foreach(DialogueOnClick d in instances)
		{
			if(d.myName == nameToGet)
			{
				return d;
			}
		}

		return null;
	}

	private void UpdateDialogue()
	{
		if (newDialoguePaths.ContainsKey(myName))
		{
			dialoguePath = newDialoguePaths[myName];
		}
		dialogue = DialogueControl.GetPartFromFile(dialoguePath);
	}

	public void OnMouseHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(true);
		if (InputControl.InteractKeyDown())
		{
			
			//dialogue = DialogueControl.GetPartFromFile(DialoguePath);
			ProgressTracker.main.RegisterTalk(myName);
			UpdateDialogue();
			DialogueControl.main.StartDialoguePart(dialogue, myName);
		}
	}

	public void OnMouseStopHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
