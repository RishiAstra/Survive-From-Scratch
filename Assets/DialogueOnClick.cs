using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnClick : MonoBehaviour, IMouseHoverable
{
	public GameObject onHover;
	public DialogueLine dialogue;

	private void Start()
	{
		if(onHover != null) onHover.SetActive(false);
	}

	public void OnMouseHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(true);
		if (Input.GetMouseButtonDown(0))
		{
			DialogueControl.main.StartDialogueLine(dialogue);
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
