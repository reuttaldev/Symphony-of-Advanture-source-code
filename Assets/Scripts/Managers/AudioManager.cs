using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;



// this is a singleton because the current available library must be persistent between scenes
public class AudioManager : SimpleSingleton<AudioManager>
{
    GameSettings settings;
    AudioSource audioSource;
    [SerializeField]
    public UnityEvent OnTrackChanged;
    [SerializeField]
    private List<TrackData> library = new List<TrackData>(); // our currently available library of tracks
    [SerializeField]
    private List<TrackData> collectibles = new List<TrackData>(); // our currently available library of tracks
    #region ASSET LOADING
    #region SYNTAX CHECKS
    #endregion
    bool LoadLibraryList()
    {
        if (settings.initTrackLibrary.Length < GameSettings.minTrackLibrarySize)

        {
            Debug.LogError("Not enough tracks specified in Collectible Track. You need a minimum of " + GameSettings.minCollectibleTracks + " tracks.");
            return false;
        }
        foreach (TrackDataReference reference in settings.initTrackLibrary)
        {
            if (reference == null)
            {
                Debug.LogError("Null reference found in Game Settings -> Init Track Library");
                return false;
            }
            AsyncOperationHandle loadHandle = reference.LoadAssetAsync<TrackData>();
            loadHandle.Completed += track =>
            {
                library.Add(track.Result as TrackData);
            };
        }
        return true;
    }
    bool LoadCollectibleList()
    {
        if (settings.collectibleTracks.Length < GameSettings.minCollectibleTracks)
        {
            Debug.LogError("Not enough tracks specified in Track Library. You need a minimum of " + GameSettings.minCollectibleTracks + " tracks.");
            return false;
        }
        // check that there are no duplicates 
        // check that nothing that was selected as a collectible was also selected for the initial track list, otherwise it cannot be collected
        return true;
    }
    public void LoadTracksFromAddressable()
    {
        LoadLibraryList();
        LoadCollectibleList();
    }
    private void OnDestroy()
    {
        foreach (var item in library)
        {

        }
    }
    #endregion

    public void PlayNext()
    {
        //current = (current + 1) % library.Count;
        //PlayCurrent();
    }
    void StopPlaying()
    {
        audioSource.Stop();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
        settings = ServiceLocator.Instance.Get<GameManager>().settings;
        if (settings == null)
            Debug.LogError("Audio manager is missing a reference to game settings");
        LoadTracksFromAddressable();
    }
    private void Start()
    {
    }
    public void PlayTrack(TrackData data)
    {
        OnTrackChanged.Invoke();
        //audioSource.clip = data.audioClip;
        //audioSource.Play();
    }
    public void PlayNextTrack()
    {

    }
    public void PlayLastTrack()
    {

    }
    public void SetTrackEmotion(string trackID, Emotions emotion)
    {

        //allTracks[trackID].SetUserResponse(emotion);
    }
    public TrackData GetCurrentTrack()
    {
        //return library[current];
        return null;
    }
    void RemoveFromLibrary()
    {
        //        reference.ReleaseAsset();

    }
    /*
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
    
    void PlayCurrent()
    {
        PlayTrack(library[current]);
        if(!TrackAvilable(current))
        {
            Debug.LogError("Index out of range for audio library");
            return;
        }
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
    */
}
//making an asset "Addressable" allows you to use that asset's unique address to call it from anywhere
 // Addressable assets are not loaded to memeory when the scene is loaded like hard-referenced objects, it will be loaded to memory only when you instantiate the needed asset
 // making it Addressable ensured an asset is loaded and we are investing memory to it only when an if it is used, rather than loading the entire resources folder to the build
 // bundle mode of is set to pack individually so not everything that is in a group (e.g. all audio tracks) must all be loaded at once. I want to choose which ones I will need to use for this build 
 //You only need to make sure that any time you explicitly load an asset, you release it when your application no longer needs that instance.

[Serializable]
public class AudioClipReference : AssetReferenceT<AudioClip>
{
    public AudioClipReference(string guid) : base(guid) { }
}
[Serializable]
public class TrackDataReference : AssetReferenceT<TrackData>
{
    public TrackDataReference(string guid) : base(guid) { }
}