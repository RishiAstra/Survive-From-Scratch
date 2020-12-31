#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using bobStuff;

[CustomEditor(typeof(gameControll))]
[System.Serializable]
public class gameControllEditor : Editor
{
	public static bool showTypes = false;

	public static List<bool> show = new List<bool>();
	gameControll t;
	public override void OnInspectorGUI()
	{
		t = (gameControll)target;
		DrawDefaultInspector();
		UpdateShowList();		
			
		showTypes = EditorGUILayout.Foldout(showTypes, "Item Types");
		EditorGUI.indentLevel += 2;
		if (showTypes)
		{
			for(int i = 0; i < t.itemTypes.Count; i++)
			{
				show[i] = EditorGUILayout.Foldout(show[i], i + " : " + t.itemTypes[i].name);
				if (show[i])
				{
					EditorGUI.indentLevel += 2;
					ItemType item = t.itemTypes[i];
					item.name = EditorGUILayout.TextField("name", item.name);
					//public Texture2D Icon;//icon to display in inventory
					//public ItemCatagory Cat;
					item.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", item.prefab, typeof(GameObject), false);
					item.equipPrefab = (GameObject)EditorGUILayout.ObjectField("Equip Prefab", item.equipPrefab, typeof(GameObject), false);
					item.strength = EditorGUILayout.FloatField("Strength", item.strength);
					//item.type = (ItemToolType)EditorGUILayout.EnumFlagsField("Type", item.type);
					item.Cat = (ItemTag)EditorGUILayout.EnumFlagsField("Catigory", item.Cat);
					EditorGUI.indentLevel += 2;
					EditorGUI.indentLevel -= 2;
					//public float strength;
					t.itemTypes[i] = item;
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("+"))
					{
						t.itemTypes.Insert(i + 1, new ItemType());
					}
					if (GUILayout.Button("-"))
					{
						t.itemTypes.RemoveAt(i);
					}
					GUILayout.EndHorizontal();
					EditorGUI.indentLevel -= 2;
				}
			}
		}
		EditorGUI.indentLevel -= 2;
	}

	void UpdateShowList()
	{
		while (show.Count < t.itemTypes.Count)
		{
			show.Add(false);
		}
		while (show.Count > t.itemTypes.Count)
		{
			show.RemoveAt(show.Count - 1);
		}
	}
}
#endif