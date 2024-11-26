using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Track Data", menuName = "Scriptable Objects/ Track Data")]
[Serializable]
public class TrackData : MyScriptableObject
{
    public AudioClip audioClip;
    public string trackID;
    public string trackName;
    public string artistName;
    [TextAreaAttribute]
    public string license = null;
    public string source = null;
    public int index; // the order in which the input was written in the meta data spreadsheet, i.e. the line they were written at, starting from 0
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

