using UnityEngine;
[CreateAssetMenu(fileName = "Dialogue Node Data", menuName = "Scriptable Objects/ Dialogue Node Data")]
// mission wrappers set dialouge interactbles node to be something different when missions are completed
// but those will be resetted once you leave and re enter the scene
// need to save them in scriptable objects 

// basically I store this in a scriptable object and not in mono because I need this data to be presistent between scenes 
public class DialogueNodeData : MyScriptableObject
{
    public string nodeTitle;
    public MissionData associatedMission;
    public bool interactableMoreThanOnce = false;

}
