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
    public AudioClip mp3;
    public string trackID;
    public string trackName;
    public string artistName;
    public string license = null;
    public string source = null;
    private Emotions? userResponse = null;

    public string GetUserResponse()
    {
        return userResponse.ToString();
    }
    public void SetUserResponse(Emotions emotion)
    {
        this.userResponse = emotion;
    }
    private void OnEnable()
    {
        if(string.IsNullOrEmpty(trackID)|| string.IsNullOrEmpty(trackName) || string.IsNullOrEmpty(artistName))
        {
            Debug.LogError("Track Data scriptable object is missing data");
        }
    }
}

