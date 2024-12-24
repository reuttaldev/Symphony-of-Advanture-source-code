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
    public string spreadsheetID;// the ID where both import/export tables are found
    [SerializeField]
    int importSheetID; // the id at the end of the URL of the sheet we need to export from
    [ReadOnly]
    [SerializeField]
    public int exportSheetID; // the id at the end of the URL of the sheet we need to import to
    [SerializeField]
    public string exportSheetName = "Collected Data";
    [SerializeField]
    [ReadOnly]
    string saveSOToPath = @"Assets\Scriptable Objects\Tracks Data";
    [SerializeField]
    [ReadOnly]
    string loadAudioPath = @"Assets\Audio\Tracks";
    [SerializeField]
    public int columnsToRead=2;
    [SerializeField]
    public bool sentResultByEmail = false;
    [SerializeField]
    public string researchersEmail;
    public NewSheetProperties newSheetProperties = new NewSheetProperties();
    public ResearcherData researcherData;
    public ClientData clientData;
    // these values are here so we can know when to display the helper boxes in the editor
    [SerializeField]
    private string tempExportName;
    [SerializeField]
    private string errorMessage;
    public string GetTracksDataLocation()
    {
        return saveSOToPath;
    }
    public string GetTracksLocation()
    { return loadAudioPath; }
}
