using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        switch (missionEventType)
        {
            case MissionEventType.Start:
                missionData.StartMission();
                break;
            case MissionEventType.StartAndComplete:
                missionData.StartMission();
                missionData.EndMission();
                break;
            case MissionEventType.Complete:
                missionData.EndMission();
                break;
        }
    }
}

enum MissionEventType
{
    Start, Complete, StartAndComplete
}
