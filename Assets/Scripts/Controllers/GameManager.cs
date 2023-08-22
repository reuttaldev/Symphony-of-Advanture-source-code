using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IRegistrableService
{

    private void Awake()
    {
        
    }
    void Start()
    {
        ServiceLocator.Instance.Register<GameManager>(this);
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
}
