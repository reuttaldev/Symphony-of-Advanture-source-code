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
        //gameManager = ServiceLocator.Instance.Get<GameManager>();
        //StartCoroutine(WalkToOfficeCoroutine());
        SceneManager.Instance.LoadScene("TownKeeperOffice");
    }
    /*
    static IEnumerator WalkToOfficeCoroutine()
    {
        foreach (var item in wayToOffice)
        {
            yield return gameManager.WalkPlayer(item);

        }
    }*/


}
