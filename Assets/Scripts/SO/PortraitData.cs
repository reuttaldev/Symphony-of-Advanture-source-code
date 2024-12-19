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

    private void OnEnable()
    {
        //if (spritesDictionary.Count != portraits.Length) 
        {
            foreach (var data in portraits)
            {
                Debug.Log(data.charName.ToLower());
                spritesDictionary[data.charName.ToLower()] = data;
            }
        }
    }
    public Sprite GetSprite(string charName, Emotions emotion = Emotions.Neutral)
    {
        if (!spritesDictionary.ContainsKey(charName))
        {
            Debug.LogError("No sprite set up for " + charName);
            return null;
        }
        foreach (Portrait portrait in spritesDictionary[charName].portraits)
        {
            if (portrait.emotion == emotion)
                return portrait.sprite;
        }
        Debug.LogError("Emotion " + emotion + " was not found for " + charName);
        return spritesDictionary[charName].portraits[0].sprite;
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

}