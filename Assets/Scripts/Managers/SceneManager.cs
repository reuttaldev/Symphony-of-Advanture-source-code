using System;
using System.Collections;
using UnityEngine;
public class SceneManager : SimpleSingleton<SceneManager> // the canvas needs to be shown during scene changes, therefore it cannot be a scene object and must be a don't destroy on load singleton
{
    bool changingScene = false;
    [SerializeField] 
    SceneTransitionPanel sceneTransitionPanel;
    private Animator animator;
    [SerializeField]
    GameObject canvas;
    string previousSceneName;
    public event Action<string> OnSceneLoaded;

    protected override void  Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        animator = sceneTransitionPanel.GetComponent<Animator>();
    }
#if UNITY_EDITOR
    private void Start()
    {
        // so it triggers on scene load methods when we start the game from the editor, without switching scenes 
        if(!changingScene) 
            OnSceneLoaded.Invoke(null);
    }
#endif
    public void LoadScene(string sceneToLoadName)
    {
        if (!string.IsNullOrEmpty(sceneToLoadName))
        {
            if (changingScene)
            {
                Debug.LogError("Scene is already loading");
                return;
            }
            previousSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            StartCoroutine(LoadSceneWithAnimation(sceneToLoadName));
        }
        else Debug.LogError("scene to load  name is empty", this);
    }
    public IEnumerator LoadSceneWithAnimation(string sceneName)
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
        // wait until fade in animation has finished 
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            yield return new WaitForEndOfFrame();
        // start fading in
        animator.SetTrigger("end");
        // let the fact that we triggered the end animation set in
        yield return new WaitForEndOfFrame();
        // actual scene switch is here
        asyncLoad.allowSceneActivation = true;
        yield return new WaitForEndOfFrame();
        // wait until fade out animation has finished 
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f)
        {
            yield return new WaitForEndOfFrame();
        }
        //invoke an event to let other scripts know that we are done with the load animation 
        OnSceneLoaded.Invoke(previousSceneName);
        yield return new WaitForEndOfFrame();

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
        {
            yield return new WaitForEndOfFrame();
        }
        canvas.SetActive(false);
        changingScene = false;
    }
}
