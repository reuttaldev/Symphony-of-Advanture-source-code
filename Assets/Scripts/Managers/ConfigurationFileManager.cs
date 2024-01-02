using Google.Apis.Sheets.v4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]

public class GameConfiguration
{
    public List<string> InitialTrackLibrary;
    public List<string> CollectibleTracks;
    public string StartingTrack;
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
        if (!CheckSyntaxCollectibleList())
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
    bool CheckSyntaxCollectibleList()
    {
        if(config.InitialTrackLibrary.Count < GameSettings.minTrackLibrarySize)
        {
            errorText.text = "Not enough tracks specified in Track Library. You need a minimum of " + GameSettings.minCollectibleTracks + " tracks.";
            return false;
        }
        return true;
    }
    bool CheckSyntaxLibraryList()
    {
        if (config.CollectibleTracks.Count < GameSettings.minCollectibleTracks)
        {
            errorText.text = "Not enough tracks specified in Collectible Track. You need a minimum of " + GameSettings.minCollectibleTracks + " tracks.";
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
            gameSettings.initTrackLibrary = config.InitialTrackLibrary.ToArray();
        if (config.CollectibleTracks.Count != 0)
            gameSettings.collectibleTracks = config.CollectibleTracks.ToArray();
        if (!string.IsNullOrEmpty(config.StartingTrack) || config.StartingTrack !="" || config.StartingTrack != "default"|| config.StartingTrack != "Default")
            gameSettings.startingTrack = config.StartingTrack;

    }
}
