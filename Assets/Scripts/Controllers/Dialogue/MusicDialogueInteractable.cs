using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class MusicDialogueInteractable : MonoBehaviour
{
    [SerializeField]
    bool interactable = true;
    [SerializeField]
    string interactionID;
    [SerializeField]
    Emotions emotionToEnvoke;
    [SerializeField]
    // call this node once the music dialogue finished (player made a choice) if the onCompletionNode is not null
    string onCompletionNode = null;

    public string GetInteractionId()
    {
        return interactionID;
    }
    public Emotions GetEmotion()
    {
        return emotionToEnvoke;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (interactable)
        {
            ServiceLocator.Instance.Get<UIManager>().currentMusicInteraction = this;
        }
    }
}
