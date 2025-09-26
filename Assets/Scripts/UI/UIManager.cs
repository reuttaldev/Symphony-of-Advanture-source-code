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
        escapeAction.action.performed += EscapeUI;
    }

    private void OnDisable()
    {
        escapeAction.action.performed -= EscapeUI;
    }
    private void Start()
    {
        inputManager = ServiceLocator.Instance.Get<InputManager>();
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

    
    public void OpenDialogueUI()
    {
        uiPanalOpen = SwitchUIMap();
    }
    public void CloseDialogueUI()
    {

        CloseAndSwitchUIMap();
        uiPanalOpen = false;
    }
    void EscapeUI(InputAction.CallbackContext context)
    {
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
            escapeText.text = "Hold ESC to exit" + new string('.', i); // Append dots based on the index
            yield return null;
        }
        escapeText.text = "";
        yield break;

    }

}
