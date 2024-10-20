using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MissionManager : MonoBehaviour, IRegistrableService
{
    // key is misssion id, value is mission data (its weapper)
    // its only for mission we want to attach UnityEvents to 
    Dictionary<string,MissionWrapper> missionWrappers = new Dictionary<string, MissionWrapper>();
     void Awake()
    {
        ServiceLocator.Instance.Register<MissionManager>(this);
        LoadMissionWrappers();
    }
    void OnEnable()
    {
        SceneManager.Instance.OnSceneLoaded += HandleSceneLoad;
    }
    void OnDisable()
    {
        if (SceneManager.Instance != null)
            SceneManager.Instance.OnSceneLoaded -= HandleSceneLoad;
    }

    // init the all missions dictionary based on the references that were set in the editor
    private void LoadMissionWrappers()
    {
        var children = GetComponentsInChildren<MissionWrapper>();
        foreach (MissionWrapper wrapper in children)
        {
            if (missionWrappers.ContainsKey(wrapper.MissionDataID))
            {
                Debug.LogError("Duplicate mission wrapper with name " + wrapper.Name);
                return;
            }
            // trigger on scene start events, if the associated missions are active

            missionWrappers[wrapper.MissionDataID] = wrapper;
        }
    }
    public void HandleSceneLoad(string previousSceneName)
    {
        TriggerOnSceneStartEvents();
    }
    public void TriggerOnSceneStartEvents()
    {

        var children = GetComponentsInChildren<StartSceneEventWrapper>(); 
        foreach (StartSceneEventWrapper wrapper in children)
        {
            // only get components that are active
            if (wrapper.isActiveAndEnabled)
            {
                // trigger on scene start events, if the associated missions are active
                // these missions will often be completed through the dialouge yarn script
                // or one of the events we are triggering here will be to complete the mission
                if (wrapper.CurrentMissionState == wrapper.targetState)
                {
                    wrapper.onTargetState.Invoke();
                }
            }
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
        if (missionWrappers.ContainsKey(missionID))
        {
            missionWrappers[missionID].onMissionStart.Invoke();
        }
    }
    public void MissionHasEnded(string missionID, bool successful = true)
    {
        if (missionWrappers.ContainsKey(missionID))
        {
            if (successful)
                missionWrappers[missionID].onMissionEnd.Invoke();
            else
                missionWrappers[missionID].onMissionFail.Invoke();
        }
    }

}
