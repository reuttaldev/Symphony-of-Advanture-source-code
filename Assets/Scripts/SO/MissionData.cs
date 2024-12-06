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
    private string mainObjective;
    [SerializeField]
    private string startLocation;
    public MissionState state = MissionState.NotStarted;
    public MissionState State=> state;
    [SerializeField]
    private bool mandatory = true;
    public bool Mandatory=>mandatory;
    [SerializeField]
    private List<string> prerequisiteIds; // list of ids that must be completed before this one can be started 
    public List<string> GetPrerequsite() => prerequisiteIds;

    public void StartMission()
    {
        var clone = this.GetRuntimeInstance<MissionData>(); 
        clone.state = MissionState.OnGoing;
        ServiceLocator.Instance.Get<MissionManager>().MissionHasStarted(GlobalID);
        //Debug.LogError("Mission " + clone.name + " has been started");

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
        Debug.Log("Mission " + clone.name + " has been marked as completed");
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