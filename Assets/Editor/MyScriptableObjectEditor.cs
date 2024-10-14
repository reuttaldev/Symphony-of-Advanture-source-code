using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MyScriptableObject))]
public class MyScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //serializedObject.Update();

        //DrawDefaultInspector();

        MyScriptableObject myScriptableObject = (MyScriptableObject)target;

        if (GUILayout.Button("Save"))
        {
            myScriptableObject.Save();
        }
    }
}
#endif