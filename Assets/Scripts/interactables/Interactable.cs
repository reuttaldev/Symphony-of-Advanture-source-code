using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    protected bool interactable = true;
    [SerializeField]
    protected bool interactableMoreThanOnce = true; 
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
        }
    }
    protected virtual void TriggerInteraction()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && interactable)
            TriggerInteraction();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && interactable)
            DisableInteraction();
    }
}
