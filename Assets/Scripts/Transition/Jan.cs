using UnityEngine;

public class Jan : MonoBehaviour
{
    public void AfterWalk()
    {
        ServiceLocator.Instance.Get<DialogueManager>().StartDialogue("no_progress_meeting_bubble");
    }
}
