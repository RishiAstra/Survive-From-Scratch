#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TagScript))]
public class TagScriptEditor : Editor
{
	TagScript t;
	bool showTags = false;
	List<string> tagStrings;
	List<bool> goodTag;

	private void OnEnable()
	{
		Debug.Log("starting");
		((TagScript)target).Awake();
		RefreshTags();
	}

	void RefreshTags()
	{
		tagStrings = new List<string>();
		goodTag = new List<bool>();
		foreach (int i in ((TagScript)target).tags)
		{
			if (TagScript.TagStringMap.ContainsKey(i))
			{
				tagStrings.Add(TagScript.TagStringMap[i]);
				goodTag.Add(true);
			}
		}
	}
	void UpdateTags()
	{
		t.tags = new List<int>();
		for (int i = 0; i < tagStrings.Count; i++)
		{
			if (TagScript.TagIntMap.ContainsKey(tagStrings[i]))
			{
				t.tags.Add(TagScript.TagIntMap[tagStrings[i]]);
				goodTag[i] = true;
			}
			else
			{
				goodTag[i] = false;
			}
		}
	}

	public override void OnInspectorGUI()
	{
		t = (TagScript)target;
		
		bool update = false;
		if (GUILayout.Button("Get tags from array"))
		{
			RefreshTags();
		}
		if (showTags = EditorGUILayout.Foldout(showTags, "Tags")) {
			if (GUILayout.Button("+"))
			{
				tagStrings.Add("");
				goodTag.Add(true);
				update = true;
			}
			EditorGUI.indentLevel += 2;
			for (int i = 0; i < tagStrings.Count; i++)
			{
				string temp = tagStrings[i];
				tagStrings[i] = EditorGUILayout.TextArea(tagStrings[i]);
				if (temp != tagStrings[i]) update = true;
				if(!goodTag[i]) EditorGUILayout.HelpBox("Invalid tag", MessageType.Warning);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("+"))
				{
					tagStrings.Insert(i + 1, "");
					goodTag.Insert(i + 1, true);
					update = true;
					break;
				}
				if (GUILayout.Button("-"))
				{
					tagStrings.RemoveAt(i);
					goodTag.RemoveAt(i);
					update = true;
					break;
				}
				GUILayout.EndHorizontal();
			}
			EditorGUI.indentLevel -= 2;
		}
		if (update) { 
			UpdateTags();   
		}
		//else
		//{
		//	if (tagStrings.Count != t.tags.Count) RefreshTags();
		//}
		DrawDefaultInspector();
	}
}
#endif