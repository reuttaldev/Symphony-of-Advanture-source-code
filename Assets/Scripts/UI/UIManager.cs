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
    GameObject musicDialogueText;
    [SerializeField]
    private InputActionReference openWalkmanAction;
    [SerializeField]
    private InputActionReference closeWalkmanAction;
    private InputManager inputManager;
    [SerializeField]
    GameObject extraPanel;
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

    public bool SwitchUIMap()
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

    void OpenWalkmanInterface(bool manual = false)
    {
        // open menu only if nothing else is open
        if (!SwitchUIMap())
            return;
        uiPanalOpen = true;
        walkmanUI.gameObject.SetActive(true);
        walkmanUI.Open();
        musicDialogueText.SetActive(false);
    }
    void CloseWalkmanInterface() 
    {
        walkmanUI.gameObject.SetActive(false);
        walkmanUI.Close(manual:true);
        CloseAndSwitchUIMap();
    }
    public void OpenMusicDialogueUI()
    {
        //  we will often start the music dialogue from the regular dialogue, so no need to check if another UI window is open
        uiPanalOpen = true;
        walkmanUI.gameObject.SetActive(true);
        walkmanUI.Open();
        musicDialogueText.SetActive(false);

    }
    public void CloseMusicDialogueUI()
    {
        walkmanUI.Close(manual:false);
        walkmanUI.gameObject.SetActive(false);
        //musicDialogueText.SetActive(false);

        CloseAndSwitchUIMap();
    }

    public void OpenDialogueUI()
    {
        uiPanalOpen = SwitchUIMap();
    }
    public void CloseDialogueUI()
    {
        // if we have opened the music dialogue UI though a yarn script, don't close
        if (walkmanUI.gameObject.activeInHierarchy || (extraPanel != null && extraPanel.gameObject.activeInHierarchy))
            return;
        CloseAndSwitchUIMap();
    }
}
