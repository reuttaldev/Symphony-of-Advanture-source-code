using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceKeeper : MonoBehaviour
{
    public List<MyScriptableObject> scripts = new List<MyScriptableObject>();
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
