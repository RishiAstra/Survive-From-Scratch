﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnClick : MonoBehaviour, IMouseHoverable
{
	public GameObject onHover;
	public DialoguePart dialogue;
	[Tooltip("path relative to Streaming Assets/Dialogue/ or as set in DialogueControl.cs")]
	public string DialoguePath;

	private void Start()
	{
		if(onHover != null) onHover.SetActive(false);
		dialogue = DialogueControl.GetPartFromFile(DialoguePath);
	}

	public void OnMouseHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(true);
		if (Input.GetMouseButtonDown(0))
		{
			DialogueControl.main.StartDialoguePart(dialogue);
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
