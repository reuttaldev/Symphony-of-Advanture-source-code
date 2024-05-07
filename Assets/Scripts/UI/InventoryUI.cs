using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup mapIconParent;
    [SerializeField]
    Image[] mapIconObjects;
    [SerializeField]
    TMP_Text addItemText;
    [SerializeField]
    float fadeDuration = 0.3f, showTextDuration = 180, showMapDuration = 180;
    public void ShowMapIcons(int whichPieces)
    {
        for (int i = 0; i < whichPieces ; i++)
        {
            mapIconObjects[i].gameObject.SetActive(true);
            Debug.Log("setting active");
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
        yield return FadeIconsToFullAlpha();
        yield return new WaitForSeconds(showMapDuration);
        yield return FadeIconsZeroAlpha();
    }

    IEnumerator ShowText()
    {
        addItemText.gameObject.SetActive(true);
        yield return FadeTextToFullAlpha(fadeDuration,addItemText);
        yield return new WaitForSeconds(showTextDuration);
        yield return FadeTextToZeroAlpha(fadeDuration, addItemText);
        addItemText.gameObject.SetActive(false);
    }

    IEnumerator FadeIconsToFullAlpha()
    {   
        while (mapIconParent.alpha < 1.0f)
        {
            mapIconParent.alpha += (Time.deltaTime / fadeDuration);
            yield return null;
        }
    }

    IEnumerator FadeIconsZeroAlpha()
    {
        while (mapIconParent.alpha > 0.0f)
        {
            mapIconParent.alpha -= (Time.deltaTime / fadeDuration);
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
