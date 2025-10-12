using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Portraits Data", menuName = "Scriptable Objects/ Portraits Data")]
[Serializable]
public class PortraitsData: ScriptableObject
{
    public Dictionary<string, CharacterPortrait> spritesDictionary = new Dictionary<string, CharacterPortrait>();
    [SerializeField]
    private CharacterPortrait[] portraits;

    public void OnEnable()
    {
        foreach (var data in portraits)
        {
            spritesDictionary[data.charName.ToLower()] = data;
        }
    }
    public Portrait GetPortrait(string charName, Emotions emotion = Emotions.Neutral)
    {
        if (!spritesDictionary.ContainsKey(charName))
        {
            Debug.LogError("No sprite set up for " + charName);
            return null;
        }
        foreach (Portrait portrait in spritesDictionary[charName].portraits)
        {
            if (portrait.emotion == emotion)
                return portrait;
        }
        Debug.Log("Emotion " + emotion + " was not found for " + charName);
        return spritesDictionary[charName].portraits[0];
    }
    
}
[Serializable]
public class CharacterPortrait
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
    public int frequency=1;
    public float pitchMultiplier = 1;
    public AudioClip[] dialogueAudioClip;

}