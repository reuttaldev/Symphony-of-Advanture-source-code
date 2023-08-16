using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SimpleSingleton<InputManager>
{
    PlayerInput playerInput;
    [SerializeField]
    DialogueManager dialogueManager;

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
    }

    public void ActivatePlayerMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }
    public void ActivateUIMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }
    public void ActivateDialogueMap()
    {
        playerInput.SwitchCurrentActionMap("Dialogue Box");
    }

}
