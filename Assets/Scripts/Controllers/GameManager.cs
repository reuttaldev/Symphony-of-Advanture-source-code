using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour, IRegistrableService
{
    public  UnityEvent gameOverEvent;
    private void Awake()
    {     
        ServiceLocator.Instance.Register<GameManager>(this);
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("exiting game");

    }
    public void GameOver()
    {
        ExportManager exportManager = ServiceLocator.Instance.Get<ExportManager>();
        exportManager.SendCSVByEmail();
        gameOverEvent.Invoke();
    }
}
