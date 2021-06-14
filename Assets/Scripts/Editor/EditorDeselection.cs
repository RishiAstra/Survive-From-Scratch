/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using UnityEngine;
using UnityEditor;
public class EditorDeselection : ScriptableObject
{
    [MenuItem("GameObject/Deselect all %#D")]
    static void DeselectAll()
    {
        Selection.objects = new Object[0];
    }
}