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
            audioManager = ServiceLocator.Instance.Get<AudioManager>();

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
            Debug.LogError("track data is null");
            return;
        }
        // do some animation to indicate we are switching songs 
        trackNameTxt.text = trackData.GetTrackName();
        artistNameTxt.text = trackData.GetArtist();
        if(!string.IsNullOrEmpty(trackData.GetLicense()))
            sourceTxt.text = trackData.GetLicense();
        if (!string.IsNullOrEmpty(trackData.GetSource()))
            sourceTxt.text = trackData.GetSource();
    }
}
