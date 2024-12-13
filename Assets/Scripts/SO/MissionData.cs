using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Mission Data", menuName = "Scriptable Objects/ Mission Data")]
[Serializable]
public class MissionData : MyScriptableObject
{
    [SerializeField]
    private string missionName;
    public string Name => missionName;
    [SerializeField]
    public string mainObjective; // used in a prompt to remind the players what they should do to solve this mission
    [SerializeField]
    private string startLocation;
    public MissionState state = MissionState.NotStarted;
    public MissionState State=> state;
    [SerializeField]
    private bool mandatory = true;
    [SerializeField]
    public bool Mandatory=>mandatory;
    //private List<string> prerequisiteIds; // list of ids that must be completed before this one can be started 

    public void StartMission()
    {
        var clone = this.GetRuntimeInstance<MissionData>(); 
        if (clone.State != MissionState.NotStarted)
            return;
        clone.state = MissionState.OnGoing;
        ServiceLocator.Instance.Get<MissionManager>().MissionHasStarted(GlobalID);
        if(mandatory && !String.IsNullOrWhiteSpace(mainObjective))
            MissionUIManager.Instance.ShowMissionUI(GlobalID, mainObjective);

    }
    public void EndMission(bool successful = true)
    {
        var clone = this.GetRuntimeInstance<MissionData>();
        if ( clone.state == MissionState.NotStarted)
        {
            Debug.LogError("Trying to complete a mission that has not been started. Mission name is: " + missionName);
            //return;
        }
        if (clone.state == MissionState.CompletedUnSuccessfully || state == MissionState.CompletedSuccessfully)
        {
            Debug.LogWarning("Trying to complete a mission that was already completed.");
            return;
        }
        clone.state = successful ? MissionState.CompletedSuccessfully : MissionState.CompletedUnSuccessfully;
        ServiceLocator.Instance.Get<MissionManager>().MissionHasEnded(GlobalID, successful);
        if (mandatory)
            MissionUIManager.Instance.HideMissionUI(GlobalID);
    }

}

[Serializable]
public enum MissionState
{
    CompletedSuccessfully,
    CompletedUnSuccessfully,
    OnGoing,
    NotStarted
}