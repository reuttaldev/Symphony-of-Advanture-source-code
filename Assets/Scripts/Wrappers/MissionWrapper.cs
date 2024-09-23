using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// This script is used as a wrapper to the MissionData scriptable object.
// Its purpose is to let us have a mission data instance in the hierarchy so that we can correlate each mision data
// To the events it should trigger when that mission is completed

public class MissionWrapper : MonoBehaviour
{
    [SerializeField]
    MissionData missionData;
    public UnityEvent onMissionStart;
    public UnityEvent onMissionEnd;
    // if the associated mission is completed when the scene starts, do the events that are connected this
    public UnityEvent missionDoneOnSceneStart;
    public string MissionDataID => missionData.GlobalID;
    public string Name => missionData.Name;

    public bool IsMissionDone()
    {
        return missionData.State == MissionState.CompletedUnSuccessfully || missionData.State == MissionState.CompletedSuccessfully;
    }
}
