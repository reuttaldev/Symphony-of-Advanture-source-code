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
        SceneManager.Instance.OnFadeInFinish += ShowEffect;
    }
    void OnDisable()
    {
        if (SceneManager.Instance != null)
            SceneManager.Instance.OnFadeInFinish -= ShowEffect;
    }
    void ShowEffect()
    {
        if (talkToAstridMission.State == MissionState.CompletedSuccessfully)
            return;
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
        Debug.Log("On animation over");
        talkToAstridMission.StartMission(); 
    }
}
