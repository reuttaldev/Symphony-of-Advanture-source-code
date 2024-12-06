using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class WalkmanUI : MonoBehaviour
{
    private AudioManager audioManager;
    DialogueManager dialogueManager;
    [SerializeField]
    private InputActionReference nextSongAction, lastSongAction, confirmAction;
    [SerializeField]
    private GameObject prompt;
    [SerializeField]
    private TMP_Text emotionTxt, instructionTxt, cuurentIndexText, outOfTotalText;
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
    public static float radius = 100, frontRotationSpeed = 4f, backRotationSpeed = 6f, frontRotation = -90,backRotation =130 ; // when x is rotated to -90, the cassetee appears in front of the screen
    [SerializeField]
    private RectTransform rectTransform;
    public static Vector3 centerPoint;


    private void OnEnable()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;
        audioManager.OnTrackChanged.AddListener(UpdateDisplay);
    }
    private void OnDisable()
    {
        audioManager.OnTrackChanged.RemoveListener(UpdateDisplay);
    }
    private void Awake()
    {
        // world position of the rectTransform
        centerPoint = rectTransform.TransformPoint(rectTransform.anchoredPosition);
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
        cassetteIndex = (cassetteIndex + 1) % cassettes.Count;
        MoveCassettesInCircle(true);
        Debug.Log(cassetteIndex);
        audioManager.PlayNextTrack();
        cuurentIndexText.text = (cassetteIndex+1).ToString();

    }
    public void LastWasPressed()
    {
        cassetteIndex = (cassetteIndex - 1 + cassettes.Count) % cassettes.Count;
        MoveCassettesInCircle(false);
        Debug.Log(cassetteIndex);
        audioManager.PlayLastTrack();
        cuurentIndexText.text = (cassetteIndex+1).ToString();
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
        for (int i = 0; i < cassettes.Count; i++)
        {
            var cas = cassettes[i];
            if (i == cassetteIndex)
                cas.MoveToFront(forward);
            else
                cas.MoveToBack(forward);
        }
    }
    public void Open(bool manual)
    {
        open = true;
        cassetteIndex = 0;
        cuurentIndexText.text = "1";
        outOfTotalText.text = audioManager.LibrarySize.ToString();
        for (int i = 0; i < audioManager.LibrarySize; i++)
        {
            GameObject cassette = Instantiate(cassettePrefab, cassetteParent.transform);
            cassette.name = i.ToString();
            CassetteUI cassetteUI = cassette.GetComponent<CassetteUI>();
            cassetteUI.image.sprite = cassetteImages[i % cassetteImages.Length];
            var cassettUI = cassette.GetComponent<CassetteUI>();
            cassettes.Add(cassettUI);
        }
        MoveCassettesInCircle(true);
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
        else
        {
            prompt.SetActive(false);
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
