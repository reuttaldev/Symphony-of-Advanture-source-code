using UnityEngine;
using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
#endif
// My scriptable object is used as a data container that I update during runtime e.g. changing the status of a mission data or the next node in dialougedata. If the data is static, i.e. determined completely before the build then I use regular scriptable objest
public class MyScriptableObject : ScriptableObject
{
    public string GlobalID= null; // for this gameobject, persistent throughout the project 
    // MAKING IT SO THE SCRIPTABLE OBJECT BEHAVES AS IT WOULD IN A BUILD, I.E. RESETS ITESELF WITH EACH GAME START
    // calls to this method should be done only from awake, to ensure we have a reference to the clone and not to the original asset
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
    void OnEnable()
    {
        if (string.IsNullOrEmpty(GlobalID))
            GlobalID = GlobalObjectId.GetGlobalObjectIdSlow(this).ToString();
    }
#endif
}


