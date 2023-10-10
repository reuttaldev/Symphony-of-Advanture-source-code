using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

// this script will be added to every game object that allows the start of a dialogue
[RequireComponent(typeof(Collider2D))]

public class DialogueIntractable : MonoBehaviour
{
    [SerializeField]
    private bool interactable = true;
    [SerializeField]
    private bool interactableMoreThanOnce= true; // whether this character should be enabled right now
    [SerializeField]
    string conversationStartNode;

    // then we need a function to tell Yarn Spinner to start from {specifiedNodeName}
    private void StartConversation()
    {
        if (interactable)
            ServiceLocator.Instance.Get<UIManager>().OpenDialogueUI(conversationStartNode);
        else
            Debug.Log("marked as not intractable");
    }

    // make character not able to be clicked on
    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        // then run this character's conversation
        StartConversation();
        if(!interactableMoreThanOnce)
            interactable = false;
    }
}
