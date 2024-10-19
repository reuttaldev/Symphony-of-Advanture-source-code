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

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("player_appear");
    }
    void OnEnable()
    {
        SceneManager.Instance.OnSceneLoaded += HandleSceneLoad;
    }
    void OnDisable()
    {
        SceneManager.Instance.OnSceneLoaded -= HandleSceneLoad;
    }
    public void HandleSceneLoad(string previousSceneName)
    {
        animator.Play("player_appear");
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
