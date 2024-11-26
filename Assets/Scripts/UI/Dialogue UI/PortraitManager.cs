using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
public class PortraitManager : DialogueViewBase
{
    [SerializeField]
    Image portrait;
    [SerializeField]
    PortraitData[] portraitsData;
    Dictionary<string, PortraitData> spritesDictionary = new Dictionary<string, PortraitData>();   

    private void Awake()
    {
        int i = 0;
        foreach (var data in portraitsData)
        {
            spritesDictionary[data.charName] = data;    
            i++;
        }
    }
    private Sprite GetSprite(string charName, Emotions emotion = Emotions.Neutral)
    {
        if(!spritesDictionary.ContainsKey(charName))
        {
            Debug.LogError("No sprite set up for "+ charName);
            return null;
        }
        foreach(Portrait portrait in spritesDictionary[charName].portraits)
        {
            if (portrait.emotion == emotion) 
                return portrait.sprite;
        }
        Debug.LogError("Emotions " + emotion + " was not found for " + charName);
        return null;
    }
    public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
    {
        var actorName = dialogueLine.CharacterName;
        portrait.sprite = GetSprite(actorName);
        onDialogueLineFinished();
    }
}

[Serializable]
public class PortraitData
{
    public string charName;
    public Portrait[] portraits;
}
// need this class and not just using dict so I can add elements in the editor
[Serializable]
public class Portrait
{
    public Emotions emotion;
    public Sprite sprite;

}