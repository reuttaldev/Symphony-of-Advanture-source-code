using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EXPResponse", menuName = "Scriptable Objects")]
public class EXPResponse : ScriptableObject
{
    [SerializeField]
    string trackIndex;
    [SerializeField]
    string trackName;
    [SerializeField]
    private string response = "";

    public string Response
    {
        get { return response; }
        set { this.response = value; }
    }
    public string TrackIndex { get; }
    public string TrackName { get; }
}
