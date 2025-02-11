using UnityEngine;
using UnityEngine.Events;

public class Transition : MonoBehaviour
{
    public string dialogueNodeName;
    public bool atStart = true;
    public string sceneName;
    public string date;
    private DialogueManager dialogueManager;
    public UnityEvent betweenDefaultToBubble, betweenBubbleToDefault;

    public void Start()
    {
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();

        if (atStart)
        {
            //dialogueManager.SetDialogueView(true);
            dialogueManager.StartDialogue(dialogueNodeName);
        }
    }
    public void TransitionScene()
    {
        SceneManager.Instance.LoadScene(sceneName);
    }
}
