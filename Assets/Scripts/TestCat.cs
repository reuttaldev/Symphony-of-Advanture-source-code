using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCat : MonoBehaviour
{
    public CatController controller;

    private void Start()
    {
        controller.gameObject.SetActive(true);  
        controller.StartEscapingPlayer  ();
    }
}
