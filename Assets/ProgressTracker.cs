using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    public static ProgressTracker main;

    public List<IQuest> quests;
    public List<QuestSave> questSaves;
    public GameObject QuestUIPrefab;
    public Transform QuestUIParent;
    public Progress prog;
    public Menu questMenu;
    // Start is called before the first frame update
    void Awake()
    {
        if (main != null) Debug.LogError("Two progress trackers");
        main = this;

        if (quests == null)
        {
            quests = new List<IQuest>();
            questSaves = new List<QuestSave>();
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyUp(KeyCode.F1))
		{
            questMenu.ToggleMenu();
            UpdateQuestUI();
        }		
    }

    public void UpdateQuestData()
	{
        for (int i = 0; i < quests.Count; i++)
        {
            //check if finished, if it is, try to complete mission (including collecting any rewards)
			if (quests[i].IsFinished() && quests[i].TryCompleteMission())
			{

                //if there's a next dialogue, update it
                if (!string.IsNullOrEmpty(questSaves[i].nextDialogueJson) && !string.IsNullOrEmpty(questSaves[i].nextDialogueTargetName))
                {
                    //update the dialogue
                    //DialogueOnClick.newDialoguePaths.Add(questSaves[i].nextDialogueTargetName, questSaves[i].nextDialogueJson);

                    if (DialogueOnClick.newDialoguePaths.ContainsKey(questSaves[i].nextDialogueTargetName))
                    {
                        DialogueOnClick.newDialoguePaths[questSaves[i].nextDialogueTargetName] = questSaves[i].nextDialogueJson;

                    }
                    else
                    {
                        DialogueOnClick.newDialoguePaths.Add(questSaves[i].nextDialogueTargetName, questSaves[i].nextDialogueJson);
                    }

                    print(DialogueOnClick.newDialoguePaths[questSaves[i].nextDialogueTargetName]);
                }


                //if there's a next quest, activate it
                if (!string.IsNullOrEmpty(questSaves[i].nextQuestJson))
                {
                    //load the next quest
                    questSaves[i] = GetQuestSaveFromPath(questSaves[i].nextQuestJson);
                    quests[i] = ConvertQuestSaveToQuest(questSaves[i]);
				}
				else
				{
                    //done with this quest
                    questSaves.RemoveAt(i);
                    quests.RemoveAt(i);
                    i--;
				}

                
			}
        }

        UpdateQuestUI();
    }

    public void UpdateQuestUI()
	{
        if (!questMenu.gameObject.activeSelf) return;//don't update if can't see it
        for (int i = QuestUIParent.childCount - 1; i >= 0; i--)
		{
            Destroy(QuestUIParent.GetChild(i).gameObject);
		}

		for (int i = 0; i < quests.Count; i++)
		{

            GameObject g = Instantiate(QuestUIPrefab, QuestUIParent);

            QuestUI q = g.GetComponent<QuestUI>();
            if(q != null)
			{
                q.title.text = quests[i].GetQuestName();
                q.description.text = quests[i].GetDescription();
			}
		}
    }

    public void RegisterKill(string type, Abilities killed, Abilities killer)
	{
		foreach (IQuest q in quests)
		{
			q.OnEntityKilled(type, killer);
		}

        UpdateQuestData();
        

        if(killer == GameControl.main.myAbilities)
		{
            //mark a kill of this type
            if (prog.TotalKillsByType == null) prog.TotalKillsByType = new Dictionary<string, int>();

            if (prog.TotalKillsByType.ContainsKey(type)){
                prog.TotalKillsByType[type]++;
			}
            else
			{
                prog.TotalKillsByType.Add(type, 1);
			}

            //if the killed entity has tags, mark 1 kill of each tag
            TagScript tags = killed.GetComponent<TagScript>();
            if(tags != null)
			{
                if (prog.TotalKillsByTag == null) prog.TotalKillsByTag = new Dictionary<int, int>();

                foreach (int i in tags.tags)
				{
                    if (prog.TotalKillsByTag.ContainsKey(i))
                    {
                        prog.TotalKillsByTag[i]++;
                    }
                    else
                    {
                        prog.TotalKillsByTag.Add(i, 1);
                    }
                }                
            }
		}
	}

    public void RegisterTalk(string talkedTo)
    {
        foreach (IQuest q in quests)
        {
            q.OnTalked(talkedTo);
        }

        UpdateQuestData();

        prog.talkedTimes++;
    }

    public void RegisterDamage(float amount, Abilities from, string type)
	{
		//foreach (IQuest q in quests)
		//{
  //          q.OnEntityDamaged(type, from, amount);
		//}

  //      if(from != null)
		//{

		//}
	}

	public void TryAddQuest(QuestSave questResult, string fromDialogueName)
	{
        //quest cannot be null, return if it is
        if (questResult == null) return;

        

		IQuest temp = ProgressTracker.ConvertQuestSaveToQuest(questResult);

        //check that this quest is not already being done, if it is, return
        string tempQuestName = temp.GetQuestName();
        foreach(IQuest q in quests)
		{
            if (q.GetQuestName() == tempQuestName) return;
		}

        //add the new quest
        quests.Add(temp);
        questResult.nextDialogueTargetName = fromDialogueName;
        questSaves.Add(questResult);

        print(ConvertQuestToString(temp));
        UpdateQuestUI();

    }

	public static IQuest ConvertStringToQuest(string s)
	{
        QuestSave q = JsonConvert.DeserializeObject<QuestSave>(s);
        //      IQuest result = null;
        //switch (q.type)
        //{
        //          case "KillQuest":
        //              result = (IQuest)Convert.ChangeType(q.data, Type.GetType(q.type));// CastObject<Type.GetType(q.type)>(q.data);// (Type.GetType(q.type))q.data;
        //          break;
        //}
        return ConvertQuestSaveToQuest(q);//
        //return (IQuest)Convert.ChangeType(q.data, Type.GetType(q.type));// result;
	}

    public static IQuest ConvertQuestSaveToQuest(QuestSave q)
	{
        //return (IQuest)(q.data as Type.GetType(q.type));//  Convert.ChangeType(q.data, Type.GetType(q.type));
        IQuest result = null;

        //use this stupid workaround to cast the object to KillQuest
		switch (q.type)
		{
			case "KillQuest":
				result = JsonConvert.DeserializeObject<KillQuest>(JsonConvert.SerializeObject(q.data));// CastObject<Type.GetType(q.type)>(q.data);// (Type.GetType(q.type))q.data;
			break;
            case "TalkQuest":
                result = JsonConvert.DeserializeObject<TalkQuest>(JsonConvert.SerializeObject(q.data));// CastObject<Type.GetType(q.type)>(q.data);// (Type.GetType(q.type))q.data;
            break;
        }

        return result;
	}

    public static string ConvertQuestToString(IQuest q)
	{
        QuestSave result = new QuestSave()
        {
            type = q.GetType().ToString(),
            data = q// JsonConvert.SerializeObject(q, Formatting.Indented)
        };

        return JsonConvert.SerializeObject(result, Formatting.Indented); //result;
    }


    public static QuestSave GetQuestSaveFromPath(string path)
    {
		try
		{
            return JsonConvert.DeserializeObject<QuestSave>(File.ReadAllText(DialogueControl.dialogueDataPath + path));

		}
		catch
		{
            Debug.LogError("failed to get quest: " + DialogueControl.dialogueDataPath + path);
            return null;
		}
    }
}

[System.Serializable]
public class QuestSave
{
    public string type;
    public string nextQuestJson;
    public string nextDialogueJson;
    public string nextDialogueTargetName;
    public object data;
}

[System.Serializable]
public class Progress
{
    public double TotalDamageDealt;
    public double TotalDamageTaken;
    public Dictionary<string, int> TotalKillsByType;
    public Dictionary<int, int> TotalKillsByTag;
    public int talkedTimes;

 //   public Progress()
	//{
 //       TotalDamageDealt = 0;
 //       TotalDamageTaken = 0;
 //       TotalKillsByType = new Dictionary<string, int>();
 //       TotalKillsByTag = new Dictionary<int, int>();
	//}
}
