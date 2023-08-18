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
        dialogueRunner.onDialogueStart.AddListener(()=>ServiceLocator.Instance.Get<InputManager>().ActivateUIMap());
        dialogueRunner.onDialogueComplete.AddListener(()=>ServiceLocator.Instance.Get<InputManager>().ActivatePlayerMap());
    }


}
