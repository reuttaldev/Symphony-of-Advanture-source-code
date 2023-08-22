using Yarn.Unity;
using UnityEngine;
using UnityEngine.Events;

    // In this script I will connect to yarn all of the commands it needs to have access to
public class DialogueManager : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    void Awake()
    {
        dialogueRunner = gameObject.GetComponent<DialogueRunner>();
    }
    private void Start()
    {
        dialogueRunner.AddCommandHandler("ExitGame", ServiceLocator.Instance.Get<GameManager>().ExitGame);
        dialogueRunner.AddCommandHandler("SMD", ServiceLocator.Instance.Get<UIManager>().OpenMusicDialogueUI);
        dialogueRunner.AddCommandHandler("EMD", ServiceLocator.Instance.Get<UIManager>().CloseMusicDialogueUI);
    }


}
