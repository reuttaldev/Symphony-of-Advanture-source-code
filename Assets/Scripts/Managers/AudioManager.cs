using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using System.Linq;
using System;

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


    //making an asset "Addressable" allows you to use that asset's unique address to call it from anywhere
    // Addressable assets are not loaded to memeory when the scene is loaded like hard-referenced objects, it will be loaded to memory only when you instantiate the needed asset
    // making it Addressable ensured an asset is loaded and we are investing memory to it only when an if it is used, rather than loading the entire resources folder to the build
    // bundle mode of is set to pack individually so not everything that is in a group (e.g. all audio tracks) must all be loaded at once. I want to choose which ones I will need to use for this build 
    [SerializeField]
    AssetReference reference;
    //You only need to make sure that any time you explicitly load an asset, you release it when your application no longer needs that instance.

    [Serializable]
    public class AudioReferenceAudioClip: AssetReferenceT<AudioClip>
    {
        public AudioReferenceAudioClip(string guid) : base(guid) { }
    }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        current = 0;
    }
    private void Start()
    {
        // play the fist song in the library on start game 
        current = startingSongIndex;
        PlayTrack(library[current]);  
    }
    #region TRACK MEMORY MANAGMENT 
    // this will load the track data scriptable object from a location that is accessible in build thorugh adressables
    // so we can choose what to load for different builds
    private void LoadTracks()
    {
    //Use the LoadAssetsAsync method to load more than one Addressable asset in a single operation. spesify a list of keys
        //Addressables.LoadAssetAsync();
        var loadedObjects = Resources.LoadAll("Tracks Data", typeof(TrackData)).Cast<TrackData>();
        foreach (TrackData data in loadedObjects)
        {
            allTracks[data.trackID] = data;
            AddTrackToLibrary(data.trackID);
        }
    }
    public void AddTrackToLibrary(string trackID)
    {
        if (TrackAvilable(trackID))
        {
            Debug.LogError("Track is already in library");
            return;
        }
        if (!TrackExists(trackID))
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
    #endregion
    void PlayTrack(TrackData data)
    {
        OnTrackChanged.Invoke();
        //audioSource.clip = data.audioClip;
        //audioSource.Play();
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
        //audioSource.Stop();
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
}
