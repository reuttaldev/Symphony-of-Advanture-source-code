using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableAnimationTrigger : Interactable
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    string parameter;
    protected override void TriggerInteraction()
    {
        animator.SetTrigger(parameter);
    }
}
