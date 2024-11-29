using UnityEngine;
using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
#endif
// My scriptable object is used as a data container that I update during runtime e.g. changing the status of a mission data or the next node in dialougedata. If the data is static, i.e. determined completely before the build then I use regular scriptable objest
public class MyScriptableObject : ScriptableObject
{
    [HideInInspector]
    public string GlobalID= null; // for this gameobject, persistent throughout the project 
    // MAKING IT SO THE SCRIPTABLE OBJECT BEHAVES AS IT WOULD IN A BUILD, I.E. RESETS ITESELF WITH EACH GAME START
    public T GetRuntimeInstance<T>() where T : MyScriptableObject
    {
        var manager = ScriptableObjectManager.Instance;
        var storedInstance = manager.Get(GlobalID);
        if (storedInstance != null)
            return (T)storedInstance;
        T instance = Instantiate(this) as T;
        // use ScriptableObjectManager to keep it persistent
        ScriptableObjectManager.Instance.Keep(instance);
        return instance;
    }
#if UNITY_EDITOR
    // private bc I don't want this to be serialized 
    private string defaultStateInJson;
    [HideInInspector]
    [SerializeField]
    public bool saved = true;
    virtual protected void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    virtual protected void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }
    void OnPlayModeChanged(PlayModeStateChange change)
    {
        // the order here matters, because on disable gets called before on enabled 
        //if (change == PlayModeStateChange.EnteredPlayMode)
            //Save();
        //else
        if (change == PlayModeStateChange.ExitingPlayMode)
            ResetOnExitPlay();
    }
    virtual public void Save()
    {
        if (string.IsNullOrEmpty(GlobalID))
            GlobalID = GlobalObjectId.GetGlobalObjectIdSlow(this).ToString();
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


