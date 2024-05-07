using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;

public class WalkmanUI : MonoBehaviour
{
    private AudioManager audioManager;
    DialogueManager dialogueManager;
    [SerializeField]
    private InputActionReference nextSongAction, lastSongAction, confirmAction;

    [SerializeField]
    private TMP_Text emotionTxt; // this is the emotion we are asking the player get the character to feel in this specific dialogue;
    [SerializeField]
    Image leftArrow, rightArrow;
    [SerializeField]
    float hightlightArrowDuration = 0.2f;
    [SerializeField]
    Sprite selectorOn, selectorOff;
    [SerializeField]
    Sprite[] cassetteImages;


    private void OnEnable()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;

        audioManager.OnTrackChanged.AddListener(UpdateDisplay);
        UpdateDisplay();
    }
    private void OnDisable()
    {
        audioManager.OnTrackChanged.RemoveListener(UpdateDisplay);
    }
    private void Update()
    {
        if(lastSongAction.action.WasPressedThisFrame())
        {
            audioManager.PlayLastTrack();
            StartCoroutine(HighlightArrow(rightArrow));
        }
        if (nextSongAction.action.WasPressedThisFrame())
        {
            audioManager.PlayNextTrack();
            StartCoroutine(HighlightArrow(leftArrow));
        }
        if (confirmAction.action.WasPressedThisFrame())
        {
            ServiceLocator.Instance.Get<DialogueManager>().PlayerLabeledTrack();
        }
    }
    IEnumerator HighlightArrow(Image arrow)
    {
        yield return new WaitForSeconds(hightlightArrowDuration);
    }
    // this method will be called when a new song is being played 
    void UpdateDisplay()
    {
        TrackData currentTrack = audioManager.GetCurrentTrack();
        // do something to indicate we are switching songs 

    }
}
