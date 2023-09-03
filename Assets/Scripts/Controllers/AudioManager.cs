using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

// this is a singleton because the current available library must be persistent between scenes
public class AudioManager : SimpleSingleton<AudioManager>
{
    // a dictionary of all currently available songs for the player to hear. key is track id and value is it's data container 
    [SerializeField]
    private List<TrackData> library = new List<TrackData>();
    [SerializeField]
    int startingSongIndex;
    // all possible tracks that can be used in the game
    private Dictionary<string,TrackData> allTracks = new Dictionary<string,TrackData>();
    // whenever the track we are currently playing has changed, invoke this events with the following parameters: track name, artist name, source (optional), license (optional)
    public UnityEvent OnTrackChanged;
    // the index in the library list we are currently playing
    private int current;
    private AudioSource audioSource;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
        // load all of the track data scriptable objects from resources folder into all tracks
        var loadedObjects = Resources.LoadAll("Tracks Data", typeof(TrackData)).Cast<TrackData>();
        foreach (TrackData data in loadedObjects)
        {
            allTracks[data.trackID] = data;
            AddTrackToLibrary(data.trackID);
        }
        current = 0;
    }
    private void Start()
    {
        // play the fist song in the library on start game 
        current = startingSongIndex;
        PlayTrack(library[current]);  
    }
    void PlayTrack(TrackData data)
    {
        OnTrackChanged.Invoke();
        audioSource.clip = data.audioClip;
        audioSource.Play();
    }
    void PlayCurrent()
    {
        PlayTrack(library[current]);
        if(!TrackAvilable(current))
        {
            Debug.LogError("Index out of range for audio library");
            return;
        }
    }
    void StopPlayingTrack()
    {
        audioSource.Stop();
    }
    public void PlayNextTrack()
    {
        current = (current+1)%library.Count;
        PlayCurrent();
    }
    public void PlayLastTrack()
    {
        if (current == 0)
            current = library.Count - 1;
        else
            current--;
        PlayCurrent();
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
