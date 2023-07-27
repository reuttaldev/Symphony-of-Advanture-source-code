// Ignore Spelling: Dialogue
using Yarn.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    void Awake()
    {
        dialogueRunner = gameObject.GetComponent<DialogueRunner>();
        dialogueRunner.AddCommandHandler("ExitGame", GameManager.Instance.ExitGame);
    }
    private void Start()
    {
        dialogueRunner.onDialogueStart.AddListener(()=>InputManager.Instance.ActivateDialogueMap());
        dialogueRunner.onDialogueComplete.AddListener(()=>InputManager.Instance.ActivatePlayerMap());
    }
    void MyAction()
    {

    }

    // this function will set and store the answer of the player to a "how did this track make you feel"
    // The player has answered in dialog. this function will be called through a yarn script
    [YarnCommand("ExpResponse")]
    public void SetExpResponse(int trackIndex, string trackName, string response)
    {
        Debug.Log("answer is "+response);
        // export response to Google sheets
        // save to scriptable object for later use in the game
    }
}
