using Yarn.Unity;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

// In this script I will connect to yarn all of the commands it needs to have access to
public class DialogueManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    // / keep a reference to the interaction that called to open the interface so that we can have it's id and the associated label
    [HideInInspector]
    public MusicDialogueData currentMusicInteraction;
    //[HideInInspector]
    // the mission that is associatedMission with the currently open dialogue
    MissionData missionToComplete;
    [HideInInspector]
    MissionData missionToStart;
    UIManager uiManager;
    string lastReadDialogueNode = null;
    [SerializeField]
    PlayerNameData playerNameData;
    void Awake()
    {
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
        dialogueRunner.VariableStorage.SetValue("$playerName", playerNameData.PlayerName);
        uiManager = ServiceLocator.Instance.Get<UIManager>();
        dialogueRunner.AddCommandHandler("ExitGame", ServiceLocator.Instance.Get<GameManager>().ExitGame);
        dialogueRunner.AddCommandHandler("OMD",delegate { uiManager.OpenMusicDialogueUI(); });
        // finish mission successfuly
        dialogueRunner.AddCommandHandler("FMS", delegate { missionToComplete.EndMission(); });
        // finish mission insuccessfuly
        dialogueRunner.AddCommandHandler("FMF", delegate { missionToComplete.EndMission(); });
        dialogueRunner.AddCommandHandler("SM", delegate { missionToStart.StartMission(); });
        //dialogueRunner.VariableStorage.(customVariableStorage);
    }

    public void SetMissionToComplete(MissionData data)
    {
        Debug.Log("mission to complete is set to " + data.Name);
        missionToComplete = data;
    }
    public void SetMusicInteraction(MusicDialogueData data)
    {
        currentMusicInteraction = data;
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
        if (missionToStart != null)
            missionToStart.StartMission();
    }

    // Please note that in order to mark the associated mission as completed at the end of a dialogue
    // you need to do that from the yarn script itself and say if it was sucessful or not.
    // actually, for now I will do it here, so it will always be marked as completed successfuly for now.
    void FinishDialogue()
    {
        uiManager.CloseDialogueUI();
        missionToComplete = null;
        missionToStart = null;
    }
    void FinishMusicDialogue()
    {
        string nextNode = currentMusicInteraction.onCompletionNode;
        uiManager.CloseMusicDialogueUI();
        if(!string.IsNullOrEmpty(nextNode))
        {
            if (currentMusicInteraction.missionToComplete != null)
                missionToComplete = currentMusicInteraction.missionToComplete;
            StartDialogue(nextNode);

        }
        ServiceLocator.Instance.Get<ExportManager>().ExportData(AudioManager.Instance.GetCurrentTrack(), currentMusicInteraction, lastReadDialogueNode);
        currentMusicInteraction = null;
        lastReadDialogueNode = null;
    }
    public void PlayerLabeledTrack() // this will be called by a unity event on Music Dialogue UI, don't forget to set that this will be called on the inspector on music dialogue UI
    {
        if (currentMusicInteraction == null)
        {
            Debug.LogError("Current interaction is null. Who is calling open music dialogue? Need interaction ID");
            return;
        }
        if (string.IsNullOrEmpty(currentMusicInteraction.GlobalID))
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
        FinishMusicDialogue();

    }
}
