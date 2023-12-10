using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Scriptable Objects/Game Settings")]
[Serializable]
public class GameSettings : MyScriptableObject
{
    public DataMigrationSettings exportSettings;
    public List<TrackData> usedTracks;
    public TrackData startingTrack;
    public string playerName = "Reut";
    public string playerId { get { return GetPlayerID(); } }
    public int gameSessionIndex {get { return GetGameSessionIndex(); } set { SetGameSessionIndex(value); } }
    public bool configFileLoaded = false;
    public string configurationID = "Default";


    //[Header("Audio Selection")]


    int GetGameSessionIndex()
    {
        return PlayerPrefs.GetInt("GameSessionIndex");
    }
    void SetGameSessionIndex(int i)
    {
        PlayerPrefs.SetInt("GameSessionIndex", i);
    }
    
    string GetPlayerID()
    {
        return SystemInfo.deviceUniqueIdentifier.ToString();
    }

}
