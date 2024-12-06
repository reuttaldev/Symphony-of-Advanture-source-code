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
        talkToAstridMission = talkToAstridMission.GetRuntimeInstance<MissionData>();   
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
        ServiceLocator.Instance.Get<InputManager>().ActivatePausedUIMap();
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
