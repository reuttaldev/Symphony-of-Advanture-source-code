using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    private MissionState state = MissionState.NotStarted;
    public MissionState State=> state;
    [SerializeField]
    private bool mandatory = true;
    public bool Mandatory=>mandatory;
    [SerializeField]
    private List<string> prerequisiteIds; // list of ids that must be completed before this one can be started 
    public List<string> GetPrerequsite() => prerequisiteIds;

#if UNITY_EDITOR
    override protected void OnEnable()
    {
        base.OnEnable();
        missionName = name;
    }
#endif
    public void StartMission()
    {
        state = MissionState.OnGoing;
        ServiceLocator.Instance.Get<MissionManager>().MissionHasStarted(GlobalID);
        Debug.Log("Started mission " + Name);
     }
    public void EndMission(bool successful = true)
    {
        if ( state != MissionState.NotStarted)
        {
            Debug.LogError("Trying to complete a mission that has not been started. Mission name is: " + missionName);
            //return;
        }
        if (state == MissionState.CompletedUnSuccessfully || state == MissionState.CompletedSuccessfully)
        {
            Debug.LogError("Trying to complete a mission that was already completed.");
            return;
        }
        state = successful ? MissionState.CompletedSuccessfully : MissionState.CompletedUnSuccessfully;
        ServiceLocator.Instance.Get<MissionManager>().MissionHasEnded(GlobalID);
        Debug.Log("Mission " + Name + " has been marked as completed");
    }

}

public enum MissionState
{
    CompletedSuccessfully,
    CompletedUnSuccessfully,
    OnGoing,
    NotStarted
}