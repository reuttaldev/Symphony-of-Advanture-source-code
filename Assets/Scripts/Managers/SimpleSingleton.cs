using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool isQuitting = false;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (isQuitting)
                return null;
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                // if couldn't find in scene
                if (instance == null)
                {
                    // create new
                    GameObject go = new GameObject(typeof(T).ToString());
                    instance = go.AddComponent<T>();
                    Debug.Log("Instantiating new singleton of type " + typeof(T).ToString());
                }
            }
            return instance;
        }
    }
    protected virtual void Awake()

    {
        isQuitting = false;
        if (instance == null)
        {
            instance = gameObject.GetComponent<T>();
        }
        else if(instance!=this)
        {
            //Debug.Log("Duplicate instances for " + GetType().FullName + ", extra one deleted");
            Destroy(gameObject);
        }
    }

    // sometimes, on destroy of one class is called before an on disable of another class. 
    // this is done to avoid a new instance will not be created when the application closes, in the case we request an instance in some other class on disable
    void OnApplicationQuit()
    {
        isQuitting = true;  
    }
}
