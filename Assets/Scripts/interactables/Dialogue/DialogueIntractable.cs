using UnityEngine;
using UnityEngine.Events;

// this script will be added to every game object that allows the start of a dialogue
public class DialogueIntractable : Interactable
{
    [SerializeField]
    DialogueNodeData nodeData;
    DialogueManager dialogueManager;
    GameManager gameManager;
    [SerializeField]
    // where to place the companion in comparison to the player once the dialogue (with someone who is not the companion) has started
    Direction companionPosition = Direction.none, playerDirection = Direction.none;
    private void Start()
    {
        if (nodeData == null)
            Debug.LogError("Forgot to set DialogueStartNodeData to DialogueIntractable " + name);
        if (nodeData != null && string.IsNullOrWhiteSpace(nodeData.nodeTitle))
            Debug.LogError("DialogueStartNodeData with name " + nodeData.name + " has no title set.");
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
        gameManager = ServiceLocator.Instance.Get<GameManager>();
        interactableMoreThanOnce = nodeData.interactableMoreThanOnce;   
    }
    protected override void DisableInteraction()
    {
        if (!nodeData.interactableMoreThanOnce)
        {
            interactable = false;
        }
    }
    protected override void TriggerInteraction()
    {
        // start conversation
        // we need a function to tell Yarn Spinner to start from {specifiedNodeName}
        if (nodeData.associatedMission != null)
        {
            // associated mission was already completed, dialogue is irrelevent now 
            if(nodeData.associatedMission.State == MissionState.CompletedSuccessfully || nodeData.associatedMission.State == MissionState.CompletedUnSuccessfully)
            {
                return;
            }
            dialogueManager.missionToComplete = nodeData.associatedMission;
            nodeData.associatedMission.StartMission();
        }
        if (playerDirection != Direction.none)
            gameManager.FacePosition(playerDirection, transform.position);
        if (companionPosition != Direction.none)
            gameManager.CompanionWalkToPlayer(companionPosition, false);

        dialogueManager.StartDialogue(nodeData.nodeTitle);
    }
    public void ChangeConversationStartNode(string nodeName)
    {
        nodeData.nodeTitle = nodeName;
        // make the trigger interactble again
        interactable = true;
        nodeData.associatedMission = null;
        Debug.Log("Changing dialogue node to be " + nodeName + " on npc " + transform.root.name);
    }
    public void InteractableMoreThanOnce(bool i)
    {
        nodeData.interactableMoreThanOnce = i;
    }
    public void ChangeAssociatedMission(MissionData newAssociatedMission) 
    {
        nodeData.associatedMission = newAssociatedMission;
    }
}
