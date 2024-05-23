using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour, IRegistrableService
{
    // key is misssion id, value is mission data (its weapper)
    // its only for mission we want to attach UnityEvents to 
    Dictionary<string,MissionWrapper> missionWrappers = new Dictionary<string, MissionWrapper>();

     void Awake()
    {
        ServiceLocator.Instance.Register<MissionManager>(this);
    }

    void Start()
    {
        // init the all missions dictionary based on the references that were set in the editor
        var children = GetComponentsInChildren<MissionWrapper>();
        foreach (MissionWrapper wrapper in children)
        {
            if(missionWrappers.ContainsKey(wrapper.MissionDataID))
            {
                Debug.LogError("Duplicate mission wrapper with name " + wrapper.Name);
                return;
            }    
            missionWrappers[wrapper.MissionDataID] = wrapper;
        }
    }

    void PrintAllMission()
    {
        foreach (var pair in missionWrappers)
        {
            Debug.Log("Key: " + pair.Key + ", Value: " + pair.Value);
        }
    }
    // MissionData scriptable object will call this once a mission has been started
    // dialogue manger or mission interactables have reference to missionData and those call complete mission on mission data and then it will call this
    public void MissionHasStarted(string missionID)
    {
        Debug.LogError(missionID);
        if (missionWrappers.ContainsKey(missionID))
        {
            Debug.Log("Envoking onMissionStart events for " + missionWrappers[missionID].Name);
            missionWrappers[missionID].onMissionStart.Invoke();
        }
    }
    public void MissionHasEnded(string missionID)
    {
        if (missionWrappers.ContainsKey(missionID))
        {
            Debug.Log("Envoking onMissionEnd events for " + missionWrappers[missionID].Name);
            missionWrappers[missionID].onMissionEnd.Invoke();
        }
    }

}
