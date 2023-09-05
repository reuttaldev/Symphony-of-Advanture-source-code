using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class UIManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    private MainMenuUI mainInterfaceUI;
    [SerializeField]
    private MusicDialogueUI musicDialogueUI;
    // / keep a reference to the interaction that called to open the interface so that we can have it's id and the associated label
    [HideInInspector]
    public MusicDialogueInteractable currentMusicInteraction;  
    [SerializeField]
    private WalkmanUI walkmanUI;
    [SerializeField]
    private InputActionReference openMainMenuAction;
    [SerializeField]
    private InputActionReference closeMainMenuAction;
    private InputManager inputManager;
    private DialogueRunner dialogueRunner;
    bool uiPanalOpen; // this will be true if any of our UI menus are currently open

    private void Awake()
    {
        ServiceLocator.Instance.Register<UIManager>(this);
        uiPanalOpen = false;
    }
    private void OnEnable()
    {
        openMainMenuAction.action.performed += context => OpenMainMenu();
        closeMainMenuAction.action.performed += context => CloseMainMenu();
        if(dialogueRunner == null)
            dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(CloseDialogueUI);
        // listen to when the player has made a choce in a music dialogue
        musicDialogueUI.OnPlayerLabeledTrack.AddListener(PlayerLabeledTrack);
    }
    private void Start()
    {
        inputManager = ServiceLocator.Instance.Get<InputManager>();
        walkmanUI.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        openMainMenuAction.action.performed -= context => OpenMainMenu();
        closeMainMenuAction.action.performed -= context => CloseMainMenu();
        dialogueRunner.onDialogueComplete.RemoveListener(CloseDialogueUI);
        musicDialogueUI.OnPlayerLabeledTrack.RemoveListener(PlayerLabeledTrack);
    }

    bool CanOpen()
    {
        // if no other menu is already open
        if (uiPanalOpen)
            return false;
        uiPanalOpen = true;
        inputManager.ActivateUIMap();
        return true;
    }
    // call this when closing any UI interface
    void Close()
    {
        uiPanalOpen = false;
        inputManager.ActivatePlayerMap();
    }
    void OpenMainMenu()
    {
        // open menu only if nothing else is open
        if (!CanOpen())
            return;
        // only make visible if we are not in the middle of a dialogue and no other menu is open
        mainInterfaceUI.MakeVisible();
        walkmanUI.gameObject.SetActive(true);
    }
    void CloseMainMenu() 
    {
        // close only if open already
        if (!mainInterfaceUI.active)
            return;
        mainInterfaceUI.MakeInvisible();
        walkmanUI.gameObject.SetActive(false);
        Close();
    }
    public void OpenMusicDialogueUI()
    {
        //  we will often start the music dialogue from the regular dialogue, so no need to check if another UI window is open

        walkmanUI.gameObject.SetActive(true);
        musicDialogueUI.MakeVisible();
    }
    public void CloseMusicDialogueUI()
    {
        musicDialogueUI.MakeInvisible();
        walkmanUI.gameObject.SetActive(false);
        currentMusicInteraction = null;
        Close();
    }

    public void OpenDialogueUI(string conversationStartNode)
    {
        if (!CanOpen())
            return;
        if (string.IsNullOrEmpty(conversationStartNode))
        {
            Debug.LogError("conversationStartNode is null");
            return;
        }
        if(dialogueRunner.IsDialogueRunning)
        {
            Debug.LogError("Dialogue already running");
            return;
        }
        dialogueRunner.StartDialogue(conversationStartNode);
    }
    public void CloseDialogueUI()
    {
        // if we have opened the music dialogue UI though a yarn script, don't close
        if(!musicDialogueUI.active)
            Close();
    }

    public void PlayerLabeledTrack()
    {

        Debug.Log("Player labeled track");
        if(currentMusicInteraction == null)
        {
            Debug.LogError("Current interaction is null. Who is calling open music dialogue? Need interaction ID");
            return;
        }
        if(string.IsNullOrEmpty(currentMusicInteraction.GetInteractionId()))
        {
            Debug.LogError("No interaction id");
            return;
        }

        ServiceLocator.Instance.Get<ExportManager>().ExportData(AudioManager.Instance.GetCurrentTrack(), currentMusicInteraction.GetInteractionId(), currentMusicInteraction.GetEmotion());
        CloseMusicDialogueUI();
    }
}
