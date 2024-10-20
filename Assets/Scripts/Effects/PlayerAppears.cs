using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerAppears : MonoBehaviour
{
    [SerializeField]
    MissionData talkToAstridMission;
    [SerializeField]
    AudioClip playerFallAudio;
    [SerializeField]
    CameraShake cameraShake;
    Animator animator;
    [SerializeField]
    PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void OnEnable()
    {
        SceneManager.Instance.OnSceneLoaded += HandleSceneLoad;
    }
    void OnDisable()
    {
        if (SceneManager.Instance != null)
            SceneManager.Instance.OnSceneLoaded -= HandleSceneLoad;
    }
    public void HandleSceneLoad(string previousSceneName)
    {
        if (talkToAstridMission.State != MissionState.CompletedSuccessfully)
            ShowEffect();
    }
    void ShowEffect()
    {
        animator.Play("player_appear");
        playerMovement.CantMove();
    }
    public void PlayClip()
    {
        AudioManager.Instance.PlayClip(playerFallAudio);    
    }

    public void ShakeCamera()
    {
        cameraShake.ShakeCamera();
    }
    public void OnAnimationOver()
    {
        talkToAstridMission.StartMission(); 
    }
}
