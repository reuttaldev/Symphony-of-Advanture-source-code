using UnityEngine;
using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
#endif
// MAKING IT SO THE SCRIPTABLE OBJECT BEHAVES AS IT WOULD IN A BUILD, I.E. RESETS ITESELF WITH EACH GAME START
public class MyScriptableObject : ScriptableObject
{
    public string GlobalID; // for this gameobject, persistent throughout the project 

#if UNITY_EDITOR
    private string defaultStateInJson;
    virtual protected void OnEnable()
    {
        GlobalID = GlobalObjectId.GetGlobalObjectIdSlow(this).ToString();
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    virtual protected void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }
    void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.ExitingPlayMode)
            ResetOnExitPlay();
        if (change == PlayModeStateChange.EnteredPlayMode)
            Save();
    }
    virtual public void Save()
    {
        defaultStateInJson = JsonUtility.ToJson(this);
    }

    virtual protected void ResetOnExitPlay()
    {
        JsonUtility.FromJsonOverwrite(defaultStateInJson, this);
        // Mark the object dirty again to ensure changes are recognized
        EditorUtility.SetDirty(this);
    }
   
#endif
}


