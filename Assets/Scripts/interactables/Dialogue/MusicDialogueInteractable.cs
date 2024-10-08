using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn;

// music dialogue intractable will hold a reference to music dialogue data to let the dialogue manager know this is the data that needs to be opend once the command OMD has been used
// alternatively, pass the name of the data you want to open as an argument to the OMD command, the dialogue manager will have a reference too all of these scriptable objects so everything can be handles from whithin the yarn script
public class MusicDialogueInteractable : Interactable
{

    [SerializeField]
    MusicDialogueData data;
    DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
        PerformDataSyntaxCheck();
    }
    void PerformDataSyntaxCheck()
    {
        if(data == null)
        {
            Debug.LogError(gameObject.name + "'s music dialogue data is not set! ");
            return;
        }
        if (string.IsNullOrEmpty(data.GlobalID))
        {
            Debug.LogError("No interaction id");
        }
        if (string.IsNullOrEmpty(data.interactionName))
        {
            Debug.LogError("No interaction name");
        }
    }
    protected override void TriggerInteraction()
    {
        dialogueManager.SetMusicInteraction(data);
        if (data.missionToComplete != null)
            dialogueManager.SetMissionToComplete(data.missionToComplete);
    }
}
