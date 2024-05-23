using Google.Apis.Sheets.v4.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    [SerializeField]
    float speed = 6f, offsetFromPlayer = 10, meowInterval = 5, waitToTeaseDuration, teaseDuration;
    [SerializeField]
    AudioClip[] meows;
    [SerializeField]
    AudioClip purr;
    [SerializeField]
    Transform[] way;
    [SerializeField]
    Transform teasePlayerPosition;
    [SerializeField]
    MissionData associatedMission;
    [SerializeField]
    GameObject interactble;
    int i = 0;
    bool reachedCurrentPoint = false;
    AudioSource audioSource;
    Animator animator;
    GameManager gameManager;
    // where we are currently walking to 
    Transform walkTo;
    bool move = false;
    bool escapeFromPlayer, meow;
    // Timer variable to track elapsed time
    private float timer = 0f;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = ServiceLocator.Instance.Get<GameManager>();
        animator = GetComponent<Animator>();
        walkTo = way[0];
        escapeFromPlayer = false;
        meow = true;
        walkTo = teasePlayerPosition;
        if (associatedMission.State == MissionState.OnGoing)
            transform.position = way[0].position;
        else
            gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!escapeFromPlayer)
            return;
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
            StopMoving();
        }

        // if we are close to player 
        // let the cat know its next destination
        if (Vector2.Distance(transform.position, player.transform.position) < offsetFromPlayer)
        {
            // so the condition is not satisfied multiple times before the cat can even get away
            if (!reachedCurrentPoint)
                return;
            // if we have reached our current destination, and we have a next one, move towards that one
            if (i + 1 == way.Length)
            {
                StopMoving();
                animator.SetTrigger("Resting");
                escapeFromPlayer = false;
                interactble.gameObject.SetActive(true);
            }
            else
            {
                i++;
                walkTo = way[i];
                Move();
                reachedCurrentPoint = false;
            }
        }
        timer += Time.fixedDeltaTime;
        if (timer >= meowInterval)
        {
            if (!meow)
                return;
            audioSource.PlayOneShot(meows[Random.Range(0, meows.Length)]);
            timer = 0f;
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
    public void StopMoving()
    {
        animator.SetBool("Moving", false);
        move = false;
    }


    public void Step()
    {

    }


    // this will be called once the dialouge with the old lady has started
    public void TeasePlayer()
    {
        StartCoroutine(TeasePlayerCoroutine());
    }

    IEnumerator TeasePlayerCoroutine()
    {
        yield return new WaitForSeconds(waitToTeaseDuration);
        yield return WalkTo();
        //yield return new WaitForSeconds(teaseDuration);
        walkTo = way[0];
        yield return WalkTo();
        StopMoving();
        animator.SetTrigger("Resting");
    }
    IEnumerator WalkTo()
    {
        while (Vector2.Distance(transform.position, walkTo.position) > 0.1)
        {
            Move();
            yield return new WaitForEndOfFrame();
        }
        StopMoving();

    }

    public void StartEscapingPlayer()
    {
        escapeFromPlayer = true;
    }
}
