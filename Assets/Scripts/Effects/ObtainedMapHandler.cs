using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ObtainedMapHandler : MonoBehaviour
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    [SerializeField]
    DialogueIntractable companionInteractable; 
    [SerializeField]
    MissionData map1, map2, map3;
    [SerializeField]
    ReturnPoint outsideDoorPos;
    [SerializeField]
    GameObject castelDoor;
    void OnEnable()
    {
        if (ServiceLocator.Instance == null)
            return;
        var gameManger = ServiceLocator.Instance.Get<GameManager>();    
        gameManger.OnPlayerPlaced += PlaceNextToDoor;
    }
    void OnDisable()
    {
        if (ServiceLocator.Instance == null)
            return;
        var gameManger = ServiceLocator.Instance.Get<GameManager>();
        if (gameManger != null)
            gameManger.OnPlayerPlaced += PlaceNextToDoor;
    }

    private void Awake()
    {
        map1 = map1.GetRuntimeInstance<MissionData>();
        map2 = map2.GetRuntimeInstance<MissionData>();
        map3 = map3.GetRuntimeInstance<MissionData>();
        
    }
    void Start()
    {
        if(dialogueRunner != null)
            dialogueRunner.AddCommandHandler("DoorScene", GetToDoorScene);
    }
    public static void GetToDoorScene()
    {
        SceneManager.Instance.LoadScene("OldCity");
    }
    public void PlaceNextToDoor()
    {
        if (SceneManager.GetActiveScene() == "OldCity")
        {
            if (CheckIfObtainedMap())
            {
                ServiceLocator.Instance.Get<GameManager>().PlacePlayer(outsideDoorPos);
                companionInteractable.Interactable(true);
                companionInteractable.ChangeConversationStartNode("companion_at_castel_door");
                companionInteractable.Trigger();
                castelDoor.SetActive(true);
            }
        }
    }
    private bool CheckIfObtainedMap()
    {
        return (map1.State == MissionState.CompletedSuccessfully && map2.State == MissionState.CompletedSuccessfully && map3.State == MissionState.CompletedSuccessfully);
    }

    // this is called in village after the player collects the 3rd map piece from the yard
    // and after sona gives the second piece 
    public void CheckAndActFullMap()
    {
        if(CheckIfObtainedMap())
        {

            dialogueRunner.Stop();
            companionInteractable.Interactable(true);
            companionInteractable.ChangeConversationStartNode("companion_obtained_map");
            companionInteractable.Trigger();
        }
    }
}
