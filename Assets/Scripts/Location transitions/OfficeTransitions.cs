using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class OfficeTransitions : MonoBehaviour
{
    [YarnCommand("WalkToOffice")]

    public void WalkToOffice()
    {
        SceneManager.Instance.LoadScene("TownKeeperOffice");
        //dialogueManager.StartDialogue("");
    }
    [YarnCommand("LeaveOffice")]

    public void LeaveOffice()
    {
        SceneManager.Instance.LoadScene("TownSquare");
    }

}
