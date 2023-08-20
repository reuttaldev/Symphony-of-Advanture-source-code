using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data Migration Settings", menuName = "Scriptable Objects/Data Migration Settings")]
[Serializable]
public class DataMigrationSettings : ScriptableObject
{
    [SerializeField]
    private string documentID;
    private string saveToPath = "Assets/Scriptable Objects/Tracks Data";

    public string GetSaveToPath()
    {
        return saveToPath;
    }
    public string GetDocumentId()
    {
        return documentID;
    }
}
