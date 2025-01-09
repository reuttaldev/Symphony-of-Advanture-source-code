using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System.Collections;



// this is a singleton because the current available library must be persistent between scenes
public class AudioManager : SimpleSingleton<AudioManager>, IRegistrableService
{
    [SerializeField]
    GameSettings settings;
    [SerializeField]
    AudioSource tracksAudioSource;
    [SerializeField]
    AudioSource ambienceAudioSource;
    [SerializeField]
    AudioClip[] ambienceOptions; 
    [SerializeField]
    public UnityEvent OnTrackChanged;
    private Dictionary<string, TrackData> loadedTracks = new Dictionary<string, TrackData>(); // our currently available library of tracks, the key is the id
    [SerializeField]
    // our current library, what we make available for the player out of the loaded tracks. Contains the keys for the loaded tracks that are stored in the dictionary above 
    private List<string> library = new List<string>();   // I need this list so I can iterate over the library dictionary by order of insertion of the keys
    int index=0;// currently playing index
    public int LibrarySize => library.Count;
    [SerializeField]
    float fadeOutDuration = 2;
    #region ASSET LOADING
    Dictionary<string, AsyncOperationHandle> loadHandles = new Dictionary<string, AsyncOperationHandle>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        tracksAudioSource = GetComponent<AudioSource>();
        if (settings == null)
            Debug.LogError("Audio manager is missing a reference to game settings");
        tracksAudioSource = GetComponent<AudioSource>();
        ServiceLocator.Instance.Register<AudioManager>(this);
        PlayAmbienceMusic();

    }
    void Start()
    {
        LoadTracksFromAddressable(settings.libraryKeys, 0, GameSettings.minTrackLibrarySize);
    }
    TrackDataReference GetAdressableReferenceByID(string trackID)
    {
        int i = settings.trackDataKeys.IndexOf(trackID);
        if (i==-1)
        {
            Debug.LogWarning("Could not find requested track with id "+trackID+", loading default.");
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
    void LoadTrackFromAddressable(TrackDataReference assetReference, bool playOnLoad = false, bool addKeyToList = true)
    {
        // keep a dict with all TrackDataReference that you loaded and check if you have already loaded something before performing this to avoid errors.
        if (loadHandles.ContainsKey(assetReference.ID))
            return;
        AsyncOperationHandle loadHandle = assetReference.LoadAssetAsync<TrackData>();
        loadHandles.Add(assetReference.ID, loadHandle);
        loadHandle.Completed += request =>
        {
            TrackData t = request.Result as TrackData;
            loadedTracks.Add(t.trackID, t);
            //Debug.Log("loaded track with id " + t.trackID);
            if(addKeyToList)
            {
                library.Add(t.trackID);
            }
            // make sure this call is after you added it to the library
            if(playOnLoad)
            {
                SetCurrentTrack(t.trackID);
            }

        };
    }
    void LoadTracksFromAddressable(List<string> libraryKeys,int startDefaultAt, int minSize, bool playOnLoad = false, bool addKeyToList = true)
    {
        bool fromConfigFailed = false;
        // keep references that you might load. if something goes wrong when looking for the assets, the list will be reset to the default
        List<TrackDataReference> references = new List<TrackDataReference>();
        List<string> referencesKeys = new List<string>();
        for (int i = 0; i < libraryKeys.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(libraryKeys[i]))
            {
                fromConfigFailed = true;
                break;
            }
            {
                TrackDataReference toAdd = GetAdressableReferenceByID(libraryKeys[i]);
                if (toAdd == null)
                {
                    Debug.LogError("failed to find id " + libraryKeys[i]);
                    fromConfigFailed = true;
                    break;
                }
                references.Add(toAdd);
                referencesKeys.Add(libraryKeys[i]);
            }
        }
        if(!fromConfigFailed &&references.Count < minSize)
        {
            Debug.Log("Not enough tracks specified in initial library or collectibles. Loading default");
            fromConfigFailed = true;
        }
        if(fromConfigFailed)
        {
            references.Clear();
            // load default
            for (int i = startDefaultAt; i < minSize+ startDefaultAt; i++)
            {
                references.Add(GetAdressableReferenceByIndex(i));
                referencesKeys.Add(libraryKeys[i]);
            }
        }

        for (int i = 0; i < references.Count; i++)
        {
            LoadTrackFromAddressable(references[i], playOnLoad, addKeyToList);
        }
    }

    public void UnloadAdressable(string trackID)
    {
        var loadHandle = loadHandles[trackID];
       //before you release the handle, make sure to remove everything from the list so the reference count goes down 
        if (loadHandle.IsValid())
        {
            Addressables.Release(loadHandle);
        }
    }
    #endregion


    void PlayAmbienceMusic()
    {
        if (ambienceOptions.Length == 0)
        {
            //Debug.LogError("Audio manager says: no ambience options");
            return;
        }
        var clip = ambienceOptions[UnityEngine.Random.Range(0, ambienceOptions.Length)];
        ambienceAudioSource.clip = clip;
        ambienceAudioSource.Play();
    }

    public void PlayClip(AudioClip clip)
    {
        tracksAudioSource.PlayOneShot(clip);
    }
    public void PlayCurrentTrack()
    {
        OnTrackChanged.Invoke();
        string idToPlay = library[index];
        tracksAudioSource.clip = loadedTracks[idToPlay].audioClip;
        tracksAudioSource.Play();
    }
    public void SetCurrentTrack(string id)
    {
        int position = library.IndexOf(id);
        if (index < 0)
        {
            Debug.LogError("Could not set current track to be id: " + id + "since it is not present in the library");
            return; 
        }
        index = position;
        PlayCurrentTrack();
    }
    public void PlayNextTrack()
    {
        index = (index + 1) % (library.Count-1);
        PlayCurrentTrack();
    }
    public void PlayLastTrack()
    {
        if (index == 0)
            index = library.Count - 1;
        else
            index--;
        PlayCurrentTrack();
    }
    public void AddToLibrary(string id) // add track with id to the currently avaialble for playing music library
    {
        Debug.Log("adding to library track with id " + id);
        if (!loadedTracks.ContainsKey(id))
        {
            Debug.LogError("Trying to add track with id "+id+" but it is has no reference loaded");
            return;
        }
        // I want it to be the now first track in the library
        library.Insert(0,id);
    }

    public void RemoveFromLibrary(string id)
    {
        if (!library.Contains(id))
        {

            Debug.LogError("Track is not found in the current available tracks library");
            return;
        }
        loadedTracks.Remove(id);
        library.Remove(id);
        UnloadAdressable(id);
    }
    public TrackData GetCurrentTrack()
    {
        string id = library[index];
        return loadedTracks[id];

    }
    public void SetTrackEmotion(string trackID, Emotions emotion)
    {
        loadedTracks[trackID].SetUserResponse(emotion);
    }
    public void StopAudio()
    {
        StartCoroutine(FadeOut());
    }
    public IEnumerator FadeOut()
    {
        float startVolume = tracksAudioSource.volume;

        while (tracksAudioSource.volume > 0)
        {
            tracksAudioSource.volume -= startVolume * Time.deltaTime /fadeOutDuration;

            yield return null;
        }

        tracksAudioSource.Stop();
        tracksAudioSource.volume = startVolume;
    }
    public string[] GetTracksNames()
    {
        return loadedTracks.Values.Select(track => track.trackName).ToArray();
    }

}


//making an asset "Addressable" allows you to use that asset's unique address to call it from anywhere
 // Addressable assets are not loaded to memeory when the scene is loaded like hard-referenced objects, it will be loaded to memory only when you instantiate the needed asset
 // making it Addressable ensured an asset is loaded and we are investing memory to it only when an if it is used, rather than loading the entire resources folder to the build
 // bundle mode of is set to pack individually so not everything that is in a group (e.g. all audio tracks) must all be loaded at once. I want to choose which ones I will need to use for this build 
 //You only need to make sure that any time you explicitly load an asset, you release it when your application no longer needs that instance.

[Serializable]
public class TrackDataReference : AssetReferenceT<TrackData>
{
    public string ID;
    public TrackDataReference(string guid, string id) : base(guid) { ID = id; }
}