using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this manager solves two problems I have when storing scene-presistent data: 
// 1. It simulates the way data will behave on scriptable object in the build on the editor, by leaving the asset itself unchanged. I create an insrance of that asset during runtime, and change only that. The instance will die when we exit play mode and the original asset containing e.g. state of my missions at the beggining of the game will remain unchanged
// 2. Unlike the Unity editor, in Unity builds scriptable objects (assets and instances) will be garbage collected when switching scenes, unless every single scene holds a reference to it. This is troublesome, since data such as the dialogged node each character is currently at is crucial information for maintaining gameflow, regardless of scene transitions.
// 
public class MyScriptableObjectManager : SimpleSingleton<MyScriptableObjectManager>
{
    public Dictionary<string, MyScriptableObject> scripts = new Dictionary<string, MyScriptableObject>();
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void Keep(MyScriptableObject obj)
    {
        if (scripts.ContainsKey(obj.GlobalID))
        {
            Debug.LogError("Object with name " + obj.name + " already has an instance floating");
        }
        scripts.Add(obj.GlobalID, obj); 
    }

    public MyScriptableObject Get(string globalId)
    {
        if (!scripts.ContainsKey(globalId))
        {
            //Debug.LogError("My scriptable object instance does not exist");
            return null;
        }
        return scripts[globalId];
    }
}
