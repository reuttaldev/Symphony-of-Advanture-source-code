using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    private MainMenuUI mainInterfaceUI;
    [SerializeField]
    private MusicDialogueUI musicDialogueUI;
    [HideInInspector]
    // / keep a reference to the interaction that called to open the interface so that we can have it's id and the associated label
    public MusicDialogueInteractable currentInteraction = null;  
    [SerializeField]
    private WalkmanUI walkmanUI;
    [SerializeField]
    private InputActionReference openMainMenuAction;
    [SerializeField]
    private InputActionReference closeMainMenuAction;
    private InputManager inputManager;

    private void Awake()
    {
        ServiceLocator.Instance.Register<UIManager>(this);  
    }
    private void Start()
    {
        inputManager = ServiceLocator.Instance.Get<InputManager>();
    }
    private void OnEnable()
    {
        openMainMenuAction.action.performed += context => OpenMainMenu();
        closeMainMenuAction.action.performed += context => CloseMainMenu();
    }
    private void OnDisable()
    {
        openMainMenuAction.action.performed -= context => OpenMainMenu();
        closeMainMenuAction.action.performed -= context => CloseMainMenu();
    }
    void OpenMainMenu()
    {
        // only make visible if we are not in the middle of a dialogue and no other menu is open
        mainInterfaceUI.MakeVisible();
        walkmanUI.gameObject.SetActive(true);
        inputManager.ActivateUIMap();
    }
    void CloseMainMenu() 
    {
        mainInterfaceUI.MakeInvisible();
        walkmanUI.gameObject.SetActive(false);
        inputManager.ActivatePlayerMap();
    }
    public void OpenMusicDialogueUI()
    {
        if(currentInteraction == null)
        {
            Debug.LogError("Current interaction is null. Who is calling open music dialogue? Need interaction ID");
            return;
        }
        musicDialogueUI.MakeVisible();
        inputManager.ActivateUIMap();

    }
    public void CloseMusicDialogueUI()
    {
        musicDialogueUI.MakeInvisible();
        currentInteraction = null;
        inputManager.ActivatePlayerMap();
    }

    public void OpenDialogueUI()
    {
        inputManager.ActivateUIMap();

    }
    public void CloseDialogueUI()
    {
        inputManager.ActivatePlayerMap();

    }
}
