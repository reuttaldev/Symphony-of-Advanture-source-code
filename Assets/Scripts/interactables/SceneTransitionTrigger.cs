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
    MissionData[] activeOnlyAfterSuccess;
    [SerializeField]
    MissionData[] inactiveDuring;
    [SerializeField]
    bool andCondition = false; // specifying if one of the activeOnlyDuring condition is enough to open the door, or if all of them must be satisfied  
    public string GoesTo => transitionTo;


    protected override void TriggerInteraction()
    {
        foreach (var mission in inactiveDuring)
        {
            if (mission.State == MissionState.OnGoing)
            {
                Debug.LogWarning("Not switching scenes bc "+mission.Name +" is active");
                return;
            }
        }
        var result1 = CheckIfSatisfied(activeOnlyDuring, MissionState.OnGoing);
        var result2 = CheckIfSatisfied(activeOnlyAfterSuccess, MissionState.CompletedSuccessfully);
        if(!(result1 && result2))
        {
            Debug.LogWarning("Conditions for opening trigger door are not satisfied.");
            return;
        }
        if(string.IsNullOrEmpty(transitionTo))
        {
            Debug.LogError("SceneTransitionTrigger: Scene to transition to name is Null");
            return;
        }
        SceneManager.Instance.LoadScene(transitionTo);
    }

    private bool CheckIfSatisfied(MissionData[] a,  MissionState state)
    {
        // if one of them is active, or the list is empty, allow it 
        bool allow = a.Length == 0;
        foreach (var mission in a)
        {
            if (mission.State == state && !andCondition)
            {
                return true;
            }
            else if (andCondition)
            {
                return false;
            }
        }
        if (!andCondition && a.Length != 0)
            return false;
        return true;
    }
}
