using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    protected bool interactable = true;
    [SerializeField]
    protected bool interactableMoreThanOnce = true; // whether this character should be enabled right now
    public virtual void Awake()
    {
        if (!GetComponent<Collider2D>().isTrigger)
            Debug.LogWarning(gameObject.name + "'s collider is not set to trigger. Interaction will not happen.");
    }
    protected virtual void DisableInteraction()
    {
        if (!interactableMoreThanOnce)
        {
            interactable = false;
            gameObject.SetActive(false);
        }
    }
    protected virtual void TriggerInteraction()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(interactable)
            TriggerInteraction();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interactable)
            DisableInteraction();
    }
}
