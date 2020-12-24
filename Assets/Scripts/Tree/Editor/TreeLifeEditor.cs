#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TreeLife))]
public class TreeLifeEditor : Editor
{

	void Awake()
	{

	}

	// Use this for initialization
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (GUILayout.Button("Repair"))
		{
			((TreeLife)target).RepairTree();
		}
		if (GUILayout.Button("Grow"))
		{
			((TreeLife)target).GrowTree();
		}
	}
}
#endif