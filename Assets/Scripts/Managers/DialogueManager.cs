using Yarn.Unity;
using UnityEngine;
using UnityEngine.Events;

    // In this script I will connect to yarn all of the commands it needs to have access to
public class DialogueManager : MonoBehaviour, IRegistrableService
{
    DialogueRunner dialogueRunner;
    // / keep a reference to the interaction that called to open the interface so that we can have it's id and the associated label
    //[HideInInspector]
    public MusicDialogueData currentMusicInteraction;
    UIManager uiManager;
    string lastReadDialogueNode = null;
    void Awake()
    {
        dialogueRunner = gameObject.GetComponent<DialogueRunner>();
        ServiceLocator.Instance.Register<DialogueManager>(this);

    }
    void OnEnable()
    {
        dialogueRunner.onDialogueComplete.AddListener(FinishDialogue);
    }
    void OnDisable()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(FinishDialogue);
    }
    private void Start()
    {
        uiManager = ServiceLocator.Instance.Get<UIManager>();
        dialogueRunner.AddCommandHandler("ExitGame", ServiceLocator.Instance.Get<GameManager>().ExitGame);
        dialogueRunner.AddCommandHandler("OMD", StartMusicDialogue);
        //dialogueRunner.AddCommandHandler("CMD", FinishMusicDialogue);
    }
    public void StartDialogue(string nodeToStart)
    {
        if (string.IsNullOrEmpty(nodeToStart))
        {
            Debug.LogError("conversationStartNode is null");
            return;
        }
        if (dialogueRunner.IsDialogueRunning)
        {
            Debug.LogError("Dialogue already running");
            return;
        }
        dialogueRunner.StartDialogue(nodeToStart);
        uiManager.OpenDialogueUI();
        lastReadDialogueNode = dialogueRunner.CurrentNodeName;
    }

    public void FinishDialogue() 
    {
        uiManager.CloseDialogueUI();
    }

    public void StartMusicDialogue()
    {
        uiManager.OpenMusicDialogueUI();
    }
    public void FinishMusicDialogue()
    {
        currentMusicInteraction = null;
        lastReadDialogueNode = null;
        uiManager.CloseMusicDialogueUI();
    }
    public void PlayerLabeledTrack() // this will be called by a unity event on Music Dialogue UI, don't forget to set that this will be called on the inspector or music dialogue UI
    {
        if (currentMusicInteraction == null)
        {
            Debug.LogError("Current interaction is null. Who is calling open music dialogue? Need interaction ID");
            return;
        }
        if (string.IsNullOrEmpty(currentMusicInteraction.InteractionID))
        {
            Debug.LogError("No interaction id");
            return;
        }
        if (string.IsNullOrEmpty(currentMusicInteraction.interactionName))
        {
            Debug.LogError("No interaction name");
            return;
        }
        if (string.IsNullOrEmpty(lastReadDialogueNode))
        {
            Debug.LogError("last seen dialogue node is undefined ");
            return;
        }
        // the player can move on from this only if there was an active internet connection and the data was sent. otherwise, player will need to make a choice again and PlayerLabeledTrack method will be called again
        EnsureConnectionAndExportData();
    }
    public void EnsureConnectionAndExportData()
    {
        InternetConnectionManager it = ServiceLocator.Instance.Get<InternetConnectionManager>();
        StartCoroutine(it.CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                ServiceLocator.Instance.Get<ExportManager>().ExportData(AudioManager.Instance.GetCurrentTrack(), currentMusicInteraction, lastReadDialogueNode);
                // once player has made their choice, music dialogue is over. close the panel etc
                FinishMusicDialogue();
            }
            else
            {
                // pause the game if there is no connection.
                it.NoConnectionDetected();
            }
        }));
    }

}
