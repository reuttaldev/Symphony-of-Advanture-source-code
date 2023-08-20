using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour, IRegistrableService
{
    // a dictionary of all currently available songs for the player to hear. key is track id and value is it's data container 
    private List<TrackData> library = new List<TrackData>();
    // all possible tracks that can be used in the game
    private Dictionary<string,TrackData> allTracks = new Dictionary<string,TrackData>();
    // whenever the track we are currently playing has changed, invoke this events with the following parameters: track name, artist name, source (optional), license (optional)
    public UnityEvent<TrackData> OnTrackChanged;
    // the index in the library list we are currently playing
    private int current;
    void Awake()
    {
        ServiceLocator.Instance.Register<AudioManager>(this);
        current = 0;
        // make sure there is at least something in the library
    }
    void PlayTrack(TrackData data)
    {
        OnTrackChanged.Invoke(data);
    }
    public void PlayNextTrack()
    {
        int nextPos = (current+1)%library.Count;
        if (!TrackAvilable(nextPos))
        {
            Debug.LogError("Index out of range for audio library");
            return;
        }
        PlayTrack(library[nextPos]);
    }
    public void PlayLastTrack()
    {
        int lastPos = (current - 1) % library.Count;
        if (!TrackAvilable(lastPos))
        {
            Debug.LogError("Index out of range for audio library");
            return;
        }
        PlayTrack(library[lastPos]);
    }
    public TrackData GetCurrentTrack()
    {
        if(!TrackAvilable(current))
        {
            Debug.LogError("Index out of range for audio library");
            return null;
        }
        return library[current];
    }
    // check if track id is found in the currently available tracks library
    bool TrackAvilable(string trackID)
    {
        if(!TrackExists(trackID)) 
        {
            Debug.LogError("Track " + trackID + " does not exist");
            return false;
        }
        if (library.Contains(allTracks[trackID]))
            return true;
        return false;
    }
    // check if library contains this index
    bool TrackAvilable(int index)
    {
        if (index < library.Count && index >= 0)
            return true;
        return false;
    }


    // check if a track id is found in all possible game tracks
    bool TrackExists(string trackID)
    {
        if (allTracks.ContainsKey(trackID))
            return true;
        return false;
    }

    // when the user has categorized/ labeled a track to a certain emotion, use this method to add this new information to the track's data
    public void SetTrackEmotion(string trackID,Emotions emotion)
    {
        if(!TrackAvilable(trackID))
        {
            Debug.LogError("Track "+trackID+ " is not found in the current available tracks library");
            return;
        }
        allTracks[trackID].SetUserResponse(emotion);
    }
    public void AddTrackToLibrary(string trackID)
    {
        if (TrackAvilable(trackID))
        {
            Debug.LogError("Track is already in library");
            return;
        }
        if(!TrackExists(trackID))
        {
            Debug.LogError("Track does not exist");
            return;
        }
        library.Add(allTracks[trackID]);
    }
    public void RemoveTrackFromLibrary(string trackID)
    {
        if (!TrackAvilable(trackID))
        {
            Debug.LogError("Track is not found in the current available tracks library");
            return;
        }
        library.Remove(allTracks[trackID]);
    }
}
