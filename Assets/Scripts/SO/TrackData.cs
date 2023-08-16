using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Emotions
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

[CreateAssetMenu(fileName = "Track Data", menuName = "Scriptable Objects/ Tracks")]
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
    private string source;
    [SerializeField] 
    private Emotions? userResponse = null;

    public string GetTrackId()
    {
        return trackID;
    }
    public string GetUserResponse()
    {
        return userResponse.ToString();
    }
}

