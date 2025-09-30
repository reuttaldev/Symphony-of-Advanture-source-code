using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IRegistrableService
{
    public static bool paused = false;
    [SerializeField]
    List<ReturnPoint> returnPoints;
    [SerializeField] 
    GameObject player, companion;
    [SerializeField]
    Transform leftOfPlayer, rightOfPlayer, downOfPlayer, upOfPlayer;
    bool loaded = false;
    public event Action OnPlayerPlaced;


    private void Awake()
    {
        ServiceLocator.Instance.Register<GameManager>(this);
        if (returnPoints.Count == 0)
            Debug.LogWarning("Forgot to drag in scene exits to game manager");
    }
    void OnEnable()
    {
       // SceneManager.Instance.OnSceneLoaded += PlacePlayerInScene;
    }
    void OnDisable()
    {
        //if(SceneManager.Instance != null)   
           // SceneManager.Instance.OnSceneLoaded -= PlacePlayerInScene;
    }
    public void SwitchScene(string name)
    {
        SceneManager.Instance.LoadScene(name);  
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
        Debug.Log("exiting game");
        Application.Quit();
    }

}

    public enum Direction
    {
        left, right, up, down, none
    }