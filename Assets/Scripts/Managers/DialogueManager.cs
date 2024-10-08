using Yarn.Unity;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using TMPro;
using Yarn;
using System.Collections;

// In this script I will connect to yarn all of the commands it needs to have access to
public class DialogueManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    YarnProject yarnProject;
    // / keep a reference to the interaction that called to open the interface so that we can have it's id and the associated label
    [HideInInspector]
    public MusicDialogueData currentMusicInteraction;
    //[HideInInspector]
    // the mission that is associatedMission with the currently open dialogue
    MissionData missionToComplete;
    [HideInInspector]
    MissionData missionToStart;
    UIManager uiManager;
    [SerializeField]
    PlayerNameData playerNameData;
    [SerializeField]
    InputActionReference skipDialougeButton;
    [SerializeField]
    CanvasGroup cannotSkipTextGroup;
    float cannotSkipTextFadeTime = 0.2f;
    bool skippingDialouge= false, noSkipTextShowing = false, addedCommand = false;
    string lastNodeName = "init";
    void Awake()
    {
        ServiceLocator.Instance.Register<DialogueManager>(this);
    }
    void OnEnable()
    {
        dialogueRunner.onDialogueComplete.AddListener(StopDialogue);
        skipDialougeButton.action.performed += context => SkipDialogue(); 
 
    }
    void OnDisable()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(StopDialogue);
        skipDialougeButton.action.performed -= context => SkipDialogue();

    }
    private void Start()
    {
        yarnProject = dialogueRunner.yarnProject;
        dialogueRunner.VariableStorage.SetValue("$playerName", playerNameData.PlayerName);
        uiManager = ServiceLocator.Instance.Get<UIManager>();
        dialogueRunner.AddCommandHandler("OMD", delegate { dialogueRunner.Stop(); uiManager.OpenMusicDialogueUI();});
        dialogueRunner.AddCommandHandler("ExitGame", ServiceLocator.Instance.Get<GameManager>().ExitGame);
    }

    private void CompleteCurrentMission(bool successful = true)
    {
        missionToComplete.EndMission(successful);
    }
        private void OnApplicationPause(bool pause)
    {
        
    }
    public void SetMissionToComplete(MissionData data)
    {
        if (data == null)
        {
            Debug.LogError("Trying to set mission to complete to null");
            return;
        }
        Debug.Log("mission to complete is set to " + data.Name+" by music interactable");
        missionToComplete = data;
        if (!addedCommand)
        {
            addedCommand = true;
            // finish mission successfully
            dialogueRunner.AddCommandHandler("FMS", delegate { CompleteCurrentMission(true); });
            // finish mission failed
            dialogueRunner.AddCommandHandler("FMF", delegate { CompleteCurrentMission(false); });
        }
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
        lastNodeName = nodeToStart;
        if (missionToStart != null)
            missionToStart.StartMission();
    }


    public void SkipDialogue()
    {
        if (string.IsNullOrWhiteSpace(lastNodeName))
        {
            Debug.LogError("Trying to skip dialogue but lastNodeName is null");
            return;
        }
        var headers = yarnProject.GetHeaders(lastNodeName);
        if (headers == null)
        {
            Debug.LogError("Headers are null");
            return;
        }
        if (headers.ContainsKey("mandatory") && headers["mandatory"].IndexOf("T") >= 0)
        {
            if (noSkipTextShowing)
                return;
            // this node is marked as mandatory, do not skip it. 
            if (this != null && gameObject != null)
            {
                StartCoroutine(ShowCannotSkipText());
            }
            return;
        }
        // if it either has no metadata saying it is mandatory, or mandatory is marked as F
        if (!headers.ContainsKey("nextMandatory"))
        {
            Debug.LogWarning("Skipping dialogue but next node header is not found.");
            dialogueRunner.Stop();
            return;
        }

        if (headers["nextMandatory"].Count != 1)
        {
            Debug.LogError("next mandatory field for node " + lastNodeName + " is not filled in correctly. ");
            return;
        }
        string nextNode = headers["nextMandatory"][0];
        Debug.Log("skipping dialogue");
        skippingDialouge = true;
        // Call before starting a new node to finish the previous one and allow skipping.
        dialogueRunner.Stop();
        StartDialogue(nextNode);
        skippingDialouge = false;   
    }

    IEnumerator ShowCannotSkipText()
    {
        noSkipTextShowing = true;
        yield return StartCoroutine(Effects.FadeAlpha(cannotSkipTextGroup, 0, 1, cannotSkipTextFadeTime));
        yield return new WaitForSeconds(cannotSkipTextFadeTime);
        yield return StartCoroutine(Effects.FadeAlpha(cannotSkipTextGroup, 1, 0, cannotSkipTextFadeTime));
        noSkipTextShowing = false;
    }

    // called by dialogueRunner.Stop()
    void StopDialogue()
    {
        dialogueRunner.StopAllCoroutines();
        // if we are skipping the dialogue, we don't want to close the UI since the next node will start immediately after
        if (skippingDialouge)
            return;
        // hide the views on the canvas 
        foreach (var dialogueView in dialogueRunner.dialogueViews)
        {
            if (dialogueView == null || dialogueView.isActiveAndEnabled == false) continue;

            dialogueView.DialogueComplete();
            dialogueView.GetComponent<CanvasGroup>().alpha = 0;
        }
        // change input map 
        uiManager.CloseDialogueUI();
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
        ServiceLocator.Instance.Get<ExportManager>().ExportData(AudioManager.Instance.GetCurrentTrack(), currentMusicInteraction, lastNodeName);
        currentMusicInteraction = null;
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
        if (string.IsNullOrEmpty(lastNodeName))
        {
            Debug.LogError("last seen dialogue node is undefined ");
            return;
        }
        FinishMusicDialogue();

    }
    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
