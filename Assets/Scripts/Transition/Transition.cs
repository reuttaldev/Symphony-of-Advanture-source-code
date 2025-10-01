using System;
using UnityEngine;
using UnityEngine.Events;

public class Transition : MonoBehaviour, IRegistrableService
{
    public string dialogueNodeName;
    public bool atStart = true;
    public string sceneName;
    public string date;
    private DialogueManager dialogueManager;
    public UnityEvent betweenDefaultToBubble, betweenBubbleToDefault,eventA, eventB,eventC;

    void Awake()
    {
        ServiceLocator.Instance.Register<Transition>(this);
    }
    public void Start()
    {
        if(ServiceLocator.Instance != null)    
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();

    }
    void OnEnable()
    {
        SceneManager.Instance.OnFadeInFinish += OnStart;
    }


    void OnDisable()
    {
        if(SceneManager.Instance != null)   
          SceneManager.Instance.OnFadeInFinish -= OnStart;
    }
    void OnStart()
    {
        if (atStart)
        {
            dialogueManager.SetView(false,false);
            dialogueManager.StartDialogue(dialogueNodeName);
        }
    }
    public void TransitionScene()
    {
        SceneManager.Instance.LoadScene(sceneName,date);
    }
}
