using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    DialogueManager dialougeManager;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    GameObject tutorialPanel;
    [SerializeField]
    InputActionReference confirmAction;
    [SerializeField]
    MusicDialogueData tutorialData;
    bool reachedPart = false;
    [YarnCommand("tutorial")]
    public void startTutorial()
    {
        tutorialPanel.gameObject.SetActive(true);
        reachedPart = true;
    }

    private void Update()
    {
        if (!reachedPart)
            return;
        if (confirmAction.action.WasPressedThisFrame())
        {
            dialougeManager.currentMusicInteraction = tutorialData;
            tutorialPanel.gameObject.SetActive(false);
            uiManager.OpenMusicDialogueUI();
            this.gameObject.SetActive(false);
        }
    }
}
