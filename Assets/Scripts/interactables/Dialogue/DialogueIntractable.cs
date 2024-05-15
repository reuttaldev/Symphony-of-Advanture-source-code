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
    [SerializeField]
    // where to place the companion in comparison to the player once the dialogue (with someone who is not the companion) has started
    Direction companionPosition = Direction.none;
    private void Start()
    {
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
    }
    protected override void TriggerInteraction()
    {
        // start conversation
        // we need a function to tell Yarn Spinner to start from {specifiedNodeName}
        if(associatedMission != null)
        {
            // associated mission was already completed, dialogue is irrelevent now 
            if(associatedMission.State == MissionState.CompletedSuccessfully || associatedMission.State == MissionState.CompletedUnSuccessfully)
            {
                return;
            }
            dialogueManager.missionToComplete = associatedMission;
        }
        dialogueManager.StartDialogue(conversationStartNode, companionPosition);
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
