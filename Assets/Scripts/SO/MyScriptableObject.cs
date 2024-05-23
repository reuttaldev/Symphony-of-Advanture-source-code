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
    //Allow an editor class method to be initialized when Unity loads without action from the user.
    virtual protected void OnEnable()
    {
        GlobalID = GlobalObjectId.GetGlobalObjectIdSlow(this).ToString();
        //EditorUtility.SetDirty(this); // Mark the object as "dirty" to ensure the change is saved.
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    virtual protected void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }
    void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredPlayMode)
            SaveOnEnterPlay();
        if (change == PlayModeStateChange.ExitingPlayMode)
            ResetOnExitPlay();
    }

    virtual protected void SaveOnEnterPlay()
    {
        defaultStateInJson = JsonUtility.ToJson(this);
    }

    virtual protected void ResetOnExitPlay()
    {
        JsonUtility.FromJsonOverwrite(defaultStateInJson, this);
    }
   
#endif
}


