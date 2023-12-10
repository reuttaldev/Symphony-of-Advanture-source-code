using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // thread safety
    private static T instance;
    public static T Instance
    {
        get
        {
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
        if (instance == null)
        {
            instance = gameObject.GetComponent<T>();
            //DontDestroyOnLoad(instance.gameObject);
        }
        else if(instance!=this)
        {
            Debug.Log("Duplicate instances for " + GetType().FullName + ", extra one deleted");
            Destroy(gameObject);
        }
    }
}
