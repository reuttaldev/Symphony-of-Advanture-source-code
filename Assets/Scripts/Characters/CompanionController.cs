using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    NPCFollowPlayer movementManager;
    [SerializeField]
    bool followPlayerAtStart;
    private void Awake()
    {
        movementManager= gameObject.GetComponent<NPCFollowPlayer>();
    }
    private void Start()
    {
        if (!followPlayerAtStart)
            movementManager.StopFollowingPlayer();
        else
            movementManager.FollowPlayer();

    }
}
