using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // thread safety
    private static readonly object padlock = new object();
    private static T instance;
    public static T Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    
                    instance = FindObjectOfType<T>(); // if couldnt find in scene
                    if (instance == null)
                    {
                        // create new
                        GameObject go = new GameObject(typeof(T).ToString());
                        instance = go.AddComponent<T>();
                        Debug.LogError("Instantiating new singleton of type " + typeof(T).ToString());
                        Instantiate(go);
                    }
                }
                return instance;
            }
        }
    }
    protected virtual void Awake()

    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<T>();
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
            throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
        }
    }
}
