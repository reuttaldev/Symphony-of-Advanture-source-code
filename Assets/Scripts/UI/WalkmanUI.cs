using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;
using System;
using Yarn;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
    float hightlightArrowDuration = 0.3f;
    [SerializeField]
    Sprite selectorOn, selectorOff;
    [SerializeField]
    Sprite[] cassetteImages;
    [SerializeField]
    Image cassetteTapeLocation;
    int cassetteImageIndex = 0;
    [SerializeField]
    GameObject cassettePrefab, gridParent;
    List<Button> cassetteButtons = new List<Button>();

    private void OnEnable()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;
        audioManager.OnTrackChanged.AddListener(UpdateDisplay); 
        cassetteImageIndex = 0;
    }
    private void OnDisable()
    {
        audioManager.OnTrackChanged.RemoveListener(UpdateDisplay);
    }


    private void Update()
    {
        if(lastSongAction.action.WasPressedThisFrame())
        {
            LastWasPressed();
        }
        if (nextSongAction.action.WasPressedThisFrame())
        {
            NextWasPressed();
        }
        if (confirmAction.action.WasPressedThisFrame())
        {
            ServiceLocator.Instance.Get<DialogueManager>().PlayerLabeledTrack();
        }
    }
    public void NextWasPressed()
    {
        cassetteImageIndex = (cassetteImageIndex + 1) % (cassetteImages.Length - 1);
        audioManager.PlayLastTrack();
        StartCoroutine(HighlightArrow(rightArrow));

    }
    public void LastWasPressed()
    {
        audioManager.PlayNextTrack();
        StartCoroutine(HighlightArrow(leftArrow));
        if (cassetteImageIndex == 0)
            cassetteImageIndex = cassetteImages.Length - 1;
        else
            cassetteImageIndex--;
    }
    IEnumerator HighlightArrow(Image arrow)
    {
        arrow.sprite = selectorOn;
        yield return new WaitForSeconds(hightlightArrowDuration);
        arrow.sprite = selectorOff;

    }

    public void Open()
    {
        for (int i = 0; i < audioManager.LibrarySize; i++)
        {
            GameObject cassette = Instantiate(cassettePrefab, gridParent.transform);
            CassetteUI cassetteUI = cassette.GetComponent<CassetteUI>();
            cassetteUI.image.sprite = cassetteImages[i];
            cassetteButtons.Add(cassette.GetComponent<Button>());
        }
        EventSystem.current.SetSelectedGameObject(cassetteButtons[0].gameObject);
        audioManager.PlayCurrentTrack();
    }
    // manual = true when we open the walkman through C key, and not through a music dialogue with one of the characters
    public void Close(bool manual = false)
    {
        if (manual)
            audioManager.StopAudio();
        cassetteButtons = new List<Button>();
        foreach (Transform child in gridParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // this method will be called when a new song is being played 
    void UpdateDisplay()
    {
        

    }
}
