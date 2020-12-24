#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(meterrain))]
//[CanEditMultipleObjects]
public class meterrainEditor : Editor {
	SerializedProperty speed;
	SerializedProperty rad;
	SerializedProperty maxSize;
	meterrain me;
//	SerializedProperty 

	void Awake(){
		me = ((meterrain)target);
	}

	void OnEnable(){
		speed = serializedObject.FindProperty ("speed");
		rad = serializedObject.FindProperty ("rad");
		maxSize = serializedObject.FindProperty ("maxSize");
		me = ((meterrain)target);
	}

	public override void OnInspectorGUI () {
//		EditorGUILayout.PropertyField (speed);
		EditorGUILayout.Slider (speed, 0.01f, 5, "speed");
		EditorGUILayout.Slider (rad, 0.1f, 100f, "radius");
		EditorGUILayout.Slider (maxSize, 1.5f, 5f, "Max quad size");
		if (GUILayout.Button ("Generate new")) {
			((meterrain)target).startMesh();
		}
		serializedObject.ApplyModifiedProperties ();
		DrawDefaultInspector();
	}
	void OnSceneGUI() {
		if(Event.current.type == EventType.MouseDown && Event.current.button == 0){
//			Debug.Log ("grow1");
//			bool[] doneThese = new bool[v.Count];
//			print ("step 1 complete");
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);//Camera.current.ScreenPointToRay (Event.current.mousePosition);
			RaycastHit hit;
//			Debug.Log (" " + ray);
			if(Physics.Raycast(ray, out hit)){
//				Debug.Log ("grow2");
				if (hit.collider == me.mc) {
					me.grow (hit.point);
//					Debug.Log ("grow");
				}
			}

//			updateMesh ();
		}
	}

}
#endif