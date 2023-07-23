using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnInteractable : MonoBehaviour
{
    private DialogueRunner dialogueRunner;
    [SerializeField]
    private bool interactable = true; // whether this character should be enabled right now
    private bool isCurrentConversation;
    [SerializeField]
    string conversationStartNode;

    public void Start()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
    }

    // then we need a function to tell Yarn Spinner to start from {specifiedNodeName}

    private void StartConversation()
    {
        dialogueRunner.StartDialogue(conversationStartNode);
        isCurrentConversation = true;
    }
    public void OnMouseDown()
    {
        // if this character is enabled and no conversation is already running
        if (interactable && !dialogueRunner.IsDialogueRunning)
        {
            // then run this character's conversation
            StartConversation();
        }

    }
    // re-enable scene interaction, deactivate indicator, etc.
    private void EndConversation()
    {
        if (isCurrentConversation)
        {
            // TODO *stop animation or turn off indicator or whatever* HERE
            isCurrentConversation = false;
        }
    }

    // make character not able to be clicked on
    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }  
}
