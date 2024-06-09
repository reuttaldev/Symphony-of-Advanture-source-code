using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

public class EnterNameManager : MonoBehaviour
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    [SerializeField]
    DialogueManager dialogueManager;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    GameObject panel;
    [SerializeField]
    TMP_InputField inputField;
    [SerializeField]
    MissionData talkToAstridMission;
    [SerializeField]
    PlayerNameData playerNameData;

    [YarnCommand("EnterName")]
    public void EnterNamePanel()
    {
        panel.gameObject.SetActive(true);
        StartCoroutine(SelectInputField());
    }

    public void SetName()
    {
        string inputName = inputField.text;
        if ( string.IsNullOrEmpty(inputName))
        {
            return;
        }
        playerNameData.PlayerName = inputName;
        dialogueRunner.VariableStorage.SetValue("$playerName", inputName);
        panel.gameObject.SetActive(false);
        dialogueManager.SetMissionToComplete(talkToAstridMission);
        dialogueManager.StartDialogue("companion_intro_after_name");
        this.gameObject.SetActive(false);
    }
    private IEnumerator SelectInputField()
    {
        // Wait for end of frame to ensure UI elements are initialized
        yield return new WaitForEndOfFrame();
        inputField.ActivateInputField();
    }
}
