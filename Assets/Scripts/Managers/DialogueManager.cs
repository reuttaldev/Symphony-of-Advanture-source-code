using Yarn.Unity;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using TMPro;
using Yarn;
using System.Collections;
using System;
using System.Net.Mail;

// In this script I will connect to yarn all of the commands it needs to have access to
public class DialogueManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    YarnProject yarnProject;
    // / keep a reference to the interaction that called to open the interface so that we can have it's id and the associated label
    // the mission that is associatedMission with the currently open dialogue
    MissionData missionToComplete;
    UIManager uiManager;
    [SerializeField]
    InputActionReference continueButton,interuptButton,skipButton, skipForTesting;
    [SerializeField]
    LineView lineView;
    [SerializeField]
    CanvasGroup cannotSkipTextGroup;
    float cannotSkipTextFadeTime = 0.2f;
    bool skippingDialouge= false, noSkipTextShowing = false, addedCommand = false;
    string lastNodeName = "init";
    private bool useAlternativeView = false;
    public GameObject defaultView,portraitImage, alternativeView;
    public Transition transition;
    void Awake()
    {
        ServiceLocator.Instance.Register<DialogueManager>(this);
        transition = FindAnyObjectByType<Transition>();
    }
    void OnEnable()
    {
        dialogueRunner.onDialogueComplete.AddListener(StopDialogue);
        continueButton.action.performed += ContinueDialouge;
        interuptButton.action.performed += InterruptDialouge;

    }
    void OnDisable()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(StopDialogue);
        continueButton.action.performed -= ContinueDialouge;
        interuptButton.action.performed -= InterruptDialouge;

    }

    private void Start()
    {
        yarnProject = dialogueRunner.yarnProject;
        uiManager = ServiceLocator.Instance.Get<UIManager>();
        dialogueRunner.AddCommandHandler("ExitGame", ServiceLocator.Instance.Get<GameManager>().ExitGame);

        // finish mission successfully
        dialogueRunner.AddCommandHandler("FMS", delegate { FinishDialogueMission(); });
        // finish mission failed
        dialogueRunner.AddCommandHandler("FMF", delegate { FinishDialogueMission(false); });
        dialogueRunner.AddCommandHandler("STOP", delegate { dialogueRunner.Stop(); });
        dialogueRunner.AddCommandHandler("Normal", delegate { SetDialogueView(false); });
        dialogueRunner.AddCommandHandler("Bubble", delegate { SetDialogueView(true); });
        dialogueRunner.AddCommandHandler("EventA", delegate { transition.eventA.Invoke(); });
        dialogueRunner.AddCommandHandler("EventB", delegate { transition.eventB.Invoke(); });
        dialogueRunner.AddCommandHandler("EventC", delegate { transition.eventC.Invoke(); });
        dialogueRunner.AddCommandHandler("Transition", delegate { transition.TransitionScene(); });
    }
    public void SetDialogueView(bool useAlternative)
    {
        useAlternativeView = useAlternative;
        UpdateDialogueView();
    }

    private void UpdateDialogueView()
    {
        if (useAlternativeView)
        {
            transition.betweenDefaultToBubble.Invoke();
            defaultView.SetActive(false);
            portraitImage.SetActive(false);
            alternativeView.SetActive(true);
        }
        else
        {
            transition.betweenBubbleToDefault.Invoke();
            defaultView.SetActive(true);
            portraitImage.SetActive(true);
            alternativeView.SetActive(false);
        }
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
            if (dialogueView == null || dialogueView.isActiveAndEnabled == false) 
                continue;

            dialogueView.DialogueComplete();
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
