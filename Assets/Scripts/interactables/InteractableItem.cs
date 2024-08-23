
using UnityEngine;
using Yarn.Unity.Example;

public class InteractableItem : Interactable
{
    [SerializeField]
    string text;
    UIManager uIManager = null;
    private void Start()
    {

        if (string.IsNullOrWhiteSpace(text))
            Debug.LogError("InteractableItem with name " + gameObject.name + " has no text set.");
    }
    protected override void TriggerInteraction()
    {
        uIManager = ServiceLocator.Instance.Get<UIManager>();
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError("Text to present in interctable item view is empty");
            return;
        }
        uIManager.interactableView.Open(text);
    }
    protected override void DisableInteraction()
    {
        uIManager.interactableView.Close();
    }
}
