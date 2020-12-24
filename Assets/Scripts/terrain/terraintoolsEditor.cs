#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(terraintools))]
public class terraintoolsEditor : Editor {
	//private terraintools t;
	// Use this for initialization
	void Start () {
		
//		..t = (terraintools)target;
//		if (t == null) {
//			Debug.LogError ("ug");
//		}
	}



	public override void OnInspectorGUI(){
		GUIStyle l = new GUIStyle();
		GUIStyle b = new GUIStyle ();
		b.wordWrap = true;
		l.wordWrap = true;
		EditorGUILayout.LabelField ("");
		EditorGUILayout.LabelField ("Noise controlls",EditorStyles.boldLabel);
		EditorGUILayout.LabelField("xzScale is perlin noise scale,\n yscale scales y. " +
			"high xzScale means more bumps. Hight yscale means mountian-sized. Offsets are for noise. There are 3 noises combined to generate terrain",l);
		DrawDefaultInspector ();

		if(GUILayout.Button("Perlin Noise heightmap")){
			terraintools tt = (terraintools)target;
			tt.noiseHeight ();
		}
		if(GUILayout.Button("Raise/Lower by height varible")){
			terraintools tt = (terraintools)target;
			tt.lower ();
		}
		EditorGUILayout.LabelField ("This is extera space to prevent accidental clicking of undo buttom",l);
		if(GUILayout.Button("undo last edit")){
			terraintools tt = (terraintools)target;
			tt.undo ();
		}
//		if (GUILayout.Button ("noise pait")) {
//			//TODO: make noise brush
//			terraintools tt = (terraintools)target;
//			if (tt.paint) {
//				tt.paint = false;
//			} else {
//				tt.paint = true;
//			}
//		}
	}

	// Update is called once per frame
	void Update () {

	}
}
#endif