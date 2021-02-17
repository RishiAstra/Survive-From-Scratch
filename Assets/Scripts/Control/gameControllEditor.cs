#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using bobStuff;
using System.IO;

[CustomEditor(typeof(GameControl))]
[System.Serializable]
public class gameControllEditor : Editor
{
	public static bool showTypes = false;

	public static List<bool> show = new List<bool>();
	GameControl t;
	public override void OnInspectorGUI()
	{
		t = (GameControl)target;
		GameControl.CheckItemTypes();

		DrawDefaultInspector();
		UpdateShowList();		
			
		showTypes = EditorGUILayout.Foldout(showTypes, "Item Types");
		EditorGUI.indentLevel += 2;
		if (showTypes)
		{
			//bool changed = false;
			for(int i = 0; i < GameControl.itemTypes.Count; i++)
			{
				show[i] = EditorGUILayout.Foldout(show[i], i + " : " + GameControl.itemTypes[i].name);
				if (show[i])
				{
					EditorGUI.indentLevel += 2;
					ItemType item = GameControl.itemTypes[i];
					item.name = EditorGUILayout.TextField("name", item.name);
					//public Texture2D Icon;//icon to display in inventory
					//public ItemCatagory Cat;
					item.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", item.prefab, typeof(GameObject), false);
					item.equipPrefab = (GameObject)EditorGUILayout.ObjectField("Equip Prefab", item.equipPrefab, typeof(GameObject), false);
					item.strength = EditorGUILayout.FloatField("Strength", item.strength);
					//item.type = (ItemToolType)EditorGUILayout.EnumFlagsField("Type", item.type);
					//item.Cat = (ItemTag)EditorGUILayout.EnumFlagsField("Catigory", item.Cat);
					//EditorGUI.indentLevel += 2;
					EditorGUILayout.Space(10);
					EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
					if (item.tags == null) item.tags = new List<int>();//TODO: is this good
					TagScriptEditor.TagGUI(ref item.tags);
					EditorGUILayout.Space(10);
					//EditorGUI.indentLevel -= 2;
					//public float strength;
					//if (!ItemType.Same(gameControll.itemTypes[i],item))
					//{
					//	changed = true;
					//}

					GameControl.itemTypes[i] = item;
					EditorGUILayout.LabelField("Add or Remove ItemType", EditorStyles.boldLabel);
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("+"))
					{
						GameControl.itemTypes.Insert(i + 1, new ItemType());
					}
					if (GUILayout.Button("-"))
					{
						GameControl.itemTypes.RemoveAt(i);
					}
					GUILayout.EndHorizontal();
					EditorGUI.indentLevel -= 2;
				}
			}

			//if(changed) gameControll.SaveItemTypes();


			if (GUILayout.Button("+ Append ItemType"))
			{
				GameControl.itemTypes.Add(new ItemType());
			}

			if (GUILayout.Button("Save"))
			{
				GameControl.SaveItemTypes();
			}
		}
		EditorGUI.indentLevel -= 2;
	}

	void UpdateShowList()
	{
		while (show.Count < GameControl.itemTypes.Count)
		{
			show.Add(false);
		}
		while (show.Count > GameControl.itemTypes.Count)
		{
			show.RemoveAt(show.Count - 1);
		}
	}
}
#endif