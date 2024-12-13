using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class InventoryUIManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    CanvasGroup mapIconParent;
    [SerializeField]
    Image[] mapIconObjects;
    [SerializeField]
    CanvasGroup addItemTextGroup;
    [SerializeField]
    TMP_Text addItemText;
    float showTextDuration = 5, showMapDuration = 8;

    private void Awake()
    {
        ServiceLocator.Instance.Register<InventoryUIManager>(this); 
        addItemText = addItemTextGroup.gameObject.GetComponent<TMP_Text>();
    }
    public void ShowMapIcons(bool[] showPieceAtIndex)
    {
        Debug.Log("Showing map icons");
        for (int i = 0; i < showPieceAtIndex.Length; i++)
        {
            mapIconObjects[i].gameObject.SetActive(showPieceAtIndex[i]);
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
        yield return StartCoroutine(Effects.FadeAlpha(mapIconParent, 0, 1, showMapDuration/2));
        yield return StartCoroutine(Effects.FadeAlpha(mapIconParent, 1, 0, showMapDuration/2));
    }

    IEnumerator ShowText()
    {
        addItemTextGroup.gameObject.SetActive(true);
        yield return StartCoroutine(Effects.FadeAlpha(addItemTextGroup, 0, 1, showTextDuration/2));
        yield return StartCoroutine(Effects.FadeAlpha(addItemTextGroup, 1, 0, showTextDuration/2));
        addItemTextGroup.gameObject.SetActive(false);
    }
}
