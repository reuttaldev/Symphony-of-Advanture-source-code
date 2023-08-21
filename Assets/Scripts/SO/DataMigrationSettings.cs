using System.Collections;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName ="Data Migration Settings", menuName ="Data Migration/Data Migration Settings")]
public class DataMigrationSettings : ScriptableObject
{
    [SerializeField]
    SheetsServiceProvider m_SheetsServiceProvider;
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
