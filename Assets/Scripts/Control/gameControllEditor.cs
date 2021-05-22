#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using bobStuff;
using System.IO;
using System;

[CustomEditor(typeof(GameControl))]
[System.Serializable]
public class gameControllEditor : Editor
{
	public static bool showTypes = false;
	//public static List<SerializedObject> mp = new List<SerializedObject>();
	//public static List<ModifierGroup> mg = new List<ModifierGroup>();

	public static List<FoldoutStuff> show = new List<FoldoutStuff>();
	GUIStyle tempStyle;
	GameControl t;
	bool showStat;
	private void OnEnable()
	{
		show = new List<FoldoutStuff>();
		//mp = new List<SerializedObject>();
	}

	public override void OnInspectorGUI()
	{
		if(tempStyle == null)
		{
			tempStyle = new GUIStyle(EditorStyles.textArea)
			{
				wordWrap = true,
			};
		}

		t = (GameControl)target;
		GameControl.CheckItemTypes();
		bool refreshItemTypeMap = false;
		DrawDefaultInspector();
		UpdateShowList();		
			
		showTypes = EditorGUILayout.Foldout(showTypes, "Item Types");
		EditorGUI.indentLevel += 2;
		if (showTypes)
		{
			//bool changed = false;
			for(int i = 0; i < GameControl.itemTypes.Count; i++)
			{

				ItemType item = GameControl.itemTypes[i];
				if (string.IsNullOrEmpty(item.name)) item.name = "New ItemType";
				show[i].mainFoldout = EditorGUILayout.Foldout(show[i].mainFoldout, i + " : " + GameControl.itemTypes[i].name);
				if (show[i].mainFoldout)
				{
					EditorGUI.indentLevel += 2;
					item.name = EditorGUILayout.TextField("name", item.name);
					EditorGUILayout.LabelField("description");
					item.description = EditorGUILayout.TextArea(item.description == null? "" : item.description, tempStyle);
					//public Texture2D Icon;//icon to display in inventory
					//public ItemCatagory Cat;
					item.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", item.prefab, typeof(GameObject), false);
					item.equipPrefab = (GameObject)EditorGUILayout.ObjectField("Equip Prefab", item.equipPrefab, typeof(GameObject), false);
					item.strength = EditorGUILayout.FloatField("Strength", item.strength);


					//EditorGUI.indentLevel += 2;

					if (item.mods == null) item.mods = new ModifierGroup();
					if (item.tags == null) item.tags = new List<int>();

					show[i].g =		ShowMods(item.mods.globalArmorModifiers	, show[i].gf,	show[i].g, "Global Armor Modifiers"	);
					show[i].h =		ShowMods(item.mods.hpMods				, show[i].hf,	show[i].h, "HP Modifiers"			);
					show[i].m =		ShowMods(item.mods.mpMods				, show[i].mf,	show[i].m, "MP Modifiers"			);
					show[i].e =		ShowMods(item.mods.engMods				, show[i].ef,	show[i].e, "Energy Modifiers"		);
					show[i].mo =	ShowMods(item.mods.morMods				, show[i].mof,	show[i].mo,"Morale Modifiers"		);
					show[i].a =		ShowMods(item.mods.atkMods				, show[i].af,	show[i].a, "Attack Modifiers"		);



					//EditorGUI.indentLevel -= 2;



					//item.type = (ItemToolType)EditorGUILayout.EnumFlagsField("Type", item.type);
					//item.Cat = (ItemTag)EditorGUILayout.EnumFlagsField("Catigory", item.Cat);
					//EditorGUILayout.PropertyField(mp[i].FindProperty("globalArmorModifiers"));
					//EditorGUILayout.PropertyField(mp[i].FindProperty("hpMods"));
					//EditorGUILayout.PropertyField(mp[i].FindProperty("mpMods"));
					//EditorGUILayout.PropertyField(mp[i].FindProperty("engMods"));
					//EditorGUILayout.PropertyField(mp[i].FindProperty("morMods"));
					//EditorGUILayout.PropertyField(mp[i].FindProperty("atkMods"));
					//mp[i].ApplyModifiedProperties();
					//EditorGUI.indentLevel += 2;
					EditorGUILayout.Space(10);
					showStat = EditorGUILayout.Foldout(showStat, "Consume Restore");
					if (showStat)
					{
						Stat temp = item.consumeRestore;
						temp.hp = EditorGUILayout.FloatField("HP", temp.hp);
						temp.mp = EditorGUILayout.FloatField("MP", temp.mp);
						temp.eng = EditorGUILayout.FloatField("ENG", temp.eng);
						temp.mor = EditorGUILayout.FloatField("MOR", temp.mor);
						temp.atk = EditorGUILayout.FloatField("ATK", temp.atk);
						item.consumeRestore = temp;
					}
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
					if (item.name != GameControl.itemTypes[i].name)
					{
						refreshItemTypeMap = true;
					}
					GameControl.itemTypes[i] = item;
					EditorGUILayout.LabelField("Add or Remove ItemType", EditorStyles.boldLabel);
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("+"))
					{
						GameControl.itemTypes.Insert(i + 1, new ItemType());
						refreshItemTypeMap = true;
					}
					if (GUILayout.Button("-"))
					{
						GameControl.itemTypes.RemoveAt(i);
						refreshItemTypeMap = true;
					}
					GUILayout.EndHorizontal();
					EditorGUI.indentLevel -= 2;
				}
			}

			//if(changed) gameControll.SaveItemTypes();


			if (GUILayout.Button("+ Append ItemType"))
			{
				GameControl.itemTypes.Add(new ItemType());
				refreshItemTypeMap = true;
			}

			if (GUILayout.Button("Save"))
			{
				GameControl.SaveItemTypes();
			}
		}
		EditorGUI.indentLevel -= 2;

