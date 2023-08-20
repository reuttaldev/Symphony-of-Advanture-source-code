using UnityEngine;
using UnityEditor;


public static class GenerateTracksData 
{
    [SerializeField]
    private static DataMigrationSettings settings;


    [MenuItem("Data/Generate Tracks Data")]
    public static void GenerateData()
    {
        TrackData track = ScriptableObject.CreateInstance<TrackData>();
        AssetDatabase.CreateAsset(track, settings.GetSaveToPath() +track.GetTrackId()+".asset");
    }
}
