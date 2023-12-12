using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Scriptable Objects/Game Settings")]
[Serializable]
public class GameSettings : MyScriptableObject
{
    public const int minTrackLibrarySize = 5;
    public const int minCollectibleTracks =5;
    // I want to display the constant values as a read only property.
    [Header("Audio Settings")]
    [SerializeField]
    [ReadOnly]
    private int minimumLibrarySize = minTrackLibrarySize;
    [SerializeField]
    [ReadOnly]
    private int minimumCollectibleTracks = minCollectibleTracks;
    // TrackDataReference so it is marked as adressable and not loaded into memory until we specifically tell it to.
    // this way, we can keep a list of  the default songs, but they will not be loaded into memory until we confirm the configuration file does not specify other songs.
    // this way we avoid loading unused audio clips into memory.
    public TrackDataReference[] initTrackLibrary = new TrackDataReference[minTrackLibrarySize]; // the tracks here will be loaded onto the walkman at the start of them game 
    public TrackDataReference[] collectibleTracks = new TrackDataReference[minCollectibleTracks]; // list of tracks the player will be able to pick up throughout the game
    public TrackDataReference startingTrack;

    [Header("Player Settings")]
    public string playerName = "Reut";
    public string playerId { get { return GetPlayerID(); } }
    public int gameSessionIndex {get { return GetGameSessionIndex(); } }

    [Header("Build Settings")]
    [HideInInspector]
    public string configurationID = "Default";
    [HideInInspector]
    public bool configFileLoaded = false;
    [SerializeField]
    public string buildID = "1";

    [Header("Asset Settings")]
    public DataMigrationSettings dataMigrationSettings;

    int GetGameSessionIndex()
    {
        return PlayerPrefs.GetInt("GameSessionIndex");
    }
    public void IncreaseGameSessionIndex()
    {
        PlayerPrefs.SetInt("GameSessionIndex", gameSessionIndex+1);
    }
    
    string GetPlayerID()
    {
        return SystemInfo.deviceUniqueIdentifier.ToString();
    }

#if UNITY_EDITOR
    // load data into usedTracks list. Load by order in which the track ids were originally input in the meta data spreadsheet, stop when your reached the minimum required number of tracks.
    // will be done in the editor
    public void SetDefaultTracks()
    {
        try
        {
            // load all TrackData assets. loading all (addressable assets) with certain tag
            AsyncOperationHandle loadHandle;
            loadHandle = Addressables.LoadAssetsAsync<TrackData>("Track Data", null);
            loadHandle.Completed += foundAssets =>
            {
                List<TrackData> allTracks = (List<TrackData>)foundAssets.Result;
                if(allTracks.Count < minCollectibleTracks+minTrackLibrarySize)
                {
                    throw new Exception("Not enough songs were imported. Please ensure to include at least " + minCollectibleTracks + minTrackLibrarySize + "tracks.");
                }
                for (int i = 0; i < minCollectibleTracks + minTrackLibrarySize; i++)
                {
                    // find them by the index, so the order remains as it was in the meta data spreadsheet 
                    TrackData track = allTracks.Find(t => t.index == i);
                    // cast the track data to track data reference, so it will not be loaded automatically and only when we tell it to.
                    if(i<minTrackLibrarySize)
                        initTrackLibrary[i] = GetTrackDataReference(track);
                    else
                        collectibleTracks[i- minTrackLibrarySize] = GetTrackDataReference(track);
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
        catch(Exception e) 
        {
            Debug.LogError("Could not load default tracks to game settings. Error: " + e.Message);
        }
    }
    TrackDataReference GetTrackDataReference(TrackData asset)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        string assetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
        AssetReference reference = settings.CreateAssetReference(assetGUID);
        if (reference == null)
            throw new Exception("reference is null");
        return new TrackDataReference(assetGUID);
    }

#endif
}
