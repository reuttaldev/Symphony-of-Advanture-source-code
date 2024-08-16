using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class ItemInteractableView : MonoBehaviour
{
    [SerializeField]
    internal CanvasGroup canvasGroup;
    [SerializeField]
    InputActionReference continueAction;
    [SerializeField]
    internal TextMeshProUGUI lineText;
    [SerializeField]
    [Min(0)]
    internal float fadeInTime = 0.25f;
    [SerializeField]
    [Min(0)]
    internal float fadeOutTime = 0.05f;

    private bool open = false;
    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    private void OnEnable()
    {
        continueAction.action.performed += context => Close();
    }
    private void OnDisable()
    {
        continueAction.action.performed -= context => Close();
    }

    public void Open(string textToShow)
    {
        lineText.text = textToShow;
        StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeInTime));
        open = true;
    }
    public void Close()
    {
        if (open)
        {
            Debug.Log("closing item interactable view");
            StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, fadeOutTime));
            UIManager uIManager = ServiceLocator.Instance.Get<UIManager>();
            uIManager.CloseDialogueUI();
            open = false;
        }
    }
}
