using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using Yarn.Unity.Example;

public class InteractableItem : Interactable
{
    [SerializeField]
    string text;
    private void Start()
    {

        if (string.IsNullOrWhiteSpace(text))
            Debug.LogError("InteractableItem with name " + gameObject.name + " has no text set.");
    }
    protected override void TriggerInteraction()
    {
        UIManager uIManager = ServiceLocator.Instance.Get<UIManager>();
        uIManager.OpenItemInteractableView(text);

    }
}
