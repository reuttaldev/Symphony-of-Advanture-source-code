using System.Collections;
using UnityEngine;

public class EndSceneController : MonoBehaviour 
{
    [SerializeField] float delaySeconds = 6f;

    private void Start()
    {
        StartCoroutine(QuitAfterDelay());

    }
    public IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delaySeconds);
        Debug.Log("exiting");
        Application.Quit();

    }

}
