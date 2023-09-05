using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempExportSceneTrigger : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public bool interctable = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(interctable)
        {
            ExportManager exportManager = ServiceLocator.Instance.Get<ExportManager>();
            bool sucess = exportManager.SendCSVByEmail();
            interctable = false;
            if(sucess)
                spriteRenderer.color = Color.green;
            else
                spriteRenderer.color = Color.red;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        interctable = true;
        spriteRenderer.color = Color.white;
    }
}
