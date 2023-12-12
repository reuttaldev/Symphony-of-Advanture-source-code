#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor.AddressableAssets;
using System.Text.RegularExpressions;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using static PlasticPipe.Server.MonitorStats;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GenerateTracksData 
{
    static string[] mandatoryColumnTitles = { "Track ID", "Track Name", "Artist" };
    static string[] optionalColumnTitles = { "Source", "License" };
    static string audioClipTag = "Track Audio Clip";
    public static void GenerateData( IList<Dictionary<string, string>> table,DataMigrationSettings settings)
    {
        //syntax check
        if(!CheckRequiredColumns(settings.columnsToRead))
            return;
        CreateFolder(settings.GetTracksDataLocation());
        // keep a dictionary with all of the track keys we have created a scriptable object for to detect duplicates and possible mistakes 
        //(id,row)
        Dictionary<string, int > allKeys = new Dictionary<string, int>();
        // need to check and notify for duplicate keys or missing value
        int rowNumber = 2;
        int trackIndex = 0;
        foreach (Dictionary<string, string> row in table)
        {
            List<string> rowValues = CheckMandatoryValues( row, rowNumber);
            if (rowValues == null)
                return;
            string trackID = rowValues[0];
            if (CheckForDuplicateIDs(allKeys, trackID,rowNumber))
                return;
            TrackData track = CreateTrackData(rowValues, trackIndex);
            AudioClip clip = GetAudioClipByID( trackID, settings.GetTracksLocation());
            if(clip == null) 
                return;
            track.audioClip = clip;
            string saveToPath = Path.Combine(settings.GetTracksDataLocation(), track.trackID+ ".asset");
            AssetDatabase.CreateAsset(track, saveToPath);
            rowNumber++;
            trackIndex++;
        }
        Debug.Log("Saved track data successfully.");
    }
    #region SYNTAX CHECKS
    static bool CheckRequiredColumns(int columnsToRead)
    {

        // check if the columns we want to load are all in the sheet
        if (mandatoryColumnTitles.Length + optionalColumnTitles.Length != columnsToRead)
        {
            Debug.LogError("Column count failed");
            Debug.LogError("Saving track data failed.");
            return false;
        }
        return true;
    }
    static List<string> CheckMandatoryValues( Dictionary<string, string> row, int rowNumber)
    {
        // check all mandatory columns are filled out with values for each line
        List<string> rowValues = new List<string>();
        foreach (string key in mandatoryColumnTitles)
        {
            if (!row.ContainsKey(key) || string.IsNullOrWhiteSpace(row[key]))
            {
                Debug.LogError($"Your metadata Google sheet is missing {key} column in row {rowNumber}. Please ensure they are present,spelled out correctly, and filled out for each value");
                Debug.LogError("Saving track data failed.");
                return null;
            }
            rowValues.Add(row[key]);
        }
        // add optional values, if found
        if (row.ContainsKey(optionalColumnTitles[0]))
        {
            rowValues.Add(row[optionalColumnTitles[0]]);
        }
        if (row.ContainsKey(optionalColumnTitles[1]))
        {
            rowValues.Add(row[optionalColumnTitles[1]]);
        }
        return rowValues;
    }
    static bool CheckForDuplicateIDs(Dictionary<string, int> allKeys,string trackID, int rowNumber)
    {
        // check if track with this id has already been instantiated
        if (allKeys.ContainsKey(trackID))
        {
            Debug.LogError($"Your metadata Google sheet contains duplicate track id {trackID} in rows {rowNumber} and {allKeys[trackID]}. Please ensure no duplicate values are found and try again");
            Debug.LogError("Saving track data failed.");
            return true;
        }
        allKeys[trackID] = rowNumber;
        return false;
    }
    #endregion
    #region ASSET MANAGMENT
    static TrackData CreateTrackData(List<string> values, int index)
    {
        TrackData track = ScriptableObject.CreateInstance<TrackData>();
        track.trackID = values[0];
        track.trackName = values[1];
        track.artistName = values[2];
        if(values.Count > 3)
            track.source = values[3];
        if(values.Count > 4)
        track.license = values[4];
        track.index = index;
        return track;
    }
    static AudioClip GetAudioClipByID(string id, string lookInFolder)
    {
        var guids = AssetDatabase.FindAssets(id,new[] { lookInFolder });
        if(guids.Length ==0)
        {
            Debug.LogError("Saving track data failed.");
            return null;
        }
        try
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
        }
        catch
        {
            Debug.LogError($"There is no audio file that matches track id {id} in {lookInFolder}. Please make sure there is an audio file for every entry in the metadata spreadsheed. The name of the audio file should be the track id.");
            return null;
        }
    }
    static void CreateFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Console.WriteLine("Directory created: " + folderPath);
        }
        else // I am deleting everything where our asset will be placed to ensure no old  data remains 
        {
    
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            // Delete all files in the folder
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
            // make changes appear in the editor
            AssetDatabase.Refresh();
        }
    }

    static string RemoveForbiddenPathCharacters(string inputPath)
    {
        // Define a regular expression pattern to match forbidden characters
        string pattern = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";

        // Use Regex.Replace to remove forbidden characters
        return Regex.Replace(inputPath, pattern, " ");
    }
    static AssetReference AddAssetToAddressables(UnityEngine.Object asset)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        string assetPath = AssetDatabase.GetAssetPath(asset);
        string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
        return settings.CreateAssetReference(assetGUID);
    }
    #endregion
}
#endif