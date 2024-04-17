using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    private bool followPlayer = true;
    [SerializeField]
    PlayerMovement companionMovment;
    [SerializeField]
    DialogueIntractable dialogueIntractable;
    public void FollowPlayer()
    {
        followPlayer = true;
        companionMovment.enabled = true;
    }
    public void StopFollowingPlayer()
    {
        followPlayer = false;
        companionMovment.enabled = false;
    }
}
