using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using Yarn.Unity.Example;

public class UIManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    private WalkmanUI walkmanUI;
    [SerializeField]
    public ItemInteractableView interactableView;
    [SerializeField]
    private InputActionReference openWalkmanAction;
    [SerializeField]
    private InputActionReference closeWalkmanAction;
    [SerializeField]
    private InputActionReference escapeAction; // this is in the plyer action map!!
    private InputManager inputManager;
    [SerializeField]
    GameObject extraPanel;
    [SerializeField]
    TMP_Text escapeText;
    [SerializeField]
    float holdDurationToEsc = 3;
    bool uiPanalOpen; // this will be true if any of our UI menus are currently open
    private void Awake()
    {
        ServiceLocator.Instance.Register<UIManager>(this);
        uiPanalOpen = false;
    }
    private void OnEnable()
    {
        openWalkmanAction.action.performed += OpenWalkmanInterface;
        closeWalkmanAction.action.performed += CloseWalkmanInterface;
        escapeAction.action.performed += EscapeUI;
    }

    private void OnDisable()
    {
        openWalkmanAction.action.performed -= OpenWalkmanInterface;
        closeWalkmanAction.action.performed -= CloseWalkmanInterface;
        escapeAction.action.performed -= EscapeUI;
    }
    private void Start()
    {
        inputManager = ServiceLocator.Instance.Get<InputManager>();
        walkmanUI.gameObject.SetActive(false);
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

    void OpenWalkmanInterface(InputAction.CallbackContext context)
    {
        Debug.LogError("pressed 1");
        if (walkmanUI.open)
        {
            CloseWalkmanInterface();
            return;
        }
        // open menu only if nothing else is open
        if (!SwitchUIMap())
            return;
        uiPanalOpen = true;
        walkmanUI.gameObject.SetActive(true);
        walkmanUI.Open(true);
    }
    void CloseWalkmanInterface(InputAction.CallbackContext context) 
    {
        if (!walkmanUI.open)
            return;
        walkmanUI.gameObject.SetActive(false);
        walkmanUI.Close();
        CloseAndSwitchUIMap();
    }
    public void OpenMusicDialogueUI()
    {
        SwitchUIMap();
        uiPanalOpen = true;
        walkmanUI.gameObject.SetActive(true);
        walkmanUI.Open(false);

    }
    public void CloseMusicDialogueUI()
    {
        walkmanUI.Close();
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
        {
            Debug.Log("Not closing dialouge UI because something else is open");
            return;
        }
        CloseAndSwitchUIMap();
        uiPanalOpen = false;
    }
    void EscapeUI(InputAction.CallbackContext context)
    {
        escapeText.text = "Hold ESC to exit";
        StopCoroutine(ShowEsccapeText());
        StartCoroutine(ShowEsccapeText());
    }

    public IEnumerator ShowEsccapeText()
    {
        float elapsedTime = 0;
        float showDotInterval = holdDurationToEsc / 4;
        while (escapeAction.action.IsPressed())
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= holdDurationToEsc)
                ServiceLocator.Instance.Get<GameManager>().ExitGame();
            int i = Mathf.FloorToInt(elapsedTime / showDotInterval) % 4; // Cycle through 0-3
            escapeText.text = "Exiting" + new string('.', i); // Append dots based on the index
            yield return null;
        }
        escapeText.text = "";
        yield break;

    }

}
