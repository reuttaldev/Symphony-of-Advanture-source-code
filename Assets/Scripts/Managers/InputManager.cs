using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, IRegistrableService
{
    PlayerInput playerInput;
    string lastActiveMapName;
    void Awake()
    {
        ServiceLocator.Instance.Register<InputManager>(this);
        playerInput = GetComponent<PlayerInput>();
        lastActiveMapName = playerInput.currentActionMap.name;
    }
    public void ActivatePlayerMap()
    {
        lastActiveMapName = playerInput.currentActionMap.name;
        playerInput.SwitchCurrentActionMap("Player");
    }
    public void ActivateUIMap()
    {
        lastActiveMapName = playerInput.currentActionMap.name;  
        playerInput.SwitchCurrentActionMap("UI");
    }
    public void ActivatePausedUIMap()
    {
        lastActiveMapName = playerInput.currentActionMap.name;
        playerInput.SwitchCurrentActionMap("Paused UI");
    }

    // activate the action map that was on before the last one we used
    public void ActionMapGoBack()
    {
        playerInput.SwitchCurrentActionMap(lastActiveMapName);
    }

}
