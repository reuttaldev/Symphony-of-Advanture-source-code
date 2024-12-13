using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
            }
                return instance;
        }
    }
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<T>();
        }
        else if(instance!=this)
        {
            Destroy(gameObject);
        }
    }
}
