using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;



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
    AsyncOperationHandle loadHandle;
    void LoadStartingTrack()
    {
        if (!string.IsNullOrWhiteSpace(settings.startingTrack))
        {
            var refToLoad = GetAdressableReferenceByID(settings.startingTrack);
            if(refToLoad != null ) 
            {
                Debug.Log("trying 1");
                LoadAdressable(refToLoad, library, true,true);
                return;
            }
            // if we got here it means refToLoad is null and we should load by default
            Debug.LogError("Could not find requested first track, loading default.");
        }
        LoadAdressable(GetAdressableReferenceByIndex(0), library, true,true);
        Debug.Log("Loaded to library the starting track");
    }
    TrackDataReference GetAdressableReferenceByID(string id)
    {
        int i = libraryKeys.IndexOf(id);
        if (i==-1)
        {
            Debug.LogError("Could not find requested track with id "+id+", loading default.");
            return null;
        }
        return settings.trackDataReferences[i];
    }
    TrackDataReference GetAdressableReferenceByIndex(int index)
    {
        if (index >= settings.trackDataReferences.Count())
        {
            Debug.LogError("Could not find requested track with index " + index + " as there are not enough tracks in the list. Loading default.");
            return null;
        }
        return settings.trackDataReferences[index];
    }
    void LoadAdressable(TrackDataReference dataToLoad, Dictionary<string, TrackData> loadTo, bool playOnLoad = false, bool addKeyToList = true)
    {
        loadHandle = dataToLoad.LoadAssetAsync<TrackData>();
        loadHandle.Completed += track =>
        {
            TrackData t = track.Result as TrackData;
            if (t == null)
                Debug.LogError("Could not load starting track correctly");
            // make it the first track in the list 
            loadTo.Add(t.trackID, t);
            if(addKeyToList)
                libraryKeys.Add(t.trackID);
            Debug.Log(t.trackID);
            if(playOnLoad)
                PlayTrack(t.trackID);
        };
    }
    int LoadAdressable(string [] loadFrom, Dictionary<string, TrackData> loadTo,int startDefaultAt, int minSize, bool playOnLoad = false, bool addKeyToList = true)
    {

        bool fromSettingFailed = false;
        List<TrackDataReference> references = new List<TrackDataReference>();
        for (int i = 0; i < loadFrom.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(loadFrom[i]))
            {
                fromSettingFailed = true;
                break;
            }
            {
                TrackDataReference toAdd = GetAdressableReferenceByID(settings.initTrackLibrary[i]);
                if (toAdd == null)
                {
                    fromSettingFailed = true;
                    break;
                }
                references.Add(toAdd);

            }
        }
        if(!fromSettingFailed &&references.Count < minSize)
        {
            Debug.LogError("Not enough tracks specified in initial library or collectibles.");
            fromSettingFailed = true;
        }
        if(fromSettingFailed)
        {
            references.Clear();
            // load default
            for (int i = startDefaultAt; i < minSize+ startDefaultAt; i++)
            {
                references.Add(GetAdressableReferenceByIndex(i));
            }
        }
        for (int i = 0; i < references.Count; i++)
        {
            LoadAdressable(references[i], loadTo, playOnLoad, addKeyToList);
        }
        return references.Count;
    }

    void LoadLibraryList()
    {
        LoadStartingTrack();
        int loadedToLibrary= LoadAdressable(settings.initTrackLibrary, library, 1, GameSettings.minTrackLibrarySize -1); // -1 bc first track is already loaded
        Debug.Log("Loading into library " + loadedToLibrary + " tracks.");
        int loadedTocollectibles = LoadAdressable(settings.collectibleTracks, collectibles, loadedToLibrary+1, GameSettings.minCollectibleTracks,false,false);
        Debug.Log("Loading into collectibles " + loadedTocollectibles + " tracks.");
    }

    void LoadCollectibleList()
    {
       /*if (gameSettings.collectibleTracks.Length < GameSettings.minCollectibleTracks)
        {
            Debug.LogError("Not enough tracks specified in Track Library. You need a minimum of " + GameSettings.minCollectibleTracks + " tracks.");
            return false;
        }
        foreach (TrackDataReference reference in gameSettings.collectibleTracks)
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
        return true;*/
    }
    public void LoadTracksFromAddressable()
    {
        LoadLibraryList();
        LoadCollectibleList();
    }
    private void OnDestroy()
    {
        Addressables.Release(loadHandle);
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
        if (index == 0)
            index = library.Count - 1;
        index--;
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
    public void AddToLibrary(TrackData data) // add track with id to the currently avaialble for playing music library
    {
        if (TrackInLibrary(data.trackID))
        {
            Debug.LogError("Duplicate track ID found in Initial Track Library. Duplicate ID is " + data.trackID);
        }
        library.Add(data.trackID, data);
        libraryKeys.Add(data.trackID);
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