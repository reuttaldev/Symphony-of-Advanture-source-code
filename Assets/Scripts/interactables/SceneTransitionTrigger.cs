using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransitionTrigger : Interactable
{
    [SerializeField]
    string transitionTo;
    [SerializeField]
    MissionData[] activeOnlyDuring;
    [SerializeField]
    MissionData[] inactiveDuring;
    [SerializeField]
    MissionData[] activeWhenCompleted;
    public string GoesTo => transitionTo;


    protected override void TriggerInteraction()
    {
        foreach (var mission in inactiveDuring)
        {
            if (mission.State == MissionState.OnGoing)
                return;
        }
        bool allow = activeOnlyDuring.Length == 0;
        // if one of them is active, or the list is emptyallow it 
        foreach (var mission in activeOnlyDuring)
        {
            if (mission.State == MissionState.OnGoing)
            {
                allow= true;
                continue;
            }
        }
        if (!allow)
            return;
        if(string.IsNullOrEmpty(transitionTo))
        {
            Debug.LogError("SceneTransitionTrigger: Scene to transition to name is Null");
            return;
        }
        SceneManager.Instance.LoadScene(transitionTo);
    }
}
