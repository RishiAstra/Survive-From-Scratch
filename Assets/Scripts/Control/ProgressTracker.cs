/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
	public static ProgressTracker main;
	public static string savePath { get { return GameControl.saveDirectory + "ProgressTracker/"; } }
	public static string questSavePath { get { return savePath + "quests/"; } }
	public static string activateSavePath { get { return savePath + "Quest Activates/"; } }
	public static string activateSaveFile { get { return activateSavePath + "data.json"; } }

	public List<IQuest> quests;
	public List<QuestSave> questSaves;
	public GameObject QuestUIPrefab;
	public Transform QuestUIParent;
	public Progress prog;
	public Menu questMenu;
	public Dictionary<string, QuestGameObjectData> activates = new Dictionary<string, QuestGameObjectData>();

	// Start is called before the first frame update
	void Awake()
	{
		if (main != null) Debug.LogError("Two progress trackers");
		main = this;

		LoadAllProgressData();

		if (quests == null)
		{
			quests = new List<IQuest>();
			questSaves = new List<QuestSave>();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1) && !Menu.IsOtherMenuActive(questMenu))
		{
			questMenu.ToggleMenu();
			print("toggled");
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
		QuestGameObjectActivate.CheckAll();
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

	public void RegisterKill(string type, StatScript killed, Abilities killer)
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
		NotificationControl.main.AddNotification(
				new Notification()
				{
					message = temp.GetDescription()
				}
			);
		if (!string.IsNullOrEmpty(fromDialogueName)) questResult.nextDialogueTargetName = fromDialogueName;
		questSaves.Add(questResult);

		print(ConvertQuestToString(temp));
		UpdateQuestUI();

	}

	//public static IQuest ConvertStringToQuest(string s)
	//{
	//	QuestSave q = JsonConvert.DeserializeObject<QuestSave>(s);
	//	//      IQuest result = null;
	//	//switch (q.type)
	//	//{
	//	//          case "KillQuest":
	//	//              result = (IQuest)Convert.ChangeType(q.data, Type.GetType(q.type));// CastObject<Type.GetType(q.type)>(q.data);// (Type.GetType(q.type))q.data;
	//	//          break;
	//	//}
	//	return ConvertQuestSaveToQuest(q);//
	//	//return (IQuest)Convert.ChangeType(q.data, Type.GetType(q.type));// result;
	//}

	public static IQuest ConvertQuestSaveToQuest(QuestSave q)
	{
		//return (IQuest)(q.data as Type.GetType(q.type));//  Convert.ChangeType(q.data, Type.GetType(q.type));
		//IQuest result = null;

		//use this stupid workaround to cast the object to KillQuest
		return GetIQuestFromStringAndType(q.data, q.type);
	}

	private static IQuest GetIQuestFromStringAndType(object data, string type)
	{
		IQuest result = null;

		switch (type)
		{
			//TODO: use JObject.FromObject instead of serilizing and deserilizing etc.
			case "KillQuest":
				result = JsonConvert.DeserializeObject<KillQuest>(JsonConvert.SerializeObject(data));// CastObject<Type.GetType(q.type)>(q.data);// (Type.GetType(q.type))q.data;
				break;
			case "TalkQuest":
				result = JsonConvert.DeserializeObject<TalkQuest>(JsonConvert.SerializeObject(data));// CastObject<Type.GetType(q.type)>(q.data);// (Type.GetType(q.type))q.data;
				break;
			case "ComplexQuest":
				JObject obj = JObject.FromObject(data);
				JArray quests = obj["quests"] as JArray;
				//make null so the later deserilization doesn't handle this
				obj["quests"] = null;

				List<IQuest> tempQuestList = new List<IQuest>();

				for (int i = 0; i < quests.Count; i++)
				{
					//WARNING: recursive
					tempQuestList.Add(ConvertQuestSaveToQuest(quests[i].ToObject<QuestSave>()));
				}

				ComplexQuest cq = obj.ToObject<ComplexQuest>();
				cq.quests = tempQuestList;

				result = cq;
				break;
		}

		return result;
	}

	public static string ConvertQuestToString(IQuest q)
	{
		string myType = q.GetType().ToString();
		QuestSave result = new QuestSave()
		{
			type = myType,
			data = q// JsonConvert.SerializeObject(q, Formatting.Indented)
		};

		if(myType == "ComplexQuest")
		{
			ComplexQuest complex = q as ComplexQuest;
			JObject obj = JObject.FromObject(result);

			JArray a = new JArray();

			List<IQuest> tempQuestList = new List<IQuest>();

			for (int i = 0; i < complex.quests.Count; i++)
			{

				string t = complex.quests[i].GetType().ToString();
				QuestSave r = new QuestSave()
				{
					type = t,
					data = complex.quests[i]// JsonConvert.SerializeObject(q, Formatting.Indented)
				};
				a.Add(JToken.FromObject(r));
			}

			obj["data"]["quests"] = a;

			return obj.ToString(Formatting.Indented);
		}
		else
		{
			return JsonConvert.SerializeObject(result, Formatting.Indented); //result;
		}

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

	private void OnDestroy()
	{
		QuestGameObjectActivate.CheckSaveAll();
		//delete old quest save
		if(Directory.Exists(questSavePath)) Directory.Delete(questSavePath, true);
		//re-make the directory
		Directory.CreateDirectory(questSavePath);
		for(int i = 0; i < quests.Count; i++)
		{
			File.WriteAllText(questSavePath + i + ".json", ConvertQuestToString(quests[i]));
		}
		File.WriteAllText(questSavePath + "saves.json", JsonConvert.SerializeObject(questSaves, Formatting.Indented));

		File.WriteAllText(savePath + "progress.json", JsonConvert.SerializeObject(prog, Formatting.Indented));

		File.WriteAllText(savePath + "questDialogueUpdates.json", JsonConvert.SerializeObject(DialogueOnClick.newDialoguePaths));

		Directory.CreateDirectory(activateSavePath);
		File.WriteAllText(activateSaveFile, JsonConvert.SerializeObject(activates));
	}

	void LoadAllProgressData()
	{
		quests = new List<IQuest>();
		string questsavesSavePath = questSavePath + "saves.json";
		if (Directory.Exists(questSavePath))
		{

			for (int i = 0; i < 10000; i++)
			{
				string thisQuestPath = questSavePath + i + ".json";
				if (File.Exists(thisQuestPath))
				{
					QuestSave q = JsonConvert.DeserializeObject<QuestSave>(File.ReadAllText(thisQuestPath));
					TryAddQuest(q, null);
					//quests.Add(ConvertStringToQuest());
				}
				else
				{
					break;
				}
			}
			questSaves = JsonConvert.DeserializeObject<List<QuestSave>>(File.ReadAllText(questsavesSavePath));
		}

		string newDialoguePathsSavePath = savePath + "questDialogueUpdates.json";
		if(File.Exists(newDialoguePathsSavePath)) DialogueOnClick.newDialoguePaths = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(newDialoguePathsSavePath));

		string progressSavePath = savePath + "progress.json";

		if (File.Exists(progressSavePath))
		{
			prog = JsonConvert.DeserializeObject<Progress>(File.ReadAllText(progressSavePath)); //File.WriteAllText(progressSavePath, JsonConvert.SerializeObject(prog));
		}


		if (File.Exists(activateSaveFile))
		{
			activates = JsonConvert.DeserializeObject<Dictionary<string, QuestGameObjectData>>(File.ReadAllText(activateSaveFile)); //File.WriteAllText(progressSavePath, JsonConvert.SerializeObject(prog));
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
