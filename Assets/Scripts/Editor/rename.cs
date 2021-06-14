/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public class NameControll : EditorWindow
{
	string addToEnd = "Hello World";
	bool replace;
	string replaceString;
	bool myBool = true;
//	float myFloat = 1.23f;

	// Add menu named "My Window" to the Window menu
	[MenuItem("Window/Name Controll")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		NameControll window = (NameControll)EditorWindow.GetWindow(typeof(NameControll));
		window.minSize = new Vector2 (300, 300);
		window.Show();
	}
		

	void OnGUI()
	{
		GUILayout.Label("Change end by this", EditorStyles.boldLabel);
		myBool = EditorGUILayout.Toggle ("Change end (instead of add to end)", myBool);
		addToEnd = EditorGUILayout.TextField("Text Field", addToEnd);
		replace = EditorGUILayout.Toggle ("Find and Replace", replace);
		replaceString = EditorGUILayout.TextField ("replace with this", replaceString);
		if (GUILayout.Button ("Change selected")) {
			for (int i = 0; i < Selection.objects.Length; i++) {
				string n = Selection.objects [i].name;
				bool found = false;
				if(replace){
					if (n.Substring (n.Length - replaceString.Length, replaceString.Length).Equals(replaceString)) {
						found = true;
					}
//					for (int j = 0; j < n.Length-replaceString.Length;j++) {
//						if (n.Substring (i, replaceString.Length).Equals(replaceString)) {
//							found = true;
//							break;
//						}
//					}
				}
				if (!replace || found) {
					Undo.RecordObject ((Object)Selection.objects [i], "Rename");
					if (myBool) {
						string s = Selection.objects [i].name;
						s = s.Substring (0, s.Length - addToEnd.Length);
						s += addToEnd;
						Selection.objects [i].name = s;
					} else {
						Selection.objects [i].name += addToEnd;
					}
				}
			}
		}
	}
}
#endif