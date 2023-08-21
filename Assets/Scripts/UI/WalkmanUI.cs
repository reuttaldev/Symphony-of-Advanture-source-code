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
        UpdateDisplay(audioManager.GetCurrentTrack());

        nextSongAction.action.performed += context => audioManager.PlayNextTrack();
        lastSongAction.action.performed += context => audioManager.PlayLastTrack();
    }
    private void Start()
    {
       gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        audioManager.OnTrackChanged.RemoveListener(UpdateDisplay);

        nextSongAction.action.performed -= context => audioManager.PlayNextTrack();
        lastSongAction.action.performed -= context => audioManager.PlayLastTrack();
    }

    // this method will be called when a new song is being played 
    void UpdateDisplay(TrackData trackData)
    {
        if (trackData == null)
        {
            Debug.LogError("Walkman UI got empty track data");
            return;
        }
        // do some animation to indicate we are switching songs 
        trackNameTxt.text = trackData.trackName;
        artistNameTxt.text = trackData.artistName;
        if(!string.IsNullOrEmpty(trackData.license))
            sourceTxt.text = trackData.license;
        if (!string.IsNullOrEmpty(trackData.source))
            sourceTxt.text = trackData.source;
    }
}
