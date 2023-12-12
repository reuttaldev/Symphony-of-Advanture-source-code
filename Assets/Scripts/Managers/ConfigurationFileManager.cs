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
        if (!ValidateStartingTrack())
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
        SheetsService service = SheetsServiceProvider.ConnectWithServiceAccountKey(gameSettings.dataMigrationSettings);
        return GoogleSheets.ValidateSheetConnection(service, gameSettings.dataMigrationSettings.spreadsheetID, exportSheetID);
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
    bool ValidateStartingTrack()
    {
        if (!string.IsNullOrEmpty(config.StartingTrack) && config.StartingTrack != "default" && config.StartingTrack != "Default")
        {
            // check starting track is valid
            //the stating track must be an element from the give list of track IDs to use
            if (config.InitialTrackLibrary == null|| config.InitialTrackLibrary.Count ==0 || !config.InitialTrackLibrary.Contains(config.StartingTrack)) 
            {
                errorText.text = "Specified starting track must be included in the initial track library list.";
                return false;
            }
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
                errorText.text = ($"Cannot have a valid connection to sheet with this ID. Are you connected to the Internet? If so, ensure that spreadsheet with ID {gameSettings.dataMigrationSettings.spreadsheetID} contains a table with the provided export ID.");
                return false;
            }
        }
        return true;
    }
    void ApplySettings()
    {
        gameSettings.configFileLoaded = true;
        gameSettings.dataMigrationSettings.exportSheetID = int.Parse(config.ExportSheetID);
        gameSettings.configurationID = config.ConfigurationID;
        //if (config.TracksIDs.Count != 0)
          //  gameSettings.usedTracks = tracksToLoad.ToArray();
        //if (startingTrackData != null)
          //  gameSettings.startingTrack = startingTrackData;

    }
}
