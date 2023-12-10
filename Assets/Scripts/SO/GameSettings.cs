using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Scriptable Objects/Game Settings")]
[Serializable]
public class GameSettings : MyScriptableObject
{
    public const int minNumberOfTracks =10;
    public DataMigrationSettings exportSettings;

    [Header("Audio Settings")]
    [SerializeField]
    [ReadOnly]
    private int minimumNumberOfTracks = minNumberOfTracks; // I want to display the constant value as a read only property.
    public TrackData[] usedTracks = new TrackData[minNumberOfTracks];
    public TrackData startingTrack;

    [Header("Player Settings")]
    public string playerName = "Reut";
    public string playerId { get { return GetPlayerID(); } }
    public int gameSessionIndex {get { return GetGameSessionIndex(); } }

    [Header("Build Settings")]
    [SerializeField]
    public string configurationID = "Default";
    public bool configFileLoaded = false;
    [SerializeField]
    public string buildID = "1";

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
