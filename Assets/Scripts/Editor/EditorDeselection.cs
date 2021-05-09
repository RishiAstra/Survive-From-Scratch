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