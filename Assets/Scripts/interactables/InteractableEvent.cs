using UnityEngine;
using UnityEngine.Events;


public class InteractableEvent : Interactable
{
    [SerializeField]
    UnityEvent onTrigger;
    protected override void TriggerInteraction()
    {
        onTrigger.Invoke();
    }
}
