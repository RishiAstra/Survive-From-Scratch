using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnClick : MonoBehaviour, IMouseHoverable
{
	//use this to assign new dialogue paths, even if the dialogueonclick isn't in the currently loaded scene
	public static Dictionary<string, string> newDialoguePaths = new Dictionary<string, string>();

	public GameObject onHover;
	public DialoguePart dialogue;
	[Tooltip("path relative to Streaming Assets/Dialogue/ or as set in DialogueControl.cs")]
	public string DialoguePath;
	public string myName;

	private void Start()
	{
		if (onHover != null) onHover.SetActive(false);
		UpdateDialogue();
	}

	private void UpdateDialogue()
	{
		if (newDialoguePaths.ContainsKey(myName))
		{
			DialoguePath = newDialoguePaths[myName];
		}
		dialogue = DialogueControl.GetPartFromFile(DialoguePath);
	}

	public void OnMouseHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(true);
		if (Input.GetMouseButtonDown(0))
		{
			UpdateDialogue();
			//dialogue = DialogueControl.GetPartFromFile(DialoguePath);
			ProgressTracker.main.RegisterTalk(myName);
			DialogueControl.main.StartDialoguePart(dialogue, this);
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
