using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class MusicDialogueInteractable : MonoBehaviour
{
    [SerializeField]
    string interactionId;
    [SerializeField]
    Emotions emotionToEnvoke;
}
