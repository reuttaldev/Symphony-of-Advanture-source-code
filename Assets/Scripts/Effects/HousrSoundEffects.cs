using Google.Apis.Sheets.v4.Data;
using System.Collections;
using UnityEngine;

public class HousrSoundEffects : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    AudioClip caneBreaking, phoneRinging,footSteps;
    Animator animator;
    [SerializeField]
    float speed = 5;
    bool move = false;
    [SerializeField]
    GameObject answerPhone;
    [SerializeField]
    Sprite onCall;
    SpriteRenderer spriteRenderer;
    DialogueManager dialogueManager;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        dialogueManager = ServiceLocator.Instance.Get<DialogueManager>();
    }

    public void MoveTowards()
    {
        animator.Play("house_walk");
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

    }
    public void Fall()
    {
        move = false;
        animator.Play("house falling");

    }
    public void FootStep()
    {
        audioSource.PlayOneShot(footSteps);
    }

    public void Phone()
    {
        audioSource.PlayOneShot(phoneRinging);
        StartCoroutine(AfterPhone());

    }

    public void CaneBreaking()
    {
        audioSource.PlayOneShot(caneBreaking);
    }

    public void AfterFallText()
    {
        dialogueManager.StartDialogue("dr_house_fall");
    }

    IEnumerator AfterPhone()
    {
        yield return new WaitForSeconds(3);
       dialogueManager.StartDialogue("dr_house_duty_calls");
    }

    public void OnCall()
    {
        audioSource.Stop();
        answerPhone.GetComponent<SpriteRenderer>().sprite = onCall;

    }


}
