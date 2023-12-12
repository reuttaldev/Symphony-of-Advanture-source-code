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
    GameSettings settings;
    AudioSource audioSource;
    [SerializeField]
    public UnityEvent OnTrackChanged;

    private AsyncOperationHandle loadHandle; // need to unload the assets when done


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
    }
    private void Start()
    {
        settings = ServiceLocator.Instance.Get<GameManager>().gameSettings;
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
    #region TRACK MEMORY MANAGMENT 
    public void LoadTrackData(List<string> keys) 
    {

    }

    // Operation handle used to load and release assets

    // Load Addressables by Label
    public IEnumerator Start(List<string> keys)
    {
        float x = 0, z = 0;
        loadHandle = Addressables.LoadAssetsAsync<GameObject>(
            keys,
            addressable =>
            {
                //Gets called for every loaded asset
                Instantiate<GameObject>(addressable,
                    new Vector3(x++ * 2.0f, 0, z * 2.0f),
                    Quaternion.identity,
                    transform);

                if (x > 9)
                {
                    x = 0;
                    z++;
                }
            }, Addressables.MergeMode.Union, // How to combine multiple labels 
            true); // Whether to fail and release if any asset fails to load

        yield return loadHandle;
    }

    private void OnDestroy()
    {
        Addressables.Release(loadHandle);
        // Release all the loaded assets associated with loadHandle
        // Note that if you do not make loaded addressables a child of this object,
        // then you will need to devise another way of releasing the handle when
        // all the individual addressables are destroyed.
    }
    #endregion

}//making an asset "Addressable" allows you to use that asset's unique address to call it from anywhere
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