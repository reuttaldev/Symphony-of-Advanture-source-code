using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDialogueUI : MonoBehaviour
{
    private void Awake()
    {
    }
    public void OpenMenu()
    {
        this.gameObject.SetActive(true);
    }
    public void CloseMenu()
    {
        this.gameObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
