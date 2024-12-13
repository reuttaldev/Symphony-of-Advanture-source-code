using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// it is a singleton since the mission that appear on screen are presistent between scenes
public class MissionUIManager : SimpleSingleton<MissionUIManager>
{
    [SerializeField]
    GameObject items,missionUIPrefab;
    Dictionary<string, MissionUI> mandatoryActiveMissions = new Dictionary<string, MissionUI>();
    public void ShowMissionUI(string id, string description)
    {
        if (mandatoryActiveMissions.ContainsKey(id))
            return;
        var GO = Instantiate(missionUIPrefab,items.transform);
        mandatoryActiveMissions[id] = GO.GetComponent<MissionUI>();
        mandatoryActiveMissions[id].descriptionText.text = description;
    }
    public void HideMissionUI(string id)
    {
        if (!mandatoryActiveMissions.ContainsKey(id))
        {
            //Debug.LogError("Could not find mission UI to hide");
            return;
        }
        Destroy(mandatoryActiveMissions[id].gameObject);   
    }
}
