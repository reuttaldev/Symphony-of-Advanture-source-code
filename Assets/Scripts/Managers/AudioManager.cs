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
    AudioSource ambienceAudioSource;
    [SerializeField]
    AudioClip[] ambienceOptions;
    [SerializeField]
    public UnityEvent OnTrackChanged;
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

}