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
    int index = 0;// currently playing index
    public int LibrarySize => library.Count;
    [SerializeField]
    float fadeOutDuration = 2;
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
        index = (index + 1) % (library.Count - 1);
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
            Debug.LogError("Trying to add track with id " + id + " but it is has no reference loaded");
            return;
        }
        // I want it to be the now first track in the library
        library.Insert(0, id);
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
            tracksAudioSource.volume -= startVolume * Time.deltaTime / fadeOutDuration;

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