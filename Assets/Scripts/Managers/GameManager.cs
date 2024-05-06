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
    public  UnityEvent gameOverEvent;
    public  UnityEvent startedGameEvent;
    public static bool paused = false;
    string playerName;


    [SerializeField] 
    GameObject player, companion;

    private void Awake()
    {     
        ServiceLocator.Instance.Register<GameManager>(this);
        if (settings == null)
            Debug.LogError("Game manager is missing a reference to game settings");
        if(dataMigrationSettings == null)
            Debug.LogError("Game manager is missing a reference to data migration settings");

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
    void OnEnable()
    {
        startedGameEvent.AddListener(settings.IncreaseGameSessionIndex);
    }
    void OnDisable()
    {
        startedGameEvent.RemoveListener(settings.IncreaseGameSessionIndex);
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
        gameOverEvent.Invoke();
    }

    [YarnCommand("WalkToOffice")]

    public void WalkToOffice()
    {
        SceneManager.Instance.LoadScene("TownKeeperOffice");
        dialogueManager.StartDialogue("");
    }
    [YarnCommand("LeaveOffice")]

    public void LeaveOffice()
    {
        SceneManager.Instance.LoadScene("TownSquare");
    }


    // make the player and companion face eachother
    void FaceEachother()
    {

    }

}
