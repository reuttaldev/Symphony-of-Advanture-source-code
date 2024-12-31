using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]

public class GameConfiguration
{
    public List<string> InitialTrackLibrary;
    public string ExportSheetID;
    public string ConfigurationID;
}
public class ConfigurationFileManager : MonoBehaviour
{
    private GameConfiguration config;
    [SerializeField]
    TMP_Text errorText;
    [SerializeField]
    DataMigrationSettings dataMigrationSettings;
    [SerializeField]
    GameSettings gameSettings;
    [SerializeField]
    UnityEvent onLoadingSuccess;
    private void Start()
    {
        errorText.text = "";
        errorText.gameObject.SetActive(true);
        
    }
    // this will be called during RUNTIME  
    public void LoadConfigurationFile(string text)
    {

        if (string.IsNullOrEmpty(text))
            errorText.text = "Configuration file is empty.";
        try
        {
            config = JsonUtility.FromJson<GameConfiguration>(text);
        }
        catch
        {
            errorText.text = $"Error parsing JSON: missing field in configuration file.";
            return;
        }
        if (!SyntaxChecks())
            return;
        ApplySettings();
        errorText.gameObject.SetActive(false);
        onLoadingSuccess.Invoke();
    }

    bool SyntaxChecks()
    {
        // check all given track IDs are valid, and that we have enough of them in th list 
        if (!CheckSyntaxLibraryList())
            return false;
        if (!ValidateExportSheetID())
            return false;

        if (string.IsNullOrWhiteSpace(config.ConfigurationID))
        {
            errorText.text = $"Error parsing JSON: Please specify an unique configuration ID.";
            return false;
        }
        return true;
    }
    bool ValidateExportSheetID(int exportSheetID)
    {
        SheetsService service = SheetsServiceProvider.ConnectWithServiceAccountKey(dataMigrationSettings);
        return GoogleSheets.ValidateSheetConnection(service, dataMigrationSettings.spreadsheetID, exportSheetID);
    }
    bool CheckSyntaxLibraryList()
    {
        if(config.InitialTrackLibrary.Count < GameSettings.minTrackLibrarySize)
        {
            errorText.text = "Not enough tracks specified in Track Library. You need a minimum of " + GameSettings.minTrackLibrarySize + " tracks.";
            return false;
        }
        return true;
    }
    bool ValidateExportSheetID()
    {
        if (!string.IsNullOrEmpty(config.ExportSheetID) && config.ExportSheetID != "default" && config.ExportSheetID != "Default")
        {
            int id;
            if (!int.TryParse(config.ExportSheetID, out id))
            {
                errorText.text = "Export sheet ID is not a valid integer";
                return false;
            }
            if (!ValidateExportSheetID(id))
            {
                errorText.text = ($"Cannot have a valid connection to sheet with this ID. Are you connected to the Internet? If so, ensure that spreadsheet with ID {dataMigrationSettings.spreadsheetID} contains a table with the provided export ID.");
                return false;
            }
        }
        return true;
    }
    void ApplySettings()
    {
        gameSettings.configFileLoaded = true;
        dataMigrationSettings.exportSheetID = int.Parse(config.ExportSheetID);
        gameSettings.configurationID = config.ConfigurationID;
        if (config.InitialTrackLibrary.Count != 0)
            gameSettings.libraryKeys = config.InitialTrackLibrary;
    }
}
