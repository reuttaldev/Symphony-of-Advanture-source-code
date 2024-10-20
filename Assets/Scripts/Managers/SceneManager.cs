using System;
using System.Collections;
using UnityEngine;
public class SceneManager : SimpleSingleton<SceneManager> // the canvas needs to be shown during scene changes, therefore it cannot be a scene object and must be a don't destroy on load singleton
{
    bool loadingScene = false, fadingIn = false;
    float animTimeInSec = 1;
    [SerializeField]
    SceneTransitionPanel sceneTransitionPanel;
    private Animator animator;
    string previousSceneName;
    public event Action<string> OnSceneLoaded;
    public event Action OnFadeInFinish;

#if UNITY_EDITOR
    private void Start()
    {
        // so it triggers on scene load methods when we start the game from the editor, without switching scenes 
        if (!loadingScene)
        {
            OnSceneLoaded?.Invoke(null);
            OnFadeInFinish?.Invoke();
        }
    }
#endif
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        animator = sceneTransitionPanel.GetComponent<Animator>();
    }
    public void LoadScene(string sceneToLoadName)
    {
        if (string.IsNullOrEmpty(sceneToLoadName))
        {
            Debug.LogError("scene to load  name is empty", this);
            return;
        }
        if (loadingScene)
        {
            Debug.LogError("Scene is already loading");
            return;
        }
        if (fadingIn) // if asked to switch scene before the fade in animation had a chance to finish
            StopCoroutine(FadeIn());

        previousSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneWithAnimation(sceneToLoadName));
    }

    private IEnumerator LoadAndFadeOut(string sceneName)
    {
        loadingScene = true;
        var asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        //start animation
        animator.SetTrigger("FadeOut");
        // ensure showing black screen animation has finished, before we we allow to switch scene
        yield return new WaitForSeconds(animTimeInSec); 
        asyncLoad.allowSceneActivation = true;

        //  Unity triggers scene activation at asyncLoad.progress 90%.
        //  actual scene switch is at the end of this loop
        while (asyncLoad.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        // wait until the asynchronous scene fully loads
        // awkaes are called somewhere here 
        while (!asyncLoad.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        loadingScene = false;
        // awakes (of other classes) were called for sure
        OnSceneLoaded?.Invoke(previousSceneName);

    }
    private IEnumerator FadeIn()
    {
        fadingIn = true;
        // start the hide black screen animation 
        animator.SetTrigger("FadeIn");
        // wait until fade out animation has finished 
        yield return new WaitForSeconds(animTimeInSec);
        //invoke an event to let other scripts know that we are done with the load animation 
        fadingIn = false;
        OnFadeInFinish?.Invoke();
    }

    private IEnumerator LoadSceneWithAnimation(string sceneName)
    {
        yield return StartCoroutine(LoadAndFadeOut(sceneName));
        yield return StartCoroutine(FadeIn());
    }
}
