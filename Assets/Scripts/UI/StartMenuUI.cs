using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.UIElements;
using System;

public class StartMenuUI : MonoBehaviour
{

    private void Start()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Text Files", ".txt"));       
    }

    public void StartGame()
    {
        SceneManager.Instance.LoadScene("Street");
    }
}
