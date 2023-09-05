using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempExportSceneTrigger : MonoBehaviour
{

    public bool interctable = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(interctable)
        {
            ServiceLocator.Instance.Get<GameManager>().GameOver();
            interctable = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        interctable = true;
    }
}
