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
    string lastNodeName = null;
    [SerializeField]
    PlayerNameData playerNameData;
    [SerializeField]
    InputActionReference skipDialougeButton;
    [SerializeField]
    CanvasGroup cannotSkipTextGroup;
    float cannotSkipTextFadeTime = 0.2f;
    bool skippingDialouge= false, noSkipTextShowing = false;
    void Awake()
    {
        ServiceLocator.Instance.Register<DialogueManager>(this);
    }
    void OnEnable()
    {
        dialogueRunner.onDialogueComplete.AddListener(FinishDialogue);
        skipDialougeButton.action.performed += context => SkipDialogue(); 
 
    }
    void OnDisable()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(FinishDialogue);
        skipDialougeButton.action.performed -= context => SkipDialogue();

    }
    private void Start()
    {
        yarnProject = dialogueRunner.yarnProject;
        dialogueRunner.VariableStorage.SetValue("$playerName", playerNameData.PlayerName);
        uiManager = ServiceLocator.Instance.Get<UIManager>();
        dialogueRunner.AddCommandHandler("ExitGame", ServiceLocator.Instance.Get<GameManager>().ExitGame);
        dialogueRunner.AddCommandHandler("OMD",delegate { StopDialogue(); uiManager.OpenMusicDialogueUI(); });
        // finish mission successfuly
        dialogueRunner.AddCommandHandler("FMS", delegate { missionToComplete.EndMission(); });
        // finish mission insuccessfuly
        dialogueRunner.AddCommandHandler("FMF", delegate { missionToComplete.EndMission(); });
        dialogueRunner.AddCommandHandler("SM", delegate { missionToStart.StartMission(); });
        //dialogueRunner.VariableStorage.(customVariableStorage);
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
        lastNodeName = dialogueRunner.CurrentNodeName;
        if (missionToStart != null)
            missionToStart.StartMission();
    }


    public void SkipDialogue()
    {
        if (dialogueRunner == null)
        {
            Debug.LogError("Dialogue runner is set to null in dialogue manager.");
            return;
        }
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
            StartCoroutine(ShowCannotSkipText());
            return;
        }
        // if it either has no metadata saying it is mandatory, or mandatory is marked as F
        if (headers.ContainsKey("nextMandatory"))
        {
            if (headers["nextMandatory"].Count != 1)
            {
                Debug.LogError("next mandatory field for node " + lastNodeName + " is not filled in correctly. ");
                return;
            }
            string nextNode = headers["nextMandatory"][0];
            skippingDialouge = true;
            StopDialogue();
            StartDialogue(nextNode);
            return;
        }
        else
            Debug.LogWarning("Skipping dialogue but next node header is not found.");
        //if we got here, just skip it
        Debug.Log("skipping dialogue");
        StopDialogue();
    }

    IEnumerator ShowCannotSkipText()
    {
        noSkipTextShowing = true;
        yield return StartCoroutine(Effects.FadeAlpha(cannotSkipTextGroup, 0, 1, cannotSkipTextFadeTime));
        yield return new WaitForSeconds(cannotSkipTextFadeTime);
        yield return StartCoroutine(Effects.FadeAlpha(cannotSkipTextGroup, 1, 0, cannotSkipTextFadeTime));
        noSkipTextShowing = false;
    }
    // for stopping a dialogue while its active
    void StopDialogue()
    {
        foreach (var view in dialogueRunner.dialogueViews)
        {
            if(view!= null)
                view.GetComponent<CanvasGroup>().alpha = 0;
        }
        dialogueRunner.Stop();
    }
    // Please note that in order to mark the associated mission as completed at the end of a dialogue
    // you need to do that from the yarn script itself and say if it was sucessful or not.
    // actually, for now I will do it here, so it will always be marked as completed successfuly for now.
    void FinishDialogue()
    {
        // if we are skipping the dialoue, we must terminate the current node we are in the middle of before starting the next node (yarn requirments).
        // but we don't want to switch maps if this is the case, since we still need the dialogue map
        if(skippingDialouge) 
        {
            skippingDialouge = false;
            return;
        }
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
        ServiceLocator.Instance.Get<ExportManager>().ExportData(AudioManager.Instance.GetCurrentTrack(), currentMusicInteraction, lastNodeName);
        currentMusicInteraction = null;
        lastNodeName = null;
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
}
