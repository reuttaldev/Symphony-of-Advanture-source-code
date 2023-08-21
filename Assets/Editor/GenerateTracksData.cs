using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
public static class GenerateTracksData 
{
    static string[] mandatoryColumnTitles = { "Track ID", "Track Name", "Artist" };
    static string[] optionalColumnTitles = { "Source", "License" };

    public static void GenerateData( IList<Dictionary<string, string>> table,DataMigrationSettings settings)
    {
        //  print table values 
        // keep a dictionary with all of the track keys we have created a scriptable object for to detect duplicates and possible mistakes 
        //(id,row)
        Dictionary<string, int > allKeys = new Dictionary<string, int>();
        // need to check and notify for duplicate keys or missing value
        int rowNumber = 1;
        foreach (Dictionary<string, string> row in table)
        {
            // run a syntax check
            if (mandatoryColumnTitles.Length + optionalColumnTitles.Length != settings.columnsToRead)
            {
                Debug.LogError("Column count failed");
                Debug.LogError("Saving track data failed.");
                return;
            }
            // mandatory values
            List<string> values = new List<string>();
            foreach (string key in mandatoryColumnTitles)
            {
                if (!row.ContainsKey(key) || string.IsNullOrWhiteSpace(row[key]))
                {
                    Debug.LogError($"Your metadata Google sheet is missing {key} column in row {rowNumber}. Please ensure they are present,spelled out correctly, and filled out for each value");
                    Debug.LogError("Saving track data failed.");
                    return;
                }
                values.Add(row[key]);
            }
            // check if track with this id has already been instantiated
            if (allKeys.ContainsKey(values[0]))
            {
                Debug.LogError($"Your metadata Google sheet contains duplicate track id {values[0]} in rows {rowNumber} and {allKeys[values[0]]}. Please ensure no duplicate values are found and try again");
                Debug.LogError("Saving track data failed.");
                return;
            }
            allKeys[values[0]] = rowNumber + 1;// plus one because we are skipping the first line to have room for headers in the sheet
            TrackData track = ScriptableObject.CreateInstance<TrackData>();
            track.trackID = values[0];
            track.trackName = values[1];
            track.artistName = values[2];


            if (row.ContainsKey(optionalColumnTitles[0]))
            {
                track.source = row[optionalColumnTitles[0]];
            }
            if (row.ContainsKey(optionalColumnTitles[1]))
            {
                track.license = row[optionalColumnTitles[1]];
            }
            string path = settings.GetSOSaveToPath()+ "/"+track.trackID+".asset";
            AssetDatabase.CreateAsset(track, path);
            rowNumber++;  
        }
        Debug.Log("Saved track data successfully.");
    }
}
#endif