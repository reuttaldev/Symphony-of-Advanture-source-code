using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public static class GenerateTracksData 
{
    [SerializeField]
    private static DataMigrationSettings settings;


    public static void GenerateData()
    {
        TrackData track = ScriptableObject.CreateInstance<TrackData>();
        //AssetDatabase.CreateAsset(track, settings.GetSaveToPath() +track.GetTrackId()+".asset");
    }
}
#endif