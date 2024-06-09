using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartSceneEventWrapper : MonoBehaviour
{
    [SerializeField]
    MissionData missionData;
    [Tooltip("If the associated mission is active, this will get triggered on scene start.")]
    public UnityEvent onSceneStart;
    public MissionState MissionState => missionData.State;
    public string MissionName => missionData.Name;
    private void Start()
    {
        if (missionData == null)
            Debug.LogError("forgot to set mission data on " + name);
    }
}
