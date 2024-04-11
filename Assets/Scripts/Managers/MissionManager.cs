using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : SimpleSingleton<MissionManager>, IRegistrableService
{
    [SerializeField]
    private List<MissionData> allMissionsReferences;

    // key is misssion id, value is mission data
    Dictionary<string,MissionData> allMissions;

    protected override void Awake()
    {
        base.Awake();
        // presistent just so we don't need to load and reload the missions into a dictionary each time we switch scene by creating a new mission manager
        DontDestroyOnLoad(this);
        ServiceLocator.Instance.Register<MissionManager>(this);

        // init the all missions dictionary based on the references that were set in the editor
        foreach (var mission in allMissionsReferences)
        {
            allMissions[mission.GetMissionId()] = mission;
        }
    }

    public void StartMission(string missionID)
    {
        if(!allMissions.ContainsKey(missionID)) 
        {
            Debug.Log("Could not find mission data with id " + missionID);
            return;
        }
        MissionData data = allMissions[missionID];
        if (data.completed)
        {
            Debug.LogError("Trying to start a mission that was already completed.");
            return;
        }
        if (data.active)
        {
            Debug.LogError("Trying to start a mission that was already started.");
            return;
        }
        foreach(var req in data.GetPrerequsite())
        {
            if (!allMissions.ContainsKey(req))
            {
                Debug.Log("Could not find prerequsite mission data with id " + missionID);
                return;
            }
            if (!allMissions[req].completed)
            {
                Debug.Log("Prerequsite " + req + " was not satisfied for mission " + missionID + ", and therefore it was not started.");
            }
        }
        data.active = true;
    }

    public void CompleteMission(string missionID) // successfully 
    {
        if (!allMissions.ContainsKey(missionID))
        {
            Debug.Log("Could not find mission data with id " + missionID);
            return;
        }
        MissionData data = allMissions[missionID];
        if (data.completed)
        {
            Debug.LogError("Trying to complete a mission that was already completed.");
            return;
        }
        if (!data.active)
        {
            Debug.LogError("Trying to complete a mission that was never started.");
            return;
        }
        data.completed = true;
        data.active = false;

    }
    public void StartMission(MissionData mission)
    {
        StartMission(mission.GetMissionId());
    }
    public void CompleteMission(MissionData mission)
    {
        CompleteMission(mission.GetMissionId());
    }

}
