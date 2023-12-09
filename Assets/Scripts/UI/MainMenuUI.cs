using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text errorText;

    void OpenFileExplorerRuntime()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Text Files", ".txt"));
        //FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.ShowLoadDialog(LoadConfigurationFile,null, FileBrowser.PickMode.Files);
    }
    void LoadConfigurationFile(string[] paths)
    {
        string errorMessage = ConfigurationFileManager.CheckSyntax(FileBrowserHelpers.ReadTextFromFile(paths[0]));
        if (errorMessage != null)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Configuration file is incorrect: "+errorMessage;
            return;
        }
        errorText.gameObject.SetActive(false);
        StartGame();
    }

    public void StartWithConfigurationFile()
    {
        OpenFileExplorerRuntime();
    }

    public void StartGame()
    {
        SceneManager.Instance.LoadScene("TownSquare");
    }
}
