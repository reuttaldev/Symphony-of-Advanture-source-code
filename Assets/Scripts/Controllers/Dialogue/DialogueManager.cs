using Yarn.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    void Awake()
    {
        dialogueRunner = gameObject.GetComponent<DialogueRunner>();
    }
    private void Start()
    {
        dialogueRunner.onDialogueStart.AddListener(()=>InputManager.Instance.ActivateDialogueMap());
        dialogueRunner.onDialogueComplete.AddListener(()=>InputManager.Instance.ActivatePlayerMap());
    }


}
