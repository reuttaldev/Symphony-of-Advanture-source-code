using System.Collections;
using TMPro;
using UnityEngine;
using Yarn.Unity;

public class EnterNameManager : MonoBehaviour
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    [SerializeField]
    DialogueManager dialogueManager;
    [SerializeField]
    GameObject panel;
    [SerializeField]
    TMP_InputField inputField;
    [SerializeField]
    MissionData talkToAstridMission;
    [SerializeField]
    PlayerNameData playerNameData;

    private void Awake()
    {
        panel.SetActive(false);
    }
    [YarnCommand("EnterName")]
    public void EnterNamePanel()
    {
        panel.SetActive(true);
        StartCoroutine(SelectInputField());
    }

    public void SetName()
    {
        string inputName = inputField.text;
        if ( string.IsNullOrWhiteSpace(inputName))
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
