using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Mission Data", menuName = "Scriptable Objects/ Mission Data")]
[Serializable]
public class MissionData : MyScriptableObject
{
    [SerializeField]
    string missionName;
    [SerializeField]
    string missionID; // case sensetive, be carful when using in the editor 
    [SerializeField]
    string mainObjective;
    [SerializeField]
    string missionDescription;
    [SerializeField]
    public bool active = false;
    [SerializeField]
    public bool completed = false;
    [SerializeField]
    bool mandatory = true;
    [SerializeField]
    List<string> prerequisiteIds; // list of ids that must be completed before this one can be started 

    public string GetMissionId() => missionID;
    public List<string> GetPrerequsite() => prerequisiteIds;

}
