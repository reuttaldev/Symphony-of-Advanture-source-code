using UnityEngine;
using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
#endif
// MAKING IT SO THE SCRIPTABLE OBJECT BEHAVES AS IT WOULD IN A BUILD, I.E. RESETS ITESELF WITH EACH GAME START
public class MyScriptableObject : ScriptableObject
{
    [HideInInspector]
    public string GlobalID= null; // for this gameobject, persistent throughout the project 

#if UNITY_EDITOR
    // private bc I don't want this to be serialized 
    private string defaultStateInJson;
    [HideInInspector]
    [SerializeField]
    public bool saved = true;
    virtual protected void OnEnable()
    {
        if (GlobalID == null)
            GlobalID = GlobalObjectId.GetGlobalObjectIdSlow(this).ToString();
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    virtual protected void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }
    void OnPlayModeChanged(PlayModeStateChange change)
    {
        // the order here matters, because on disable gets called before on enabled 
        if (change == PlayModeStateChange.EnteredPlayMode)
            Save();
        else if (change == PlayModeStateChange.ExitingPlayMode)
            ResetOnExitPlay();
    }
    virtual public void Save()
    {
        defaultStateInJson = JsonUtility.ToJson(this);
        saved = true;
    }

    virtual public void ResetOnExitPlay()
    {
        JsonUtility.FromJsonOverwrite(defaultStateInJson, this);
        saved = true;
    }

#endif
}


