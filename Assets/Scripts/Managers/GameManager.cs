using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn;
using Yarn.Unity;

public class GameManager : MonoBehaviour, IRegistrableService
{
    public GameSettings settings;
    public DataMigrationSettings dataMigrationSettings;
    DialogueManager dialogueManager;
    public static bool paused = false;
    string playerName;
    [SerializeField]
    List<ReturnPoints> returnPoints;
    [SerializeField] 
    GameObject player, companion;
    Vector2 companionOffset = new Vector2(5,5);

    private void Awake()
    {     
        ServiceLocator.Instance.Register<GameManager>(this);
        if (settings == null)
            Debug.LogError("Game manager is missing a reference to game settings");
        if(dataMigrationSettings == null)
            Debug.LogError("Game manager is missing a reference to data migration settings");
        if (returnPoints.Count == 0)
            Debug.LogWarning("Forgot to drag in scene exits to game manager");


    }

    void Start()
    {
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
    }
    public void SetPlayerName(string playerName)
    {
        // set player name in yarn
        var nameVariableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();
        nameVariableStorage.SetValue("$playerName", playerName);

    }

    public void UnpauseGame()
    {
        if (paused)
        {
            Debug.Log("Game Unpaused");
            Time.timeScale = 1;
            ServiceLocator.Instance.Get<InputManager>().ActionMapGoBack();
            paused = false;
        }
    }
    public void PauseGame()
    {
        if (!paused)
        {
            Debug.Log("Game paused");
            Time.timeScale = 0;
            ServiceLocator.Instance.Get<InputManager>().ActivatePausedUIMap();
            paused = true;
        }
    }
    public void ExitGame()
    {
        GameOver();
        Application.Quit();
        Debug.Log("exiting game");

    }

    public void GameOver()
    {
        ExportManager exportManager = ServiceLocator.Instance.Get<ExportManager>();
        exportManager.SendCSVByEmail();
    }

    public void PlacePlayerInScene(string previousSceneName)
    {
        Vector2 pos = new Vector2(0, 0);
        bool found= false;
        foreach(var item in returnPoints)
        {
            if(item.whenComingFrom ==  previousSceneName) 
            { 
                found = true;
                pos = item.transform.position;
                break;
            }
        }
        if(previousSceneName != null && !found)
        {
            Debug.LogError("No entry point for scene: " + previousSceneName);
            return;
        }

        player.transform.position = pos;
        companion.transform.position = new Vector2(pos.x + companionOffset.x, pos.y + companionOffset.y);
    }

    // make the player and companion face eachother
    void FaceEachother()
    {

    }

}
