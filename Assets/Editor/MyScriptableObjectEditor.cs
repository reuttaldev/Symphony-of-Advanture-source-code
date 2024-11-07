using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MyScriptableObject), true)] // "true" makes it apply to derived classes
public class MyScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        MyScriptableObject myScriptableObject = (MyScriptableObject)target;

        bool isSaved = true;
        // Check if any changes were made
        if (EditorGUI.EndChangeCheck())
        {
            myScriptableObject.saved = false;   
        }

        bool playing = Application.isPlaying;
        if(playing)
            GUI.color = Color.red;
        else
            GUI.color = myScriptableObject.saved ? Color.green : Color.red;
        if (GUILayout.Button("Save"))
        {
            myScriptableObject.Save();
        }
        if (GUILayout.Button("Reload"))
        {
            myScriptableObject.ResetOnExitPlay();
        }
        serializedObject.ApplyModifiedProperties();
        Repaint();

    }
}
#endif