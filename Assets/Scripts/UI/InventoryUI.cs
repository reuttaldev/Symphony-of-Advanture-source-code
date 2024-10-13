using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup mapIconParent;
    [SerializeField]
    Image[] mapIconObjects;
    [SerializeField]
    CanvasGroup addItemTextGroup;
    [SerializeField]
    TMP_Text addItemText;
    float fadeDuration = 1.5f, showTextDuration = 70, showMapDuration = 100;

    private void Awake()
    {
        addItemText = addItemTextGroup.gameObject.GetComponent<TMP_Text>();
    }
    public void ShowMapIcons(int whichPieces)
    {
        Debug.Log("Showing map icons");
        for (int i = 0; i < whichPieces ; i++)
        {
            mapIconObjects[i].gameObject.SetActive(true);
            Debug.Log("setting active");
        }
        StartCoroutine(ShowIcons());
    }

    public void AddTrackCollectableUI()
    {
        addItemText .text = "New cassette obtained.";
        StartCoroutine(ShowText());
    }
    IEnumerator ShowIcons()
    {
        yield return StartCoroutine(Effects.FadeAlpha(mapIconParent, 1, 0, fadeDuration));
        yield return new WaitForSeconds(showMapDuration);
        yield return StartCoroutine(Effects.FadeAlpha(mapIconParent, 0, 1, fadeDuration));
    }

    IEnumerator ShowText()
    {
        addItemTextGroup.gameObject.SetActive(true);
        yield return StartCoroutine(Effects.FadeAlpha(addItemTextGroup, 0, 1, fadeDuration));
        yield return new WaitForSeconds(showTextDuration);
        yield return StartCoroutine(Effects.FadeAlpha(addItemTextGroup, 0, 1, fadeDuration));
        addItemTextGroup.gameObject.SetActive(false);
    }
}
