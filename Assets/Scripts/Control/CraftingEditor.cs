#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using bobStuff;

[CustomEditor(typeof(Crafting))]
[System.Serializable]
public class CraftingEditor : Editor
{
	public static bool showRecipies = false;

	public static List<bool> show = new List<bool>();
	Crafting t;
	void Start()
	{
		((Crafting)target).Start();
	}
	public override void OnInspectorGUI()
	{
		//TODO: do this in better way
		if (gameControll.main == null) gameControll.main = GameObject.FindObjectOfType<gameControll>();
		t = (Crafting)target;
		DrawDefaultInspector();
		UpdateShowList();

		showRecipies = EditorGUILayout.Foldout(showRecipies, "Recipies");
		EditorGUI.indentLevel += 2;
		if (showRecipies)
		{
			for (int i = 0; i < t.recipies.Count; i++)
			{
				show[i] = EditorGUILayout.Foldout(show[i], i + " : " + gameControll.itemTypes[t.recipies[i].result.id].name);
				if (show[i])
				{
					EditorGUI.indentLevel += 2;
					Recipie recipie = t.recipies[i];
					if (recipie.ingredients == null) recipie.ingredients = new List<Item>();
					EditorGUILayout.LabelField("Ingredients");
					for (int j = 0; j < recipie.ingredients.Count; j++)
					{						
						EditorGUI.indentLevel += 2;
						recipie.ingredients[j] = ItemEditorUI(recipie.ingredients[j]);
							
						EditorGUILayout.LabelField("Add or Remove Ingredient", EditorStyles.boldLabel);
						GUILayout.BeginHorizontal();
						if (GUILayout.Button("+"))
						{
							recipie.ingredients.Insert(j + 1, new Item());
						}
						if (GUILayout.Button("-"))
						{
						recipie.ingredients.RemoveAt(j);
						}
						GUILayout.EndHorizontal();
						EditorGUI.indentLevel -= 2;
					}
					if (GUILayout.Button("+"))
					{
						recipie.ingredients.Add(new Item());
					}

					EditorGUILayout.LabelField("Result");
					recipie.result = ItemEditorUI(recipie.result);
					//item.name = EditorGUILayout.TextField("name", item.name);
					//item.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", item.prefab, typeof(GameObject), false);
					//item.equipPrefab = (GameObject)EditorGUILayout.ObjectField("Equip Prefab", item.equipPrefab, typeof(GameObject), false);
					//item.strength = EditorGUILayout.FloatField("Strength", item.strength);

					t.recipies[i] = recipie;
					EditorGUILayout.LabelField("Add or Remove Recipie", EditorStyles.boldLabel);
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("+"))
					{
						t.recipies.Insert(i + 1, new Recipie());
					}
					if (GUILayout.Button("-"))
					{
						t.recipies.RemoveAt(i);
					}
					GUILayout.EndHorizontal();
					EditorGUI.indentLevel -= 2;
				}
			}

			EditorGUILayout.Space(20);
			if (GUILayout.Button("+"))
			{
				t.recipies.Add(new Recipie());
			}
		}
		EditorGUI.indentLevel -= 2;
	}

	public static Item ItemEditorUI(Item item)
	{
		//TODO: do this in a better way
		if (gameControll.main == null) gameControll.main = GameObject.FindObjectOfType<gameControll>();
		//deal with id
		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		GUILayout.BeginHorizontal();
		item.id = EditorGUILayout.IntField(item.id, GUILayout.MaxWidth(50.0f));

		bool suc = false;
		string temp = EditorGUILayout.TextArea(gameControll.itemTypes[item.id].name);

		if(temp != gameControll.itemTypes[item.id].name)
		{
			for (int i = 0; i < gameControll.itemTypes.Count; i++)
			{
				if (gameControll.itemTypes[i].name == temp)
				{
					item.id = i;
					suc = true;
					Debug.Log("Changed item id to " + i);
				}
			}
			if (!suc)
			{
				EditorGUILayout.LabelField(TagScriptEditor.errorContent, GUILayout.MaxWidth(50.0f));
			}
		}		
		GUILayout.EndHorizontal();

		//deal with amount
		item.amount = EditorGUILayout.IntField("Amount", item.amount);
		//TODO: show strength etc as well

		EditorGUI.indentLevel = indentLevel;

		return item;
	}

	void UpdateShowList()
	{
		while (show.Count < t.recipies.Count)
		{
			show.Add(false);
		}
		while (show.Count > t.recipies.Count)
		{
			show.RemoveAt(show.Count - 1);
		}
	}
}
#endif