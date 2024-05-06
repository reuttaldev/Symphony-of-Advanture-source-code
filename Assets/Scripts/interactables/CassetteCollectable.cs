using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteCollectable : Interactable
{
    [SerializeField]
    TrackData data;

    protected override void TriggerInteraction()
    {
        ServiceLocator.Instance.Get<InventoryManager>().AddTrackCollectable(data);
        gameObject.SetActive(false);
    }
}
