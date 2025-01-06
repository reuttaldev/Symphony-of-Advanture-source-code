using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;


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
    [SerializeField]
    private static float frontRotation = -90, backRotation = 170;
    public static float radius = 180, frontRotationSpeed = 4f, backRotationSpeed = 5, radFrontRotation = NormalizeAngle(frontRotation * Mathf.Deg2Rad),radBackRotation = NormalizeAngle(backRotation * Mathf.Deg2Rad); // when x is rotated to -90, the cassetee appears in front of the screen
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
        audioManager.PlayNextTrack();
        cuurentIndexText.text = (cassetteIndex+1).ToString();

    }
    public void LastWasPressed()
    {
        cassetteIndex = (cassetteIndex - 1 + cassettes.Count) % cassettes.Count;
        MoveCassettesInCircle(false);
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
        for (int i = 0; i < cassettes.Count; i++)
        {
            var cas = cassettes[i];
            if (i == cassetteIndex)
                cas.MoveToFrontWithAnim(forward);
            else
                cas.MoveToBackWithAnim(forward);
        }
    }
    public void PlaceCassettes()
    {
        cassettes[0].PlaceAtFrontTarget();
        for (int i = 1; i < cassettes.Count; i++)
        {
            var cas = cassettes[i];
            cas.PlaceAtBackTarget();
        }
    }
    public static float NormalizeAngle(float angle)
    {
        float TwoPi = Mathf.PI * 2;
        while (angle < 0) angle += TwoPi;
        while (angle >= TwoPi) angle -= TwoPi;
        return angle;
    }
    public void Open(bool manual)
    {
        open = true;
        cassetteIndex = 0;
        cuurentIndexText.text = "1";
        outOfTotalText.text = audioManager.LibrarySize.ToString();
        var trackNames = audioManager.GetTracksNames();
        for (int i = 0; i < audioManager.LibrarySize; i++)
        {
            GameObject cassette = Instantiate(cassettePrefab, cassetteParent.transform);
            cassette.name = i.ToString();
            CassetteUI cassetteUI = cassette.GetComponent<CassetteUI>();
            cassetteUI.SetImage(cassetteImages[i % cassetteImages.Length]);
            cassetteUI.SetText(trackNames[i]);
            var cassettUI = cassette.GetComponent<CassetteUI>();
            cassettes.Add(cassettUI);
        }
        PlaceCassettes();
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
            instructionTxt.text = "Choose music to make " + data.charPronouns.ToLower() + " feel ";
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
