using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class UIManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    private UIInterface mainInterfaceUI;
    [SerializeField]
    private MusicDialogueUI musicDialogueUI;
    [SerializeField]
    private WalkmanUI walkmanUI;
    [SerializeField]
    private InputActionReference openMainMenuAction;
    [SerializeField]
    private InputActionReference closeMainMenuAction;
    private InputManager inputManager;
    bool uiPanalOpen; // this will be true if any of our UI menus are currently open
    DialogueManager dialogueManager;

    private void Awake()
    {
        ServiceLocator.Instance.Register<UIManager>(this);
        uiPanalOpen = false;
    }
    private void OnEnable()
    {
        openMainMenuAction.action.performed += context => OpenMainMenu();
        closeMainMenuAction.action.performed += context => CloseMainMenu();
        // listen to when the player has made a choce in a music dialogue
    }
    private void Start()
    {
        inputManager = ServiceLocator.Instance.Get<InputManager>();
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
        walkmanUI.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        openMainMenuAction.action.performed -= context => OpenMainMenu();
        closeMainMenuAction.action.performed -= context => CloseMainMenu();
    }

    bool OpenAndSwitchUIMap()
    {
        // if no other menu is already open
        if (uiPanalOpen)
            return false;
        uiPanalOpen = true;
        inputManager.ActivateUIMap();
        return true;
    }
    // call this when closing any UI interface
    void CloseAndSwitchUIMap()
    {
        uiPanalOpen = false;
        inputManager.ActivatePlayerMap();
    }

    // main menu contains the walkman UI inside of it, but not just that
    void OpenMainMenu()
    {
        // open menu only if nothing else is open
        if (!OpenAndSwitchUIMap())
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
        CloseAndSwitchUIMap();
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
        CloseAndSwitchUIMap();
    }

    public void OpenDialogueUI()
    {
        OpenAndSwitchUIMap();
    }
    public void CloseDialogueUI()
    {
        // if we have opened the music dialogue UI though a yarn script, don't close
        if(!musicDialogueUI.active)
            CloseAndSwitchUIMap();
    }
}
