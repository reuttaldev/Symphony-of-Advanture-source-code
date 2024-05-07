using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    CompanionMovement movementManager;
    [SerializeField]
    bool followPlayerAtStart;
    private void Awake()
    {
        movementManager= gameObject.GetComponent<CompanionMovement>();
    }
    private void Start()
    {
        if (!followPlayerAtStart)
            movementManager.StopFollowingPlayer();
        else
            movementManager.FollowPlayer();

    }
}
