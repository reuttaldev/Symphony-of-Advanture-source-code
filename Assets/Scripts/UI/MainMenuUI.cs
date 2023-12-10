using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.UIElements;
using System;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] ConfigurationFileManager configManager;

    private void Start()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Text Files", ".txt"));       
    }
    void OpenFileExplorerRuntime()
    {
        FileBrowser.ShowLoadDialog(OpenConfigurationFile, null, FileBrowser.PickMode.Files);
    }
    void OpenConfigurationFile(string[] paths)
    {
        configManager.LoadConfigurationFile(FileBrowserHelpers.ReadTextFromFile(paths[0]));
    }

    public void StartWithConfigurationFile()
    {
        OpenFileExplorerRuntime();
        // ConfigurationFileManager will call start game if the configuration file was loaded successfully
    }

    public void StartGame()
    {
        SceneManager.Instance.LoadScene("TownSquare");
    }
}
