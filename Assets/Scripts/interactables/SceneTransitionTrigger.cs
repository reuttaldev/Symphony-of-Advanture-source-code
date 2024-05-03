using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransitionTrigger : Interactable
{
    [SerializeField]
    string transitionTo;

    protected override void TriggerInteraction()
    {
        if(string.IsNullOrEmpty(transitionTo))
        {
            Debug.LogError("SceneTransitionTrigger: Scene to transition to name is Null");
            return;
        }
        SceneManager.Instance.LoadScene(transitionTo);
    }
}
