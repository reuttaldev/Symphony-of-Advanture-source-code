using System.Collections;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName ="Data Migration Settings", menuName ="Data Migration/Data Migration Settings")]
[SerializeField]
public class DataMigrationSettings : ScriptableObject
{
    [SerializeField]
    SheetsServiceProvider sheetServiceProvider;
    [SerializeField]
    string spreadsheetID;// the ID where both import/export tables are found
    [SerializeField]
    string importSheetID; // the id at the end of the URL of the sheet we need to export from
    string exportSheetID; // the id at the end of the URL of the sheet we need to import to
    "Open User Data Collection Table"
    "Import data from sheets and save" 
    private string saveSOToPath = "Assets/Scriptable Objects/Tracks Data";

    public string GetSOSaveToPath()
    {
        return saveSOToPath;
    }

}
