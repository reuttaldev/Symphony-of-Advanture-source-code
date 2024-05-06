using UnityEngine;
using UnityEngine.Events;

// this script will be added to every game object that allows the start of a dialogue
public class DialogueIntractable : Interactable
{
    [SerializeField]
    string conversationStartNode;
    DialogueManager dialogueManager;
    [SerializeField]
    MissionData associatedMission;
    private void Start()
    {
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
    }
    protected override void TriggerInteraction()
    {
        // start conversation
        // we need a function to tell Yarn Spinner to start from {specifiedNodeName}
        dialogueManager.StartDialogue(conversationStartNode);
        dialogueManager.missionToComplete = associatedMission;
    }
    public void ChangeConversationStartNode(string nodeName)
    {
        conversationStartNode = nodeName;
        // make the trigger interactble again
        interactable = true;
        Debug.Log("Changing dialogue node to be " + nodeName + " on npc " + transform.root.name);
    }
    public void InteractableMoreThanOnce(bool a)
    {
        interactableMoreThanOnce = a;
    }
    public void ChangeAssociatedMission(MissionData a) 
    {
        associatedMission = a;
    }
}
