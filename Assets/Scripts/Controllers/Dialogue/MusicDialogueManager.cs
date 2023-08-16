using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MusicDialogueManager : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    void Awake()
    {
        dialogueRunner = gameObject.GetComponent<DialogueRunner>();
        // making it possible for commands to be called from yarn scripts
        //dialogueRunner.AddCommandHandler("ExportTrackData", PlayerChoseTrackInDialogue);
    }
    void Start()
    {
        
    }
    public void StartMusicDialogue()
    {

    }
    public void PlayerChoseTrackInDialogue(string interactionID, string trackID, string response)
    {
        // export the answer
        ManagerLocator.Instance.Get<ExportManager>().ExportTrackData(interactionID, trackID, response);
        // save the answer locally 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
