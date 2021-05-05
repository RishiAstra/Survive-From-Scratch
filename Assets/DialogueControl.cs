using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class DialogueControl : MonoBehaviour
{
    public static string dialogueDataPath
    {
        get
        {
            return Application.streamingAssetsPath + "/Dialogues/";
        }
    }
    public static DialogueControl main;

    public TextMeshProUGUI dialogueBodyText;
    public TextMeshProUGUI dialogueTitleText;
    public Menu dialogueMenuParent;
    public GameObject choiceButtomPrefab;
    public RectTransform choiceHolder;

    //public DialogueLine currentLine;
    private int currentLineProgress;//used to separate sections of text
    public DialoguePart currentPart;

    private float fadeDurationLeft;
    private float fadeDuriation = 2f;
    // Start is called before the first frame update
    void Awake()
    {
        //singleton
        if (main != null) Debug.LogError("Two Dialogue Controlls");
        main = this;
    }

	public void TryAdvanceDialogue()
	{
        currentLineProgress++;
        
        //if finished going through all the current part's texts
        if (currentLineProgress >= currentPart.texts.Count)
		{
            if(currentPart.choices != null && currentPart.choices.Count > 0)
			{
                //present the user with a choice, so do nothing while waiting for them to choose. This cannot automatically advance.
                currentLineProgress = currentPart.texts.Count - 1;
            }
   //         else if (currentPart.defaultNextPart != null && currentPart.defaultNextPart.texts.Count > 0 && currentPart.defaultNextPart.texts[0] != "")
			//{
   //             //move on without offering a choice
   //             currentPart = currentPart.defaultNextPart;
			//}
			else
			{
                //nothing to move on to, dialogue is finished
                currentPart = null;
                RestartFade();
            }
		}
		else
		{
            RestartFade();
        }

        UpdateDialogue();
        //print("advanced dialogue");
	}

    private void RestartFade()
	{
        fadeDurationLeft = fadeDuriation;
	}

    public void ChooseDialogueOption(int index)
    {
        print("chose dialogue option: " + index);
        currentLineProgress = 0;
        DialoguePart dp = currentPart.choices[index].result;

        //move on if there's something to move on to with this dialogue option, otherwise be done
        if (dp == null || dp.texts.Count == 0)
        {
            currentPart = null;
        }
		else
		{
            currentPart = dp;
            RestartFade();
        }
        UpdateDialogue();
        
    }

    public void UpdateDialogue()
	{
        if(currentPart != null)// && currentPart.texts.Count > 0)
		{
            dialogueMenuParent.TryActivateMenu();//.SetActive(true);
			dialogueBodyText.text = currentPart.texts[currentLineProgress];
            if(currentPart.title != null && currentPart.title != "")
			{
                dialogueTitleText.text = currentPart.title;
			}			
            UpdateDialogueChoices();
		}
        else
		{
            dialogueMenuParent.TryDeactivateMenu();
        }
  //      if(currentLine != null && currentLineProgress < currentLine.parts.Count)
		//{
  //          dialogueMenuParent.TryActivateMenu();//.SetActive(true);
  //          dialogueBodyText.text = currentLine.parts[currentLineProgress].text;
  //          dialogueTitleText.text = currentLine.title;
		//}
		//else
		//{
  //          //call OnFinish
  //          if (currentLine.OnFinish != null)
  //          {
  //              if (currentLineProgress >= currentLine.parts.Count)
  //              {
  //                  currentLine.OnFinish(true);//succeeded
  //              }else{
  //                  currentLine.OnFinish(false);//failed to finish dialogue
		//		}
  //          }

  //          dialogueMenuParent.TryDeactivateMenu();//.SetActive(false);
		//}       
	}

	private void UpdateDialogueChoices()
	{
        for(int i = choiceHolder.childCount - 1; i >= 0; i--)
		{
            Destroy(choiceHolder.GetChild(i).gameObject);
		}
        
        //if this is the last text, show the options
        if(currentLineProgress == currentPart.texts.Count - 1 && currentPart.choices != null)
		{
            for (int i = 0; i < currentPart.choices.Count; i++)
            {
                GameObject g = Instantiate(choiceButtomPrefab, choiceHolder);
                TextMeshProUGUI choiceTitle = g.GetComponentInChildren<TextMeshProUGUI>();
                Button choiceButton = g.GetComponent<Button>();

                //set icon
                if (currentPart.choices[i].icon != null)
                {
                    Image img = g.GetComponentInChildren<Image>();
                    img.sprite = currentPart.choices[i].icon;
                }

                //set the text to describe this choise
                choiceTitle.text = currentPart.choices[i].text;
                //make the choice button choose this dialogue option when clicked
                int throwAwayCopyOfI = i;
                choiceButton.onClick.AddListener(() => ChooseDialogueOption(throwAwayCopyOfI));
            }
        }        

        LayoutRebuilder.ForceRebuildLayoutImmediate(choiceHolder);
    }

	public void StartDialoguePart(DialoguePart line)
	{
        currentLineProgress = 0;
        currentPart = line;
        dialogueTitleText.text = "";//default to no title
        UpdateDialogue();
	}

    // Update is called once per frame
    void Update()
    {
        if (dialogueMenuParent.gameObject.activeSelf) { 
            if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
		    {
                TryAdvanceDialogue();
            }

            dialogueBodyText.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), Mathf.Clamp01(fadeDurationLeft / fadeDuriation));
        }
        fadeDurationLeft -= Time.unscaledDeltaTime;
    }

    public static DialoguePart GetPartFromFile(string path)
	{
        return JsonConvert.DeserializeObject<DialoguePart>(File.ReadAllText(dialogueDataPath + path));
	}
}

[System.Serializable]
public class DialoguePart
{
    
    public string title;
    [TextArea(1, 5)]
    public List<string> texts;
    public List<DialogueChoise> choices;
    //public DialoguePart defaultNextPart;
}

//public delegate void DialogueResult(bool succeeded);

//[System.Serializable]
//public class DialogueLine
//{
//    public string title;
//    public DialoguePart initialPart;
//    public DialogueResult OnFinish;
//}

[System.Serializable]
public class DialogueChoise
{
    public Sprite icon;
    public string text;
    public DialoguePart result;
}
