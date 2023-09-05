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
    SheetsServiceProvider sheetServiceProvider;
    [SerializeField]
    public string spreadsheetID;// the ID where both import/export tables are found
    [SerializeField]
    int importSheetID; // the id at the end of the URL of the sheet we need to export from
    [SerializeField]
    [ReadOnly]
    public int exportSheetID; // the id at the end of the URL of the sheet we need to import to
    [ReadOnly]
    [SerializeField]
    string saveSOToPath = @"Assets\Resources\Tracks Data";
    [ReadOnly]
    [SerializeField]
    string loadAudioPath = @"Assets\Resources\Audio Tracks";
    [ReadOnly]
    [SerializeField]
    public int columnsToRead=5;
    [SerializeField]
    bool sentResultByEmail = false;
    [SerializeField]
    public string researchersEmail;

    public string[] columnTitles = { "CET Time stamp", "Data ID", "Track ID","Annotation","Interaction ID", "User Name", "User ID", "Game Session Index", "Configuration ID", "Time of Day" };

    public string GetSOSaveToPath()
    {
        return saveSOToPath;
    }
    public string GetLoadAudioPath()
    { return loadAudioPath; }

}
