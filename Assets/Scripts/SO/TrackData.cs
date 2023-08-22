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
    public AudioClip audioClip;
    public string trackID;
    public string trackName;
    public string artistName;
    [TextAreaAttribute]
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
}

