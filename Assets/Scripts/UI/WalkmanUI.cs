using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkmanUI : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private TMP_Text trackNameTxt, artistNameTxt, sourceTxt, licenseTxt;
    private AudioManager audioManager;
    [SerializeField]
    private InputActionReference nextSongAction, lastSongAction;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;

        audioManager.OnTrackChanged.AddListener(UpdateDisplay);
        UpdateDisplay();

        nextSongAction.action.performed += context => audioManager.PlayNextTrack();
        lastSongAction.action.performed += context => audioManager.PlayLastTrack();
    }
    private void OnDisable()
    {
        audioManager.OnTrackChanged.RemoveListener(UpdateDisplay);

        nextSongAction.action.performed -= context => audioManager.PlayNextTrack();
        lastSongAction.action.performed -= context => audioManager.PlayLastTrack();
    }

    // this method will be called when a new song is being played 
    void UpdateDisplay()
    {
        TrackData currentTrack = audioManager.GetCurrentTrack();
        // do some animation to indicate we are switching songs 
        trackNameTxt.text = "Track Name: "+currentTrack.trackName;
        artistNameTxt.text = "Artist: "+currentTrack.artistName;
        if (!string.IsNullOrEmpty(currentTrack.source))
            sourceTxt.text = "Source: "+ currentTrack.source;
        else
            sourceTxt.text = "";
        if (!string.IsNullOrEmpty(currentTrack.license))
            licenseTxt.text = currentTrack.license;
        else
            licenseTxt.text = "";

    }
}
