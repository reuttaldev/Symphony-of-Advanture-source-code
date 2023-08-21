using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
public static class GenerateTracksData 
{
    [SerializeField]
    private static DataMigrationSettings settings;


    public static void GenerateData( IList<Dictionary<string, string>> table)
    {
        TrackData track = ScriptableObject.CreateInstance<TrackData>();
        //AssetDatabase.CreateAsset(track, settings.GetSaveToPath() +track.GetTrackId()+".asset");
    }
}
#endif