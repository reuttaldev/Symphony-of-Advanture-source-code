using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    InventoryUI uiManager;
    void Awake()
    {
        ServiceLocator.Instance.Register<InventoryManager>(this);
    }
    public void ShowMapIcons(int pieceNumber)
    {
        uiManager.ShowMapIcons(pieceNumber);
    }

    public void AddTrackCollectable(TrackData cassetteData)
    {
        uiManager.AddTrackCollectableUI();
        ServiceLocator.Instance.Get<AudioManager>().AddToLibrary(cassetteData.ID);
    }
}
