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

    public void Awake()
    {
        for (int i = 0; i < activeOnlyDuring.Length; i++)
        {
            activeOnlyDuring[i] = activeOnlyDuring[i].GetRuntimeInstance<MissionData>();
        }
        for (int i = 0; i < activeOnlyAfterSuccess.Length; i++)
        {
            activeOnlyAfterSuccess[i] = activeOnlyAfterSuccess[i].GetRuntimeInstance<MissionData>();
        }
        for (int i = 0; i < inactiveDuring.Length; i++)
        {
            inactiveDuring[i] = inactiveDuring[i].GetRuntimeInstance<MissionData>();
        }
    }
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
        if (a.Length == 0)
            return true; 
        // if one of them is active, or the list is empty, allow it 
        bool allow = a.Length == 0;
        foreach (var mission in a)
        {
            if (mission.State == state)
            {
                if(!andCondition)
                    return true;
            }
            else if (andCondition)
            {
                return false;
            }
        }
        // if we got here, and the condition is or it means that *no* state is the target one. return false
        if (!andCondition)
            return false;
        // if we got here, and the condition is and it means that all states are the target ones
        return true;
    }
}
