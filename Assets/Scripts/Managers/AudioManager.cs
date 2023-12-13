using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;



// this is a singleton because the current available library must be persistent between scenes
public class AudioManager : SimpleSingleton<AudioManager>
{
    [SerializeField]
    GameSettings settings;
    AudioSource audioSource;
    [SerializeField]
    public UnityEvent OnTrackChanged;
    private Dictionary<string, TrackData> library = new Dictionary<string, TrackData>(); // our currently available library of tracks, the key is the id
    [SerializeField]
    private List<string> libraryKeys = new List<string>();   // I need this list so I can iterate over the library dictionary by order of insertion of the keys
    private Dictionary<string,TrackData> collectibles = new Dictionary<string, TrackData>(); // the key is the track ID. These are all the tracks that we can collect during the game
    int index;// currently playing index
    #region ASSET LOADING
    #region SYNTAX CHECKS
    #endregion

    void LoadStartingTrack()
    {
        AsyncOperationHandle loadHandle = settings.startingTrack.LoadAssetAsync<TrackData>();
        loadHandle.Completed += track =>
        {
            TrackData t = track.Result as TrackData;
            if(t == null)
                Debug.LogError("Could not load starting track correctly");
            // make it the first track in the list 
            library.Add(t.trackID, t);
            libraryKeys.Add(t.trackID);
            // play it once loaded
            PlayTrack();
            Debug.Log(t.trackID);
        };
    }
    bool LoadLibraryList()
    {
        if (settings.initTrackLibrary.Length < GameSettings.minTrackLibrarySize)

        {
            Debug.LogError("Not enough tracks specified in Collectible Track. You need a minimum of " + GameSettings.minCollectibleTracks + " tracks.");
            return false;
        }
        LoadStartingTrack();
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
                TrackData t = track.Result as TrackData;
                if (t == null)
                    Debug.LogError("Problem retrieving track");
                if (TrackInLibrary(t.trackID))
                {
                    Debug.LogError("Duplicate track ID found in Initial Track Library. Duplicate ID is " + t.trackID);
                }
                else
                {
                    library.Add(t.trackID, t);
                    libraryKeys.Add(t.trackID);
                }
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
        foreach (TrackDataReference reference in settings.collectibleTracks)
        {
            if (reference == null)
            {
                Debug.LogError("Null reference found in Game Settings -> Collectible Tracks");
                return false;
            }
            AsyncOperationHandle loadHandle = reference.LoadAssetAsync<TrackData>();
            loadHandle.Completed += track =>
            {
                TrackData t = track.Result as TrackData;    
                if (t == null)
                    Debug.LogError("Problem retrieving track");
                if(HasCollectible(t.trackID))
                {
                    Debug.LogError("Duplicate track ID found in Collectible Tracks. Duplicate ID is "+t.trackID);
                }
                else if(TrackInLibrary(t.trackID))
                {
                    Debug.LogError("A track that was selected in collectible is already in the initial library lust. This is not allowed. Duplicate ID is "+t.trackID);
                }
                else
                    collectibles.Add(t.trackID, t);
            };
        }
        // check that there are no duplicates 
        // check that nothing that was selected as a collectible was also selected for the initial track list, otherwise it cannot be collected
        return true;
    }
    public void LoadTracksFromAddressable()
    {
        Debug.Log("started loading tracks");
        LoadLibraryList();
        LoadCollectibleList();
    }
    private void OnDestroy()
    {
        /*foreach (var item in settings.initTrackLibrary)
        {
            item.ReleaseAsset();
        }
        foreach (var item in settings.collectibleTracks)
        {
            item.ReleaseAsset();
        }*/
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
        if (settings == null)
            Debug.LogError("Audio manager is missing a reference to game settings");
        LoadTracksFromAddressable();
        index = 0;
    }
    public void PlayTrack()
    {
        PlayTrack(libraryKeys[index]);
    }
    public void PlayNextTrack()
    {
        index = (index+1)%libraryKeys.Count;
        PlayTrack();
    }
    public void PlayLastTrack()
    {
        index = (index - 1) % (libraryKeys.Count-1);
        PlayTrack();
    }
    public void StopAudio()
    { 
        audioSource.Stop();
    } 
    public void PlayTrack(string id)
    {
        OnTrackChanged.Invoke();
        audioSource.clip = library[id].audioClip;
        audioSource.Play();
    }
    public void AddToLibrary(string id) // add track with id to the currently avaialble for playing music library
    {
        if(!HasCollectible(id))
        {
            Debug.LogError("Trying to add track with id "+id+" but it is not included as a collectible");
            return;
        }
        TrackData trackToAdd = collectibles[id];
        library.Add(id, trackToAdd);
        // I want it to be the now first track in the library
        libraryKeys.Insert(0,id);
    }
    public void RemoveFromLibrary(string id)
    {
        if (!TrackInLibrary(id))
        {

            Debug.LogError("Track is not found in the current available tracks library");
            return;
        }
        library.Remove(id);
        libraryKeys.Remove(id);
        // addressable reference will be released whwn this manager OnDestroy() is called
    }
    public TrackData GetCurrentTrack()
    {
        return library[libraryKeys[index]];
    }
    public void SetTrackEmotion(string trackID, Emotions emotion)
    {
        library[trackID].SetUserResponse(emotion);
    }
    bool TrackInLibrary(string id)
    {
        return collectibles.ContainsKey(id);
    }
    bool HasCollectible(string id)
    {
        return library.ContainsKey(id);
    }

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