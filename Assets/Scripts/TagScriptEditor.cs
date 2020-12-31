#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TagScript))]
public class TagScriptEditor : Editor
{
	static bool initialized;

	TagScript t;
	bool showTags = false;
	//List<string> tagStrings;
	//List<bool> goodTag;

	private static GUIContent errorContent;
	private void OnEnable()
	{
		Debug.Log("starting");
		((TagScript)target).Awake();
		CheckStartUp();
		//RefreshTags();
	}
	private static void CheckStartUp()
	{
		if (!initialized)
		{
			initialized = true;
			TagScript.InitializeTagMap();
		}
		if (errorContent == null) errorContent = EditorGUIUtility.IconContent("Error");
	}

	//void RefreshTags()
	//{
	//	tagStrings = new List<string>();
	//	goodTag = new List<bool>();
	//	foreach (int i in ((TagScript)target).tags)
	//	{
	//		if (TagScript.TagStringMap.ContainsKey(i))
	//		{
	//			tagStrings.Add(TagScript.TagStringMap[i]);
	//			goodTag.Add(true);
	//		}
	//	}
	//}
	//void UpdateTags()
	//{
	//	t.tags = new List<int>();
	//	for (int i = 0; i < tagStrings.Count; i++)
	//	{
	//		if (TagScript.TagIntMap.ContainsKey(tagStrings[i]))
	//		{
	//			t.tags.Add(TagScript.TagIntMap[tagStrings[i]]);
	//			goodTag[i] = true;
	//		}
	//		else
	//		{
	//			goodTag[i] = false;
	//		}
	//	}
	//}

	public override void OnInspectorGUI()
	{
		t = (TagScript)target;
		
		//bool update = false;
		//if (GUILayout.Button("Get tags from array"))
		//{
		//	RefreshTags();
		//}
		if (showTags = EditorGUILayout.Foldout(showTags, "Tags"))
		{
			TagScriptEditor.TagGUI(ref t.tags);
		}
		//if (update) { 
		//	UpdateTags();   
		//}
		//else
		//{
		//	if (tagStrings.Count != t.tags.Count) RefreshTags();
		//}
		DrawDefaultInspector();
	}

	public static void TagGUI(ref List<int> tags)
	{
		CheckStartUp();
		
		//EditorGUI.indentLevel += 2;
		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		for (int i = 0; i < tags.Count; i++)
		{
			
			//string temp = tagStrings[i];
			GUILayout.BeginHorizontal();
			tags[i] = EditorGUILayout.IntField(tags[i], GUILayout.MaxWidth(50.0f));

			bool suc;
			string temp1 = TagScript.IdTotag(tags[i], out suc);
			string temp = EditorGUILayout.TextArea(temp1);
			if (suc){
				bool succ;
				int id = TagScript.TagToId(temp, out succ);
				if (succ)
				{
					if (temp != temp1)
					{
						tags[i] = id;
						Debug.Log("Changed tag from \"" + temp1 + "\" to \"" + temp + "\"");
					}
				}
				else
				{
					//EditorGUILayout.LabelField(warningContent);
				}
				
			}
			else
			{
				EditorGUILayout.LabelField(errorContent, GUILayout.MaxWidth(50.0f));
			}

			//if (!goodTag[i]) EditorGUILayout.HelpBox("Invalid tag", MessageType.Warning);
			
			if (GUILayout.Button("+", GUILayout.MaxWidth(50.0f)))
			{
				tags.Insert(i + 1, 0);
				break;
			}
			if (GUILayout.Button("-", GUILayout.MaxWidth(50.0f)))
			{
				tags.RemoveAt(i);
				break;
			}
			GUILayout.EndHorizontal();
		}
		if (GUILayout.Button("+"))
		{
			tags.Add(0);
		}
		EditorGUI.indentLevel = indentLevel;
		//EditorGUI.indentLevel -= 2;
	}
}
#endif