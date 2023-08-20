using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : SimpleSingleton<SceneManager>
{
    bool changingScene = false;
    [SerializeField]
    float fadingTime;
    const float defultFadingTime = 1;
    public GameObject loadingScreen;
    Image loadingImage;
    Color imageColor;
    float alpha;
    protected override void  Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        loadingImage = loadingScreen.GetComponent<Image>();
        imageColor = loadingImage.color;
        if (fadingTime == 0)
            fadingTime = defultFadingTime;
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
        changingScene = true;
        loadingScreen.SetActive(true);
        yield return StartCoroutine(FadeToBlackAnimation());
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        // actual scene switch is here
        asyncLoad.allowSceneActivation = true;
        yield return StartCoroutine(FadeFromBlackAnimation());
        changingScene = false;
    }
    internal IEnumerator FadeToBlackAnimation()
    {
        loadingScreen.SetActive(true);
        float startValue = 0;
        float targetValue = 1;
        if (loadingImage.color.a != startValue)
            loadingImage.color = new Color(imageColor.r, imageColor.b, imageColor.g, startValue);
        float time = 0;

        while (time < fadingTime)
        {
            alpha = Mathf.Lerp(startValue, targetValue, time / fadingTime);
            loadingImage.color = new Color(imageColor.r, imageColor.b, imageColor.g, alpha);
            time += Time.deltaTime;
            yield return null;
        }
    }
    internal IEnumerator FadeFromBlackAnimation()
    {
        float startValue = 1;
        float targetValue = 0;
        if (loadingImage.color.a != startValue)
            loadingImage.color = new Color(imageColor.r, imageColor.b, imageColor.g, startValue);
        float time = 0;

        while (time < fadingTime)
        {
            alpha = Mathf.Lerp(startValue, targetValue, time / fadingTime);
            loadingImage.color = new Color(imageColor.r, imageColor.b, imageColor.g, alpha);
            time += Time.deltaTime;
            yield return null;
        }
        loadingScreen.SetActive(false);
    }
}
