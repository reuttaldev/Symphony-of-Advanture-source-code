using UnityEngine;

public class EndTransition : MonoBehaviour
{
    
    public void GoToEndScene()
    {
        SceneManager.Instance.LoadScene("EndMenu");
    }
}
