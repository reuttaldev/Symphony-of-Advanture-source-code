using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class InventoryManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    MissionData[] mapMissionData; 
    void Awake()
    {
        ServiceLocator.Instance.Register<InventoryManager>(this);

    }
    public void ShowMapIcons()
    {
        for (int i = 0; i < mapMissionData.Length; i++)
        {
            mapMissionData[i] = mapMissionData[i].GetRuntimeInstance<MissionData>();
        }
        ServiceLocator.Instance.Get<InventoryUIManager>().ShowMapIcons(mapMissionData.Select(map=> map.State == MissionState.CompletedSuccessfully).ToArray());
    }

    public void AddTrackCollectable(TrackData cassetteData)
    {
        ServiceLocator.Instance.Get<InventoryUIManager>().AddTrackCollectableUI();
        ServiceLocator.Instance.Get<AudioManager>().AddToLibrary(cassetteData.trackID);
    }
}
