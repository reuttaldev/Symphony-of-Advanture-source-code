using UnityEngine;

public class Jan : MonoBehaviour
{
    [SerializeField]
    RectTransform pos;
    public void AfterWalk()
    {
        ServiceLocator.Instance.Get<DialogueManager>().StartDialogue("no_progress_meeting_bubble");
    }

    public void ChangeBubbleLocation()
    {
        gameObject.transform.position = pos.position;
        var children = gameObject.GetComponentsInChildren<RectTransform>(true);
        children[1].rotation = new Quaternion(0, 180,0,0);
        children[2].localRotation = new Quaternion(0, 180,0,0);
    }
}
