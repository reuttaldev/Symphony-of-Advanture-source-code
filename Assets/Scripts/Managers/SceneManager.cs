using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
public class SceneManager : SimpleSingleton<SceneManager> // the canvas needs to be shown during scene changes, therefore it cannot be a scene object and must be a don't destroy on load singleton
{
    bool loadingScene = false, fadingIn = false;
    float showTextTime=2;
    [SerializeField]
    SceneTransitionPanel sceneTransitionPanel;
    [SerializeField]
    TMP_Text dateText;
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
    public void LoadScene(string sceneToLoadName,string date)
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

        previousSceneName = GetActiveScene();
        StartCoroutine(LoadSceneWithAnimationAndDate(sceneToLoadName,date));
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

        previousSceneName = GetActiveScene();
        StartCoroutine(LoadSceneWithAnimation(sceneToLoadName));
    }


    private IEnumerator LoadAndFadeOut(string sceneName)
    {
        loadingScene = true;
        var asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        animator.SetTrigger("FadeOut");

        // wait until the asynchronous scene fully loads, and anim is done
        while (asyncLoad.progress < 0.9f || sceneTransitionPanel.fillValue > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        // awkaes are called somewhere here 
        asyncLoad.allowSceneActivation = true;
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
        while (sceneTransitionPanel.fillValue < 1)
        {
            yield return new WaitForEndOfFrame();
        }        //invoke an event to let other scripts know that we are done with the load animation 
        fadingIn = false;
        OnFadeInFinish?.Invoke();
    }

    private IEnumerator LoadSceneWithAnimation(string sceneName)
    {
        yield return StartCoroutine(LoadAndFadeOut(sceneName));
        yield return StartCoroutine(FadeIn());
    }
    private IEnumerator LoadSceneWithAnimationAndDate(string sceneName,string date)
    {
        yield return StartCoroutine(LoadAndFadeOut(sceneName));
        dateText.text = date;
        Debug.Log("waiting for date");
        yield return new WaitForSecondsRealtime(showTextTime);
        dateText.text = "";
        yield return StartCoroutine(FadeIn());
    }

    internal static string GetActiveScene()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }
}
