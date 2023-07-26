using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private void EndConversation()
    {
        if (isCurrentConversation)
        {
            
            isCurrentConversation = false;
        }
    }

    // make character not able to be clicked on
    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if this character is enabled and no conversation is already running
        if (interactable && !dialogueRunner.IsDialogueRunning)
        {
            // then run this character's conversation
            StartConversation();
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
    
}
