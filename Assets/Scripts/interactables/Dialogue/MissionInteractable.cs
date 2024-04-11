using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionInteractable : Interactable
{
    [SerializeField]
    MissionData missionData;
    [SerializeField]
    MissionEventType missionEventType;
    MissionManager missionManager;
    private void Start()
    {
        missionManager = ServiceLocator.Instance.Get<MissionManager>();
    }
    protected override void TriggerInteraction()
    {
        if(missionEventType == MissionEventType.Start) 
            missionManager.StartMission(missionData);
        else if (missionEventType == MissionEventType.Complete)
            missionManager.CompleteMission(missionData);
        else
            missionManager.StartMission(missionData);
            missionManager.CompleteMission(missionData);
    }
}

enum MissionEventType
{
    Start, Complete, StartandComplete
}
