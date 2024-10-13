using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCollectable : Interactable
{
    [SerializeField]
    MissionData mapMissionData;
    protected override void TriggerInteraction()
    {
        mapMissionData.EndMission();    
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
