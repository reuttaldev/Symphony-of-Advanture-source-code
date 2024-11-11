using UnityEngine;
using UnityEngine.Events;

// if the associated mission is in the requested state when the scene starts, do the events that are specified 

public class StartSceneEventWrapper : MonoBehaviour
{
    [SerializeField]
    MissionData missionData;
    [Tooltip("If the associated mission is active, this will get triggered on scene start.")]
    public MissionState CurrentMissionState => missionData.State;
    public MissionState targetState = MissionState.OnGoing;
    public UnityEvent onTargetState;
    public string MissionName =>missionData.name;
    private void Start()
    {
        if (missionData == null)
            Debug.LogError("forgot to set mission data on " + name);
    }
}
