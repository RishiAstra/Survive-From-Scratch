/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using bobStuff;

// IngredientDrawer
[CustomPropertyDrawer(typeof(Item))]
public class ItemEditorUI : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GameControl.CheckItemTypes();
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

		// Calculate rects
		float singleLineHeight = EditorGUIUtility.singleLineHeight;

		var nameRect = new Rect(position.x, position.y, 100, singleLineHeight);
		var idRect = new Rect(position.x + 100, position.y, position.width - 100, singleLineHeight);

        var amountRect = new Rect(position.x, position.y + singleLineHeight, position.width, singleLineHeight);
        var strengthRect = new Rect(position.x, position.y + singleLineHeight * 2, position.width, singleLineHeight);
        var currentStrengthRect = new Rect(position.x, position.y + singleLineHeight * 3, position.width, singleLineHeight);

        //var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        int id = property.FindPropertyRelative("id").intValue;
        string currentName = (id >= 0 && id < GameControl.itemTypes.Count) ? GameControl.itemTypes[id].name : "Error";

        string newName = EditorGUI.TextArea(nameRect, currentName);
        //EditorGUI.
        if(newName != currentName)
		{
            int newId;
			if (GameControl.StringIdMap.TryGetValue(newName, out newId))
			{
                property.FindPropertyRelative("id").intValue = newId;
            }
        }

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(idRect, property.FindPropertyRelative("id"), GUIContent.none);
		EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), new GUIContent("Amount"));
        EditorGUI.PropertyField(strengthRect, property.FindPropertyRelative("strength"), new GUIContent("Strength"));
        EditorGUI.PropertyField(currentStrengthRect, property.FindPropertyRelative("currentStrength"), new GUIContent("Current Strength"));

        //      EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();

    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4;
    }
    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //{

    //    SerializedObject childObj = new UnityEditor.SerializedObject(property.objectReferenceValue);
    //    SerializedProperty ite = childObj.GetIterator();

    //    float totalHeight = EditorGUI.GetPropertyHeight(property, label, true);

    //    ite.Next(true);
    //    totalHeight += EditorGUI.GetPropertyHeight(ite, label, true);

    //    while (ite.NextVisible(true))
    //    {
    //        totalHeight += EditorGUI.GetPropertyHeight(ite, label, true);
    //    }


    //    return totalHeight;
    //}
}
#endif