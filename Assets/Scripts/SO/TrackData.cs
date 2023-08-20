using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Emotions
{
    Happy,
    Sad,
    Calm,
    Energetic,
    Motivational,
    Angry,
    Inspiring,
    Romantic,
    Sexy,
    Mysterious

}

[CreateAssetMenu(fileName = "Track Data", menuName = "Scriptable Objects/ Track Data")]
[Serializable]
public class TrackData : ScriptableObject
{
    [SerializeField]
    private AudioClip mp3;
    [SerializeField]
    private string trackID;
    [SerializeField]
    private string trackName;
    [SerializeField]
    private string artistName;
    [SerializeField]
    private string license = null;
    [SerializeField]
    private string source = null;
    private Emotions? userResponse = null;

    public string GetTrackId()
    {
        return trackID;
    }
    public string GetTrackName()
    {
        return trackName;
    }
    public string GetArtist()
    {
        return artistName;
    }
    public string GetLicense()
    {
        return license;
    }
    public string GetSource()
    {
        return source;
    }
    public string GetUserResponse()
    {
        return userResponse.ToString();
    }
    public void SetUserResponse(Emotions emotion)
    {
        this.userResponse = emotion;
    }
}

