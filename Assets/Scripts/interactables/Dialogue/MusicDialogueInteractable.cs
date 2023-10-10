using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;

[RequireComponent(typeof(Collider2D))]
// music dialogue intractable will hold a reference to music dialogue data to let the dialogue manager know this is the data that needs to be opend once the command OMD has been used
// alternatively, pass the name of the data you want to open as an argument to the OMD command, the dialogue manager will have a reference too all of these scriptable objects so everything can be handles from whithin the yarn script
public class MusicDialogueInteractable : MonoBehaviour
{
    [SerializeField]
    private bool interactable = true;
    [SerializeField]
    MusicDialogueData data;
    DialogueManager dialogueManager;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!GetComponent<Collider2D>().isTrigger)
            Debug.LogWarning(gameObject.name + "'s collider is not set to trigger. Interaction will not happen.");
        PerformDataSyntaxCheck();
    }
    private void Start()
    {
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
    }
    void PerformDataSyntaxCheck()
    {
        if(data == null)
        {
            Debug.LogError(gameObject.name + "'s music dialogue data is not set! ");
            return;
        }
        if (string.IsNullOrEmpty(data.InteractionID))
        {
            Debug.LogError("No interaction id");
        }
        if (string.IsNullOrEmpty(data.interactionName))
        {
            Debug.LogError("No interaction name");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        dialogueManager.currentMusicInteraction = data;
    }
}
