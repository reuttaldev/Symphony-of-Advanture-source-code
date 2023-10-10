using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(GetGlobalObjectId))]
public class MusicDialogueInteractable : MonoBehaviour
{
    [SerializeField]
    bool interactable = true;
    [SerializeField]
    Emotions emotionToEnvoke;
    string callingNode = null; // the name of the yarn node (dialogue name) that called to start this music interaction
    [SerializeField]
    // call this node once the music dialogue finished (player made a choice) if the onCompletionNode is not null
    string onCompletionNode = null;

    private void Start()
    {
        GetInteractionId(); // so the id is visible in the inspector at start
    }
    public string GetInteractionId()
    {
        return GetComponent<GetGlobalObjectId>().UniqueId;
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
