using Google.Apis.Sheets.v4.Data;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class HousrSoundEffects : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    AudioClip caneBreaking, phoneRinging,footSteps;
    Animator animator;
    [SerializeField]
    float speed = 5;
    bool move = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        MoveTowards();
    }

    public void MoveTowards()
    {
        move = true; 
    }
    void Update()
    {
        if (!move)
            return;
        transform.position+= (Vector3)Vector2.down * speed * Time.deltaTime;
    }

    public void StopMoving()
    {
        move = false;
        animator.StopPlayback();
        Debug.Log("stoping");

    }
    public void Fall()
    {
        move = false;
        animator.SetTrigger("Fall");

    }
    public void FootStep()
    {
        audioSource.PlayOneShot(footSteps);
    }

    public void Phone()
    {
        audioSource.PlayOneShot(phoneRinging);

    }

    public void CaneBreaking()
    {
        audioSource.PlayOneShot(caneBreaking);
    }
}
