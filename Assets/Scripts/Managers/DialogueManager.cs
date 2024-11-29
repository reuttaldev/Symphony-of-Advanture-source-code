using Yarn.Unity;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using TMPro;
using Yarn;
using System.Collections;
using System;

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
    UIManager uiManager;
    [SerializeField]
    PlayerNameData playerNameData;
    [SerializeField]
    InputActionReference continueButton,interuptButton,skipButton, skipForTesting;
    [SerializeField]
    LineView lineView;
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
        continueButton.action.performed += ContinueDialouge;
        interuptButton.action.performed += InterruptDialouge;
        skipButton.action.performed += SkipDialogue;
        skipForTesting.action.performed += SkipDialogueForTesting;

    }
    void OnDisable()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(StopDialogue);
        continueButton.action.performed -= ContinueDialouge;
        interuptButton.action.performed -= InterruptDialouge;
        skipButton.action.performed -= SkipDialogue;
        skipForTesting.action.performed -= SkipDialogueForTesting;

    }
    private void Start()
    {
        yarnProject = dialogueRunner.yarnProject;
        dialogueRunner.VariableStorage.SetValue("$playerName", playerNameData.PlayerName);
        uiManager = ServiceLocator.Instance.Get<UIManager>();
        dialogueRunner.AddCommandHandler("OMD", delegate {  uiManager.OpenMusicDialogueUI();});
        dialogueRunner.AddCommandHandler("ExitGame", ServiceLocator.Instance.Get<GameManager>().ExitGame);

        // finish mission successfully
        dialogueRunner.AddCommandHandler("FMS", delegate { FinishDialogueMission(); });
        // finish mission failed
        dialogueRunner.AddCommandHandler("FMF", delegate { FinishDialogueMission(false); });
        dialogueRunner.AddCommandHandler("STOP", delegate { dialogueRunner.Stop(); });
    }

    #region MISSION CONTROLS
    public void SetMissionToComplete(MissionData data)
    {
        if (data == null)
        {
            Debug.LogError("Trying to set mission to complete to null");
            return;
        }
        if (data.State == MissionState.CompletedUnSuccessfully || data.State == MissionState.CompletedSuccessfully)
        {
            //Debug.LogError("Trying to set mission to complete in DialogueManager a mission that is already done");
            return;
        }
        Debug.Log("mission to complete is set to " + data.Name+" by dialogue interactable");
        missionToComplete = data.GetRuntimeInstance<MissionData>();
    }

    private void FinishDialogueMission(bool sucessful = true)
    {
        if(missionToComplete == null)
        {
            Debug.LogError("No mission associated to this dialogue, but it was called to finish the mission");
            return;
        }
        missionToComplete.EndMission(sucessful);
    }
    public void SetMusicInteraction(MusicDialogueData data)
    {
        currentMusicInteraction = data;
    }
    public void PlayerLabeledTrack() // this will be called by a unity event on Music Dialogue UI, don't forget to set that this will be called on the inspector on music dialogue UI
    {
        if (currentMusicInteraction == null)
        {
            Debug.LogError("Current interaction is null. Who is calling open music dialogue? Need interaction ID");
            return;
        }
        //if (string.IsNullOrEmpty(currentMusicInteraction.GlobalID))
        //{
         //   Debug.LogError("No interaction id");
          //  return;
        //}
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
    #endregion
    #region LOGIC CONTROLS
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
    }
    private void SkipDialogue(InputAction.CallbackContext context)
    {
        Debug.Log("pressed");
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
            //Debug.LogWarning("Skipping dialogue but next node header is not found.");
            dialogueRunner.Stop();
            return;
        }

        if (headers["nextMandatory"].Count != 1)
        {
            Debug.LogError("next mandatory field for node " + lastNodeName + " is not filled in correctly. ");
            return;
        }
        string nextNode = headers["nextMandatory"][0];
        skippingDialouge = true;
        // Call before starting a new node to finish the previous one and allow skipping.
        dialogueRunner.Stop();
        StartDialogue(nextNode);
        skippingDialouge = false;
    }
    private void SkipDialogueForTesting(InputAction.CallbackContext context)
    {
        dialogueRunner.Stop();
        if (missionToComplete != null)
            missionToComplete.EndMission();
    }
    void FinishMusicDialogue()
    {
        string nextNode = currentMusicInteraction.onCompletionNode;
        uiManager.CloseMusicDialogueUI();
        if (!string.IsNullOrEmpty(nextNode))
        {
            if (currentMusicInteraction.missionToComplete != null)
                missionToComplete = currentMusicInteraction.missionToComplete;
            StartDialogue(nextNode);

        }
        ServiceLocator.Instance.Get<ExportManager>().ExportData(AudioManager.Instance.GetCurrentTrack(), currentMusicInteraction, lastNodeName);
        currentMusicInteraction = null;
    }
    #endregion
    #region UI CONTROLS
    private void ContinueDialouge(InputAction.CallbackContext context)
    {
        lineView.UserRequestedViewAdvancement();
    }
    private void InterruptDialouge(InputAction.CallbackContext context)
    {
        lineView.UserRequestedViewAdvancement();
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

            //dialogueView.DialogueComplete();
            dialogueView.GetComponent<CanvasGroup>().alpha = 0;
        }
        // change input map 
        uiManager.CloseDialogueUI();
    }
    IEnumerator ShowCannotSkipText()
    {
        noSkipTextShowing = true;
        yield return StartCoroutine(Effects.FadeAlpha(cannotSkipTextGroup, 0, 1, cannotSkipTextFadeTime));
        yield return new WaitForSeconds(cannotSkipTextFadeTime);
        yield return StartCoroutine(Effects.FadeAlpha(cannotSkipTextGroup, 1, 0, cannotSkipTextFadeTime));
        noSkipTextShowing = false;
    }
    #endregion
}
