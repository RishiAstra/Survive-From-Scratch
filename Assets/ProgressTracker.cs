using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    public static ProgressTracker main;

    public List<IQuest> quests;

    public Progress prog;
    // Start is called before the first frame update
    void Awake()
    {
        if (main != null) Debug.LogError("Two progress trackers");
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterKill(string type, Abilities killed, Abilities killer)
	{
		foreach (IQuest q in quests)
		{
			q.OnEntityKilled(type, killer);
		}

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

	public void TryAddQuest(QuestSave questResult)
	{
        //quest cannot be null, return if it is
        if (questResult == null) return;

        if (quests == null) quests = new List<IQuest>();

        IQuest temp = ProgressTracker.ConvertQuestSaveToQuest(questResult);

        //check that this quest is not already being done, if it is, return
        string tempQuestName = temp.GetQuestName();
        foreach(IQuest q in quests)
		{
            if (q.GetQuestName() == tempQuestName) return;
		}

        //add the new quest
        quests.Add(temp);

        print(ConvertQuestToString(temp));

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

		switch (q.type)
		{
			case "KillQuest":
				result = (KillQuest)q.data;// CastObject<Type.GetType(q.type)>(q.data);// (Type.GetType(q.type))q.data;
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
}

[System.Serializable]
public class QuestSave
{
    public string type;
    public object data;
}

[System.Serializable]
public class Progress
{
    public double TotalDamageDealt;
    public double TotalDamageTaken;
    public Dictionary<string, int> TotalKillsByType;
    public Dictionary<int, int> TotalKillsByTag;

 //   public Progress()
	//{
 //       TotalDamageDealt = 0;
 //       TotalDamageTaken = 0;
 //       TotalKillsByType = new Dictionary<string, int>();
 //       TotalKillsByTag = new Dictionary<int, int>();
	//}
}
