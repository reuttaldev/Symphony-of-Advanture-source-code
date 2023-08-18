using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MainInterfaceUI : MonoBehaviour
{
    [SerializeField]
    private InputActionReference openCloseMenuAction;
    private CanvasGroup canvasGroup;
    private bool visible;
    private UnityEvent makeVisible;

    private void Awake()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();
        MakeInvisible();
    }
    private void OnEnable()
    {
        openCloseMenuAction.action.performed += context => ControlVisibility();
    }
    private void OnDisable()
    {
        openCloseMenuAction.action.performed -= context => ControlVisibility();
    }
    void ControlVisibility()
    {
        if(visible) 
            MakeInvisible();
        else 
            MakeVisible();
    }
     void MakeVisible()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        visible = true;
    }
    void MakeInvisible()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        visible = false;
    }
}
