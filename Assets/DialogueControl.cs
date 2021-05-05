using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueControl : MonoBehaviour
{
    public static DialogueControl main;

    public TextMeshProUGUI dialogueBodyText;
    public TextMeshProUGUI dialogueTitleText;
    public Menu dialogueMenuParent;

    public DialogueLine currentLine;
    private int currentLineProgress;
    // Start is called before the first frame update
    void Start()
    {
        //singleton
        if (main != null) Debug.LogError("Two Dialogue Controlls");
        main = this;


    }

	public void AdvanceDialogue()
	{
        currentLineProgress++;
        UpdateDialogue();
        print("advanced dialogue");
	}

    public void UpdateDialogue()
	{
        if(currentLine != null && currentLineProgress < currentLine.parts.Count)
		{
            dialogueMenuParent.TryActivateMenu();//.SetActive(true);
            dialogueBodyText.text = currentLine.parts[currentLineProgress].text;
            dialogueTitleText.text = currentLine.title;
		}
		else
		{
            //call OnFinish
            if (currentLine.OnFinish != null)
            {
                if (currentLineProgress >= currentLine.parts.Count)
                {
                    currentLine.OnFinish(true);//succeeded
                }else{
                    currentLine.OnFinish(false);//failed to finish dialogue
				}
            }

            dialogueMenuParent.TryDeactivateMenu();//.SetActive(false);
		}       
	}

    public void StartDialogueLine(DialogueLine line)
	{
        currentLineProgress = 0;
        currentLine = line;
        UpdateDialogue();
	}

    // Update is called once per frame
    void Update()
    {
        if(dialogueMenuParent.gameObject.activeSelf && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
		{
            AdvanceDialogue();
		}
    }
}

[System.Serializable]
public class DialoguePart
{
    [TextArea(1, 5)]
    public string text;
}

public delegate void DialogueResult(bool succeeded);

[System.Serializable]
public class DialogueLine
{
    public string title;
    public List<DialoguePart> parts;
    public DialogueResult OnFinish;
}
