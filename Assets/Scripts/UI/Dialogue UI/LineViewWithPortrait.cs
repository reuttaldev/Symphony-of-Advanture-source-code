using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
public class LineViewWithPortrait : LineView
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    [SerializeField]
    Image portraitImage;
    [SerializeField]
    PortraitsData data;
    private string currentCharName; // the name of the character we are currently having the dialogue with
    private Emotions emotionToDisplay;
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
        currentCharName = dialogueLine.CharacterName.ToLower();
        portraitImage.sprite = data.GetSprite(currentCharName,emotionToDisplay);
        base.RunLine(dialogueLine, onDialogueLineFinished); 
    }
}

