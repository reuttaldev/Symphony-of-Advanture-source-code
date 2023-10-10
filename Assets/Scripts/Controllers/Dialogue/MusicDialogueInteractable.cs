using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class MusicDialogueInteractable : MonoBehaviour
{
    [SerializeField]
    bool interactable = true;
    [SerializeField]
    [ReadOnly]
    string interactionID;
    [SerializeField]
    Emotions emotionToEnvoke;
    [SerializeField]
    // call this node once the music dialogue finished (player made a choice) if the onCompletionNode is not null
    string onCompletionNode = null;

    private void Start()
    {
        GetInteractionId(); // so the id is visible in the inspector at start
    }
    public string GetInteractionId()
    {
        interactionID = GlobalObjectId.GetGlobalObjectIdSlow(this).ToString();
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
