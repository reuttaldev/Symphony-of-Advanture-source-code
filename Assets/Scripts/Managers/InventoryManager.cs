using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class InventoryManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    InventoryUI inventoryUI;
    [SerializeField]
    MissionData[] mapMissionData; 
    void Awake()
    {
        ServiceLocator.Instance.Register<InventoryManager>(this);
    }
    public void ShowMapIcons()
    {
        inventoryUI.ShowMapIcons(mapMissionData.Select(map=> map.State == MissionState.CompletedSuccessfully).ToArray());
    }

    public void AddTrackCollectable(TrackData cassetteData)
    {
        inventoryUI.AddTrackCollectableUI();
        ServiceLocator.Instance.Get<AudioManager>().AddToLibrary(cassetteData.trackID);
    }
}
