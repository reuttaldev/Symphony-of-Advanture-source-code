using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
public class LineViewWithPortraitAndSound : LineView
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    private string currentCharName; // the name of the character we are currently having the dialogue with
    
    [SerializeField]
    [Header("Image")]
    Image portraitImage;

    [Header("Sound")]
    [SerializeField]
    PortraitsData data;
    private AudioSource audioSource;
    private int charCounter,soundCounter;
    [SerializeField]
    private float[] letterToPitch = new float[26];
    [SerializeField]
    float minPitch, maxPitch;   
    [SerializeField]
    TMP_Text lineTMP;

    private Portrait currentPortrait;
    private Emotions emotionToDisplay;

    private void Awake()    
    {
        audioSource = GetComponent<AudioSource>();

    }
    private void Start()
    {
        dialogueRunner.AddCommandHandler<string>("ChangePortrait", ChangePortrait);
    }

    private void ChangePortrait(string emotionString)
    {
        if (!Enum.TryParse<Emotions>(emotionString, true, out emotionToDisplay))
        {
            Debug.LogError("Emotion " + emotionString + " was not found for " + currentCharName);
            emotionToDisplay = Emotions.Neutral;
        }
    }

    public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
    {
        if (dialogueLine.CharacterName != null)
        {
            currentCharName = dialogueLine.CharacterName.ToLower();
            currentPortrait = data.GetPortrait(currentCharName, emotionToDisplay);
            portraitImage.sprite = currentPortrait.sprite;
            charCounter = 0;
            soundCounter = 0;
        }
        base.RunLine(dialogueLine, onDialogueLineFinished); 
    }
    public void OnCharacterTyped()
    {
        int visible = lineTMP.maxVisibleCharacters-1;
        if (visible < 0 || visible >= lineTMP.textInfo.characterCount) 
            return;
        char lastChar = lineTMP.textInfo.characterInfo[visible].character;
        if (!char.IsLetter(lastChar))
            return;
            int cIndex = char.ToLower(lastChar) - 'a';

           charCounter++;
        if (charCounter == currentPortrait.frequency)
        {
            audioSource.Stop();
            try
            {
                audioSource.pitch = letterToPitch[cIndex] * currentPortrait.pitchMultiplier;
            }
            catch
            {
                audioSource.pitch = 1;
            }
            try
            {
                audioSource.PlayOneShot(currentPortrait.dialogueAudioClip[UnityEngine.Random.Range(0, currentPortrait.dialogueAudioClip.Length)]);
            }
            catch (Exception e) { }
                charCounter = 0;
        }
    }
}

