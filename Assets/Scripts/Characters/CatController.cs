using Google.Apis.Sheets.v4.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour   
{
    [SerializeField]
    GameObject player,oldLady;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    float speed = 6f, offsetFromPlayer = 10, meowInterval = 5, waitToTeaseDuration, teaseDuration;
    [SerializeField]
    AudioClip[] meows;
    [SerializeField]
    Transform[] way;
    [SerializeField]
    Transform teasePlayerPosition, afterReturnPosition;
    [SerializeField]
    MissionData findMission,returnMission;
    int i = 0;
    bool reachedCurrentPoint = false;
    AudioSource audioSource;
    Animator animator;
    GameManager gameManager;
    // where we are currently walking to 
    Transform walkTo;
    bool escapeFromPlayer, meow;
    // Timer variable to track elapsed time
    private float timer = 0f;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = ServiceLocator.Instance.Get<GameManager>();
        animator = GetComponent<Animator>();
        escapeFromPlayer = false;
        meow = true;
        walkTo = teasePlayerPosition;
        findMission = findMission.GetRuntimeInstance<MissionData>();
        returnMission = returnMission.GetRuntimeInstance<MissionData>();

    }

    private void FixedUpdate()
    {
        if (!escapeFromPlayer)
            return;

        timer += Time.fixedDeltaTime;
        if (timer >= meowInterval)
        {
            if (!meow)
                return;
            audioSource.PlayOneShot(meows[Random.Range(0, meows.Length)]);
            timer = 0f;
        }

        // if you know your next walking point and you have not yet reached it, walk to it
        // if we have not reached our current destination 
        if (Vector2.Distance(transform.position, walkTo.position) > 0.1)
        {
            // keep moving toward the same position
            Move();
        }
        else
        {
            reachedCurrentPoint = true;
            StopMovingAnimation();
        }

        // if we are close to player 
        // let the cat know its next destination
        if (Vector2.Distance(transform.position, player.transform.position) < offsetFromPlayer)
        {
            // so the condition is not satisfied multiple times before the cat can even get away
            if (!reachedCurrentPoint)
                return;
            if (i + 1 == way.Length)
            {
                ReachedPoint();
            }
            // if we have reached our current destination, and we have a next one, move towards that one
            else
            {
                i++;
                walkTo = way[i];
                Move();
                reachedCurrentPoint = false;
            }
        }
       
    }

    public void Move()
    {
        var direction = -1 * (transform.position - walkTo.transform.position).normalized;
        // look at where you will be going next 
        {
            animator.SetFloat("x", direction.x);
            animator.SetFloat("y", direction.y);
        }
        animator.SetBool("Moving", true);
        transform.position = Vector3.MoveTowards(transform.position, walkTo.position, speed * Time.deltaTime);

    }
    public void StartEscapingPlayer()
    {
        escapeFromPlayer = true;
    }
    public void StopEscapingPlayer()
    {
        escapeFromPlayer = false;

    }
    private void StopMovingAnimation()
    {
        animator.SetBool("Moving", false);
    }
    // this will be called once the dialouge with the old lady has started
    public void TeasePlayer()
    {
        StartCoroutine(TeasePlayerCoroutine());
    }

    public void Step() // called from animator 
    {

    }
    IEnumerator TeasePlayerCoroutine()
    {
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(waitToTeaseDuration);
        spriteRenderer.enabled = true;

        yield return WalkTo();
        yield return new WaitForSeconds(teaseDuration);
        walkTo = way[0];
        yield return WalkTo();
        StopMovingAnimation();
        animator.SetTrigger("Resting");
        StartEscapingPlayer();
    }
    IEnumerator WalkTo()
    {
        while (Vector2.Distance(transform.position, walkTo.position) > 0.1)
        {
            Move();
            yield return new WaitForEndOfFrame();
        }
        StopMovingAnimation();
    }

    void ReachedPoint()
    {
        StopMovingAnimation();
        escapeFromPlayer = false;
        animator.SetTrigger("Resting");
    }

    bool HasReachedEnd()
    {
        var lastPos = way[way.Length - 1].position;
        return ((int)lastPos.x == (int)transform.position.x && (int)lastPos.y == (int)transform.position.y);

    }
    public void SetAtEndPosition()
    {
        transform.position = way[way.Length - 1].position;
    }
    public void PlayerCatchedUs()
    {
        StopMovingAnimation() ;  
       // make the cat follow the player
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero;
        findMission.EndMission();
    }
    public void PlayerReturnedUs()
    {
        Debug.Log("Player returned cat");
        transform.SetParent(null);
        transform.position = afterReturnPosition.position;
        //returnMission.EndMission(); done after conversation 
    }

    // need rigit body on the collision object for this to trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player && HasReachedEnd())
        {
            if(findMission.State != MissionState.CompletedSuccessfully)
                PlayerCatchedUs();
        }
        if (collision.gameObject == oldLady)
        {
            if (returnMission.State != MissionState.CompletedSuccessfully)
                PlayerReturnedUs();
            }
    }
}
