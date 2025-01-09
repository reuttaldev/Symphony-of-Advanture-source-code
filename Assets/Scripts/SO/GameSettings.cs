using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif
[CreateAssetMenu(fileName = "Game Settings", menuName = "Scriptable Objects/Game Settings")]
[Serializable]
public class GameSettings : ScriptableObject
{
    public const int minTrackLibrarySize = 14;
    public const int minCollectibleTracks =0;
    // I want to display the constant values as a read only property.
    [Header("Audio Settings")]
    [SerializeField]
    [ReadOnly]
    private int minimumLibrarySize = minTrackLibrarySize;
    [ReadOnly]
    [SerializeField]
    private int minimumCollectibleTracks = minCollectibleTracks;

    // we can keep a list of  the default songs, but they will not be loaded into memory until we confirm the configuration file does not specify other songs.
    // this way we avoid loading unused audio clips into memory.
    public List<string> libraryKeys; // the tracks here will be loaded onto the walkman at the start of them game 
    public string[] collectibleTracks = new string[minCollectibleTracks]; // list of tracks the player will be able to pick up throughout the game

    [Header("Player Settings")]
    public string playerName { get { return playerNameData.PlayerName; } }
    public string deviceID { get { return GetPlayerID(); } }
    public int gameSessionIndex {get { return GetGameSessionIndex(); } }

    [Header("Build Settings")]
    [HideInInspector]
    public string configurationID = "Default";
    //[HideInInspector]
    public bool configFileLoaded = false;
    [SerializeField]
    public string buildID = "1";

    [Header("Asset Settings")]
    //[HideInInspector]
    // keep an adressable reference to all possible tracks so we can just grab the needed ones at loading time
    public List<TrackDataReference> trackDataReferences;
    // so that I can also know the order of insertion of the references
    //[HideInInspector]
    public List<string> trackDataKeys;
    [SerializeField]
    private PlayerNameData playerNameData;
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
}
