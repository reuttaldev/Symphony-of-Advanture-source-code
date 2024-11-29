using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class OfficeTransition : MonoBehaviour
{

    [SerializeField]
    Transform[] wayToOffice;
    [SerializeField] 
    PlayerMovement playerMovement;
    [SerializeField]
    NPCFollowPlayer companionMovement; 
    
    public void WalkToOffice()
    {
        Debug.Log("walking to office");
        SceneManager.Instance.LoadScene("Office");
    }
}
