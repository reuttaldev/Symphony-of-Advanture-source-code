#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Drawing.Printing;

public static class GenerateTracksData 
{
    static string[] mandatoryColumnTitles = { "Track ID", "Track Name", "Artist" };
    static string[] optionalColumnTitles = { "Source", "License" };
    public static bool GenerateData( IList<Dictionary<string, string>> table,DataMigrationSettings dataMigrationSetting,GameSettings gameSettings)
    {
        //syntax check
        if(!CheckRequiredColumns(dataMigrationSetting.columnsToRead))
            return false;
        CreateFolder(dataMigrationSetting.GetTracksDataLocation());
        gameSettings.trackDataReferences = new List<TrackDataReference>();
        gameSettings.trackDataKeys = new List<string>();
        int rowNumber = 2;
        int trackIndex = 0;
        foreach (Dictionary<string, string> row in table)
        {
            List<string> rowValues = CheckMandatoryValues( row, rowNumber);
            if (rowValues == null)
                return false;
            string trackID = rowValues[0];
            if (CheckForDuplicateIDs(gameSettings.trackDataKeys, trackID,rowNumber))
                return false ;
            TrackData track = CreateTrackData(rowValues, trackIndex);
            AudioClip clip = GetAudioClipByID( trackID, dataMigrationSetting.GetTracksLocation());
            if(clip == null) 
                return false;
            track.audioClip = clip;
            string saveToPath = Path.Combine(dataMigrationSetting.GetTracksDataLocation(), RemoveForbiddenPathCharacters(track.trackID)+ ".asset");
            AssetDatabase.CreateAsset(track, saveToPath);
            gameSettings.trackDataReferences.Add(GetTrackDataReference(saveToPath));
            gameSettings.trackDataKeys.Add(trackID);
            // make sure changes are saved
            EditorUtility.SetDirty(gameSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            rowNumber++;
            trackIndex++;
        }
        Debug.Log("Saved track data successfully.");
        return true;
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
    static bool CheckForDuplicateIDs(List<string> allKeys,string trackID, int rowNumber)
    {
        // check if track with this id has already been instantiated
        if (allKeys.Contains(trackID))
        {
            Debug.LogError($"Your metadata Google sheet contains duplicate track id {trackID} in row {rowNumber}, i.e. the same id already appears somewhere else. Please ensure no duplicate values are found and try again");
            Debug.LogError("Saving track data failed.");
            return true;
        }
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
            Debug.LogError("Saving track data failed. Bad GUID");
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
    static void SetTracksReference()
    {
        /*try
        {
            // load all TrackData assets. loading all (addressable assets) with certain tag
            AsyncOperationHandle loadHandle;
            loadHandle = Addressables.LoadAssetsAsync<TrackData>("Track Data", null);
            loadHandle.Completed += foundAssets =>
            {
                List<TrackData> allTracks = (List<TrackData>)foundAssets.Result;
                if (allTracks.Count < minCollectibleTracks + minTrackLibrarySize)
                {
                    throw new Exception("Not enough songs were imported. Please ensure to include at least " + minCollectibleTracks + minTrackLibrarySize + "tracks.");
                }
                for (int i = 0; i < minCollectibleTracks + minTrackLibrarySize; i++)
                {
                    // find them by the index, so the order remains as it was in the meta data spreadsheet 
                    TrackData track = allTracks.Find(t => t.index == i);
                    // cast the track data to track data reference, so it will not be loaded automatically and only when we tell it to.
                    if (i < minTrackLibrarySize)
                        initTrackLibrary[i] = GetTrackDataReference(track);
                    else
                        collectibleTracks[i - minTrackLibrarySize] = GetTrackDataReference(track);
                }
                Debug.Log("Loaded default tracks to game settings successfully.");
            };
            Addressables.Release(loadHandle);
            startingTrack = initTrackLibrary[0];
            // make sure changes are saved
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError("Could not load default tracks to game settings. Error: " + e.Message);
        }*/
    }
    static TrackDataReference GetTrackDataReference(string assetPath)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
        // adding this asset to adressables
        AssetReference reference = settings.CreateAssetReference(assetGUID);
        if (reference == null)
            throw new Exception("reference is null");
        return new TrackDataReference(assetGUID);
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
   
    #endregion
}
#endif