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
using static UnityEditor.Progress;

public class WalkmanUI : MonoBehaviour
{
    private AudioManager audioManager;
    DialogueManager dialogueManager;
    [SerializeField]
    private InputActionReference nextSongAction, lastSongAction, confirmAction;
    [SerializeField]
    private GameObject prompt;
    [SerializeField]
    private TMP_Text emotionTxt, instructionTxt; // this is the emotion we are asking the player get the character to feel in this specific dialogue;
    [SerializeField]
    Image leftArrow, rightArrow;
    [SerializeField]
    float hightlightArrowDuration = 0.3f;
    [SerializeField]
    Sprite selectorOn, selectorOff;
    [SerializeField]
    Sprite[] cassetteImages;
    int cassetteIndex = 0;
    [SerializeField]
    GameObject cassettePrefab, cassetteParent;
    List<CassetteUI> cassettes = new List<CassetteUI>();
    public bool open = false;
    [SerializeField]
    public static float radius=2, rotationSpeed=2;
    private void OnEnable()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;
        audioManager.OnTrackChanged.AddListener(UpdateDisplay);
        cassetteIndex = 0;
    }
    private void OnDisable()
    {
        audioManager.OnTrackChanged.RemoveListener(UpdateDisplay);
    }


    private void Update()
    {
        if (lastSongAction.action.WasPressedThisFrame())
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
        MoveCassettesInCircle(true);
        cassetteIndex = (cassetteIndex + 1) % cassettes.Count;
        audioManager.PlayNextTrack();
    }
    public void LastWasPressed()
    {
        MoveCassettesInCircle(false);
        cassetteIndex = (cassetteIndex - 1) % cassettes.Count;
        audioManager.PlayLastTrack();
    }
    IEnumerator HighlightArrow(Image arrow)
    {
        arrow.sprite = selectorOn;
        yield return new WaitForSeconds(hightlightArrowDuration);
        arrow.sprite = selectorOff;

    }
    public void MoveCassettesInCircle(bool forward)
    {
        float degreesToMove = 360 / cassettes.Count;
        foreach (var cassette in cassettes)
        {
            cassette.MoveInCircle(forward, degreesToMove);
        }
    }
    public void Open(bool manual)
    {
        open = true;
        float angle = 360 / (audioManager.LibrarySize -1);
        for (int i = 1; i < audioManager.LibrarySize; i++)
        {
            GameObject cassette = Instantiate(cassettePrefab, cassetteParent.transform);
            CassetteUI cassetteUI = cassette.GetComponent<CassetteUI>();
            cassetteUI.image.sprite = cassetteImages[i % cassetteImages.Length];
            var cassettUI = cassette.GetComponent<CassetteUI>();
            cassettes.Add(cassettUI);
            cassetteUI.PlaceAroundCircle(angle *i);
        }
        EventSystem.current.SetSelectedGameObject(cassettes[0].gameObject);
        audioManager.PlayCurrentTrack();
        // update targeted emotion txt
        if (!manual)
        {
            var data = ServiceLocator.Instance.Get<DialogueManager>().currentMusicInteraction;
            if (data == null)
            {
                Debug.LogError("Could not write prompt bc current music interaction in dialogue manager is null");
                return;
            }
            emotionTxt.text = data.targetedEmotionLabel;
            instructionTxt.text = "Choose a snippet that will make " + data.charPronouns.ToLower() + " feel ";
            prompt.SetActive(true);
        }
    }


    // manual = true when we open the walkman through C key, and not through a music dialogue with one of the characters
    public void Close()
    {
        if (!open)
            return;
        Debug.Log("closing walkman uI");
        prompt.SetActive(false);
        open = false;   
        audioManager.StopAudio();
        cassettes.Clear();
        foreach (Transform child in cassetteParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // this method will be called when a new song is being played 
    void UpdateDisplay()
    {
        

    }
}
