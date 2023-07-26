using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField]
    string transitionTo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag !="Player")
            return;
        if(string.IsNullOrEmpty(transitionTo))
        {
            Debug.LogError("SceneTransitionTrigger: Scene to transition to name is Null");
            return;
        }
        SceneManager.Instance.LoadScene(transitionTo);
    }
}
