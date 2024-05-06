using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup mapIconParent;
    Image[] mapIconObjects;
    [SerializeField]
    TMP_Text addItemText;
    [SerializeField]
    float fadeDuration = 0.3f, showTextDuration = 3, showMapDuration = 3;
    [SerializeField]
    MissionData[] mapMissionData;
    private void Start()
    {
        mapIconObjects = mapIconParent.GetComponentsInChildren<Image>();
    }
    public void ShowMapIcons()
    {
        for (int i = 0; i < mapIconObjects.Length; i++)
        {
            if (mapMissionData[i].State == MissionState.CompletedSuccessfully)
                mapIconObjects[i].gameObject.SetActive(true);
        }
        StartCoroutine(ShowIcons());
    }

    public void AddTrackCollectableUI()
    {
        addItemText.text = "New cassette obtained.";
        StartCoroutine(ShowText());
    }
    IEnumerator ShowIcons()
    {
        yield return FadeIconsToFullAlpha(fadeDuration, addItemText);
        yield return new WaitForSeconds(showTextDuration);
        yield return FadeIconsZeroAlpha(fadeDuration, addItemText);
    }

    IEnumerator ShowText()
    {
        addItemText.gameObject.SetActive(true);
        yield return FadeTextToFullAlpha(fadeDuration,addItemText);
        yield return new WaitForSeconds(showTextDuration);
        yield return FadeTextToZeroAlpha(fadeDuration, addItemText);
        addItemText.gameObject.SetActive(false);
    }

    IEnumerator FadeIconsToFullAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    IEnumerator FadeIconsZeroAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
    IEnumerator FadeTextToFullAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    IEnumerator FadeTextToZeroAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

}
