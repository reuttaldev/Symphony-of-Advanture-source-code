using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

// this script will be added to every game object that allows the start of a dialogue
public class DialogueIntractable : Interactable
{
    [SerializeField]
    string conversationStartNode;
    DialogueManager dialogueManager;
    [SerializeField]
    UnityEvent onCompletion;
    private void Start()
    {
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
    }
    protected override void TriggerInteraction()
    {
        // start conversation
        // we need a function to tell Yarn Spinner to start from {specifiedNodeName}
        dialogueManager.onDialogueComplete = onCompletion;
        dialogueManager.StartDialogue(conversationStartNode);
    }
}
