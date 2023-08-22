using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]

public class UIInterface : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    protected virtual void Awake()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }
    protected virtual void Start()
    {   
        MakeInvisible();
    }
    public virtual void MakeVisible()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        ServiceLocator.Instance.Get<InputManager>().ActivateUIMap();
    }
    public virtual void MakeInvisible()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        ServiceLocator.Instance.Get<InputManager>().ActivatePlayerMap();
    }
}
