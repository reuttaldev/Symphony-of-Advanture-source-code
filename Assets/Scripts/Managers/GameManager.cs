using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using Yarn;
using Yarn.Unity;

public class GameManager : MonoBehaviour, IRegistrableService
{
    public GameSettings settings;
    public DataMigrationSettings dataMigrationSettings;
    public static bool paused = false;
    string playerName;
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
        if (settings == null)
            Debug.LogError("Game manager is missing a reference to game settings");
        if(dataMigrationSettings == null)
            Debug.LogError("Game manager is missing a reference to data migration settings");
        if (returnPoints.Count == 0)
            Debug.LogWarning("Forgot to drag in scene exits to game manager");
    }
    void OnEnable()
    {
        SceneManager.Instance.OnSceneLoaded += PlacePlayerInScene;
    }
    void OnDisable()
    {
        if(SceneManager.Instance != null)   
            SceneManager.Instance.OnSceneLoaded -= PlacePlayerInScene;
    }
    public void SetPlayerName(string playerName)
    {
        // set player name in yarn
        var nameVariableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();
        nameVariableStorage.SetValue("$playerName", playerName);
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
        GameOver();
        Application.Quit();
    }

    public void GameOver()
    {
        ExportManager.Instance.SendCSVByEmail();
    }


    #region COMPANION AND PLAYER POSITION CONTROLLER METHODS

    // each scene trigger exit also has a return point child, which shows us where to place the player and companion
    // when RETURNING from that scene exit, i.e entering this scene from Scene TransitionTrigger .TransitionTo
    void PlacePlayerInScene(string previousSceneName)
    {
        if(string.IsNullOrWhiteSpace(previousSceneName))
        {
            Debug.LogWarning("previous scene name is null - cannot place player in scene");
            return;
        }
        if (previousSceneName == "StartMenu")
            return;
        if(returnPoints == null || returnPoints.Count ==0)
        {
            Debug.Log("You forgot to set return points list in game manager");
        }
        ReturnPoint returnPoint = null;
        foreach(var item in returnPoints)
        {
            if(item.whenComingFrom ==  previousSceneName) 
            { 
                returnPoint = item;
                break;
            }
        }
        if (returnPoint == null)
        {
            Debug.LogError("No entry point for scene: " + previousSceneName+" or you forget to add a reference to it in game manger.");
        }
        PlacePlayer(returnPoint);
        OnPlayerPlaced?.Invoke();
    }
    public void PlacePlayer(ReturnPoint returnPoint)
    {
        player.transform.position = returnPoint.transform.position;
        Direction lookAtDirection = returnPoint.lookAt;
        CharacterLookAt(player, lookAtDirection);
        if(companion.GetComponent<NPCFollowPlayer>().FollowingPlayer)
        {
            PlaceNextToPlayer(companion, returnPoint.companionAtSideOfPlayer);
            CharacterLookAt(companion, lookAtDirection);
        }

    }

    void PlaceNextToPlayer(GameObject character, Direction direction)
    {
        character.transform.position = GetNextToPlayerPos(direction).position;
    }
    void CharacterLookAt(GameObject character, Direction direction)
    {
        var animator = character.GetComponent<Animator>();
        switch (direction)
        {
            case Direction.left:
                animator.SetFloat("x", -1);
                animator.SetFloat("y", 0);
                break;
            case Direction.right:
                animator.SetFloat("x", 1);
                animator.SetFloat("y", 0);
                break;
            case Direction.up:
                animator.SetFloat("x", 0);
                animator.SetFloat("y", 1);
                break;
            case Direction.down:
                animator.SetFloat("x", 0);
                animator.SetFloat("y", -1);
                break;
        }
    }

    public void CompanionWalkToPlayer(string d)
    {
        CompanionWalkToPlayer(GetDirection(d),true);
    }
    public void CompanionWalkToPlayer(Direction direction,bool faceEachother)
    {
        NPCFollowPlayer companionMovements = companion.GetComponent<NPCFollowPlayer>(); 
        StartCoroutine(WalkNPC(companionMovements, GetNextToPlayerPos(direction), faceEachother, direction));
    }
    public void CompanionWalkTo(Transform transform)
    {
        NPCFollowPlayer companionMovements = companion.GetComponent<NPCFollowPlayer>();
        StartCoroutine(WalkNPC(companionMovements, transform));
        Debug.Log("companion walk to");
    }
    // when player is static, walk towards it. not the same as npc follow player bc there we are following the player when it's moving
    IEnumerator WalkNPC(NPCFollowPlayer character, Transform walkTo,bool faceEachother = false, Direction direction= Direction.none)
    {
        // stop regular companion movements (if he is already following the player
        bool wasFollowing = character.FollowingPlayer;
        if(wasFollowing) 
            character.StopFollowingPlayer();
        while (Vector2.Distance(walkTo.position, character.transform.position) > 0.2)
        {
            character.Move(walkTo);
            yield return new WaitForFixedUpdate();
        }
        // we have reached our position
        character.StopMoving();
        // make the player and character face each other
        if(faceEachother)
            FaceEachother(direction);
        // resume previous follow directions
        if(wasFollowing)
            character.FollowPlayer();

    }

    // make companion and player face each other
    void FaceEachother(Direction companionDirection)
    {
        CharacterLookAt(player, companionDirection);
        CharacterLookAt(companion, GetOppositeDirection(companionDirection));
    }



    Direction GetOppositeDirection(Direction direction) 
    {
        Direction opposite = Direction.right;
        switch (direction)
        {
            case Direction.up:
                opposite = Direction.down;
                break;
            case Direction.right:
                opposite = Direction.left;
                break;
            case Direction.down:
                opposite = Direction.up;
                break;
        }
        return opposite;
    }

    public void SetCompanionPosition(Transform at)
    {
        companion.gameObject.transform.position = at.position;
    }
    Transform GetNextToPlayerPos(Direction direction) 
    {
        switch (direction)
        {
            case Direction.left:
                return leftOfPlayer;
            case Direction.right:
                return rightOfPlayer;
            case Direction.up:
                return upOfPlayer;
            case Direction.down:
                return downOfPlayer;
        }
        return leftOfPlayer;
    }
    Direction GetDirection(string d)
    {
        switch (d)
        {
            case "left":
                return Direction.left;
            case "right":
                return Direction.right;
            case "up":
                return Direction.up;
            case "down":
                return Direction.down;
        }
        return Direction.left;
    }


    // when a dialogue starts, player and companion should face the person they are talking to
    public void FacePosition(Direction direction, Vector2 position)
    {
        CharacterLookAt(player, direction);
        CharacterLookAt(companion, direction);
        // make them stand right infront of who they are takling to, for symmetry 
    }
    #endregion
}

    public enum Direction
    {
        left, right, up, down, none
    }