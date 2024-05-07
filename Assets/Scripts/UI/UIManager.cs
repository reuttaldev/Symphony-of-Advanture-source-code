using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class UIManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    private WalkmanUI walkmanUI;
    [SerializeField]
    private InputActionReference openWalkmanAction;
    [SerializeField]
    private InputActionReference closeWalkmanAction;
    private InputManager inputManager;
    bool uiPanalOpen; // this will be true if any of our UI menus are currently open

    private void Awake()
    {
        ServiceLocator.Instance.Register<UIManager>(this);
        uiPanalOpen = false;
    }
    private void OnEnable()
    {
        openWalkmanAction.action.performed += context => OpenWalkmanInterface();
        closeWalkmanAction.action.performed += context => CloseWalkmanInterface();
        // listen to when the player has made a choce in a music dialogue
    }
    private void Start()
    {
        inputManager = ServiceLocator.Instance.Get<InputManager>();
        walkmanUI.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        openWalkmanAction.action.performed -= context => OpenWalkmanInterface();
        closeWalkmanAction.action.performed -= context => CloseWalkmanInterface();
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

    void OpenWalkmanInterface()
    {
        // open menu only if nothing else is open
        if (!OpenAndSwitchUIMap())
            return;
        walkmanUI.gameObject.SetActive(true);
    }
    void CloseWalkmanInterface() 
    {
        walkmanUI.gameObject.SetActive(false);
        CloseAndSwitchUIMap();
    }
    public void OpenMusicDialogueUI()
    {
        //  we will often start the music dialogue from the regular dialogue, so no need to check if another UI window is open
        walkmanUI.gameObject.SetActive(true);
        ServiceLocator.Instance.Get<AudioManager>().PlayCurrentTrack();

    }
    public void CloseMusicDialogueUI()
    {
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
        if(!walkmanUI.gameObject.activeInHierarchy)
            CloseAndSwitchUIMap();
    }
}
