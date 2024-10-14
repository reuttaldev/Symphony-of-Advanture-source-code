using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MyScriptableObject), true)] // "true" makes it apply to derived classes
public class MyScriptableObjectEditor : Editor
{/*
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MyScriptableObject myScriptableObject = (MyScriptableObject)target;

        // check if the object has unsaved changes (i.e., is dirty)
        bool isDirty = EditorUtility.IsDirty(myScriptableObject);
        GUI.color = isDirty ? Color.red : Color.green;
        if (GUILayout.Button("Save"))
        {
            myScriptableObject.Save();
        }
        if (GUILayout.Button("Reload"))
        {
            myScriptableObject.ResetOnExitPlay();
        }
    }*/
}
#endif