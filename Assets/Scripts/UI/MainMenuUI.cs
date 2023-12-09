using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainMenuUI : UIInterface
{

    public void LoadConfigurationFile()
    {

    }
    public void StartGame()
    {
        SceneManager.Instance.LoadScene("TownSquare");
    }
}
