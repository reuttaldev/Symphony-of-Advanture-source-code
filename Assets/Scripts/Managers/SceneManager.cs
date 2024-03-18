using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class SceneManager : SimpleSingleton<SceneManager> // the canvas needs to be shown during scene changes, therefore it cannot be a scene object and must be a don't destroy on load singleton
{
    bool changingScene = false;
    float animationDuration = 1f;
    [SerializeField] 
    SceneTransitionPanel sceneTransitionPanel;
    private Animator animator;
    [SerializeField]
    GameObject canvas; 

    protected override void  Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        animator = sceneTransitionPanel.GetComponent<Animator>();
    }
    public void LoadScene(string sceneToLoadName)
    {
        if (!string.IsNullOrEmpty(sceneToLoadName))
        {
            if (changingScene)
            {
                Debug.LogError("Scene is already loading");
                return;
            }
            StartCoroutine(LoadSceneWithAnimation(sceneToLoadName));
        }
        else Debug.LogError("scene to load  name is empty", this);
    }
    private IEnumerator LoadSceneWithAnimation(string sceneName)
    {
        canvas.SetActive(true);
        changingScene = true;
        animator.SetTrigger("start");
        // start loading the scene, while fade out animation is playing
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        // scene has finished loading (we are currently showing black screen. make sure animation finishes playing
        yield return new WaitForSeconds(animationDuration);
        animator.SetTrigger("end");
        asyncLoad.allowSceneActivation = true;
        // load the new scene while screen is still black
        // start fading in
        yield return new WaitForSeconds(animationDuration);
        canvas.SetActive(false);
        // actual scene switch is here
        changingScene = false;
    }
}