		if(refreshItemTypeMap) GameControl.InitializeItemTypeMap();
	}

	private bool ShowMods(List<TypedModifier> m, List<bool> s, bool ss, string title)
	{
		ss = EditorGUILayout.Foldout(ss, title);
		if (ss)
		{
			EditorGUI.indentLevel += 1;
			int l = EditorGUILayout.IntField("Size", m.Count);
			while (l > m.Count)
			{
				m.Add(new TypedModifier());
				s.Add(false);
			}
			while (l < m.Count)
			{
				m.RemoveAt(m.Count - 1);
				s.RemoveAt(s.Count - 1);
			}

			for (int i = 0; i < s.Count; i++)
			{
				s[i] = EditorGUILayout.Foldout(s[i], "Element " + i);
				if (s[i])
				{
					EditorGUI.indentLevel += 1;
					m[i].type = (AttackType)EditorGUILayout.EnumFlagsField("type", m[i].type);
					m[i].m_preadd = EditorGUILayout.FloatField("preadd", m[i].m_preadd);
					m[i].m_premult = EditorGUILayout.FloatField("premult", m[i].m_premult);
					m[i].m_postadd = EditorGUILayout.FloatField("postadd", m[i].m_postadd);
					m[i].m_postmult = EditorGUILayout.FloatField("postmult", m[i].m_postmult);

					EditorGUI.indentLevel -= 1;
				}
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+"))
			{
				m.Add(new TypedModifier());
				s.Add(false);
			}
			if (m.Count > 0 && GUILayout.Button("-"))
			{
				m.RemoveAt(m.Count - 1);
				s.RemoveAt(s.Count - 1);
			}
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel -= 1;
		}
		return ss;
	}

	private bool ShowMods(List<Modifier> m, List<bool> s, bool ss, string title)
	{
		ss = EditorGUILayout.Foldout(ss, title);
		if (ss)
		{
			EditorGUI.indentLevel += 1;
			int l = EditorGUILayout.IntField("Size", m.Count);
			while (l > m.Count)
			{
				m.Add(new TypedModifier());
				s.Add(false);
			}
			while (l < m.Count)
			{
				m.RemoveAt(m.Count - 1);
				s.RemoveAt(s.Count - 1);
			}

			for (int i = 0; i < s.Count; i++)
			{
				s[i] = EditorGUILayout.Foldout(s[i], "Element " + i);
				if (s[i])
				{
					EditorGUI.indentLevel += 1;
					//m[i].type = (AttackType)EditorGUILayout.EnumFlagsField("type", m[i].type);
					m[i].m_preadd = EditorGUILayout.FloatField("preadd", m[i].m_preadd);
					m[i].m_premult = EditorGUILayout.FloatField("premult", m[i].m_premult);
					m[i].m_postadd = EditorGUILayout.FloatField("postadd", m[i].m_postadd);
					m[i].m_postmult = EditorGUILayout.FloatField("postmult", m[i].m_postmult);

					EditorGUI.indentLevel -= 1;
				}
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+"))
			{
				m.Add(new TypedModifier());
				s.Add(false);
			}
			if (m.Count > 0 && GUILayout.Button("-"))
			{
				m.RemoveAt(m.Count - 1);
				s.RemoveAt(s.Count - 1);
			}
			GUILayout.EndHorizontal();
			EditorGUI.indentLevel -= 1;
		}
		return ss;
	}

	void UpdateShowList()
	{
		while (show.Count < GameControl.itemTypes.Count)
		{

			//ItemType t = GameControl.itemTypes[show.Count];
			//if (t.mods == null) t.mods = new ModifierGroup();// ScriptableObject.CreateInstance<ModifierGroup>();
			//GameControl.itemTypes[show.Count] = t;
			////Debug.Log(GameControl.itemTypes[show.Count].mods.ToString());
			////mg.Add(new ModifierGroup());
			//mp.Add((new SerializedObject();// new SerializedObject(mg[show.Count]));// GameControl.itemTypes[show.Count].mods));

			show.Add(new FoldoutStuff(false, GameControl.itemTypes[show.Count].mods));
			
		}
		while (show.Count > GameControl.itemTypes.Count)
		{
			show.RemoveAt(show.Count - 1);
			//mp.RemoveAt(show.Count - 1);
			//mg.RemoveAt(show.Count - 1);
		}
	}


	public class FoldoutStuff
	{
		public bool mainFoldout;
		public List<bool> gf;
		public List<bool> hf;
		public List<bool> mf;
		public List<bool> ef;
		public List<bool> mof;
		public List<bool> af;

		public bool g;
		public bool h;
		public bool m;
		public bool e;
		public bool mo;
		public bool a;


		public FoldoutStuff(bool mainFoldout, ModifierGroup mg)
		{
			if (mg == null) mg = new ModifierGroup();
			this.mainFoldout = mainFoldout;

			if (mg.globalArmorModifiers == null) mg.globalArmorModifiers = new List<TypedModifier>();
			if (mg.hpMods == null) mg.hpMods = new List<Modifier>();
			if (mg.mpMods == null) mg.mpMods = new List<Modifier>();
			if (mg.engMods == null) mg.engMods = new List<Modifier>();
			if (mg.morMods == null) mg.morMods = new List<Modifier>();
			if (mg.atkMods == null) mg.atkMods = new List<TypedModifier>();



			gf = GetBoolList(mg.globalArmorModifiers.Count);
			hf = GetBoolList(mg.hpMods.Count);
			mf = GetBoolList(mg.mpMods.Count);
			ef = GetBoolList(mg.engMods.Count);
			mof = GetBoolList(mg.morMods.Count);
			af = GetBoolList(mg.atkMods.Count);
		}

		private List<bool> GetBoolList(int amount)
		{
			List<bool> temp = new List<bool>();
			for(int i = 0; i < amount; i++)
			{
				temp.Add(false);
			}
			return temp;
		}

	}
}

#endif