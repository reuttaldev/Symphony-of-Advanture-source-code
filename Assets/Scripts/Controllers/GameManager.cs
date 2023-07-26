using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SimpleSingleton<GameManager>
{


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
}
