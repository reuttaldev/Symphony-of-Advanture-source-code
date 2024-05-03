using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class MissionManager : SimpleSingleton<MissionManager>, IRegistrableService
{
    // key is misssion id, value is mission data (its weapper)
    // its only for mission we want to attach UnityEvents to 
    Dictionary<string,MissionWrapper> missionWrappers = new Dictionary<string, MissionWrapper>();

    protected override void Awake()
    {
        base.Awake();
        // presistent just so we don't need to load and reload the missions into a dictionary each time we switch scene by creating a new mission manager
        DontDestroyOnLoad(this);
        ServiceLocator.Instance.Register<MissionManager>(this);
    }

    void Start()
    {
        // init the all missions dictionary based on the references that were set in the editor
        var children = GetComponentsInChildren<MissionWrapper>();
        foreach (MissionWrapper item in children)
        {
            if(missionWrappers.ContainsKey(item.ID))
            {
                Debug.LogError("Duplicate mission wrapper with name " + item.Name);
                return;
            }    
            missionWrappers[item.ID] = item;
        }
    }

    // MissionData scriptable object will call this once a mission has been started
    // dialogue manger or mission interactables have reference to missionData and those call complete mission on mission data and then it will call this
    public void MissionHasStarted(string missionID)
    {
        if (missionWrappers.ContainsKey(missionID))
        {
            missionWrappers[missionID].onMissionStart.Invoke();
            Debug.Log("Envoking onMissionStart events for " + missionWrappers[missionID].Name);
        }
    }
    public void MissionHasEnded(string missionID)
    {
        if (missionWrappers.ContainsKey(missionID))
        {
            missionWrappers[missionID].onMissionEnd.Invoke();
            Debug.Log("Envoking onMissionEnd events for " + missionWrappers[missionID].Name);
        }
    }


}
