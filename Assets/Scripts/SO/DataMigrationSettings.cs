using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName ="Data Migration Settings", menuName ="Data Migration/Data Migration Settings")]
[Serializable]
public class DataMigrationSettings : ScriptableObject
{
    [SerializeField]
    public SheetsServiceProvider sheetServiceProvider;
    [SerializeField]
    string spreadsheetID;// the ID where both import/export tables are found
    [SerializeField]
    int importSheetID; // the id at the end of the URL of the sheet we need to export from
    [SerializeField]
    int exportSheetID; // the id at the end of the URL of the sheet we need to import to
    private string saveSOToPath = "Assets/Scriptable Objects/Tracks Data";
    /// <summary>
    /// The column mappings. Each <see cref="SheetColumn"/> represents a column in a Google sheet. The column mappings are responsible for converting to and from cell data.
    /// </summary>


    public string GetSOSaveToPath()
    {
        return saveSOToPath;
    }

}
