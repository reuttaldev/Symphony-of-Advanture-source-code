using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, IRegistrableService
{
    PlayerInput playerInput;
    void Awake()
    {
        ServiceLocator.Instance.Register<InputManager>(this);
        playerInput = GetComponent<PlayerInput>();
    }
    public void ActivatePlayerMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }
    public void ActivateUIMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

}
