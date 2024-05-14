using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCFollowPlayer : MonoBehaviour
{
    [SerializeField] float speed = 6f;
    [SerializeField] float offsetDistance = 0.3f;
    [SerializeField] AudioClip grassFootsteps, gravelFootsteps, floorFootsteps;
    [SerializeField] Transform followPositionLeft, followPositionRight, followPositionUp, followPositionDown;
    [SerializeField] PlayerMovement playerMovement;

    Animator animator;
    AudioSource audioSource;
    Vector2 input;
    bool move = false, followPlayer = true;
    private Transform wayPoint;
    bool wasOutOfOffest = false;
    Vector2 direction;
    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        wayPoint = followPositionDown;
    }

    public void FollowPlayer()
    {
        followPlayer = true;
    }

    public void StopFollowingPlayer()
    {
        followPlayer = false;
    }

    void FixedUpdate()
    {
        if (!followPlayer)
            return;

        // if distance from player is big enough, walk towards it.
        // Stay in the position suitable for the direction the player is going at (waypoint)
        if (playerMovement.Moving)
        {
            FindWaypoint();
            if (Vector2.Distance(transform.position, playerMovement.transform.position) > offsetDistance)
            {
                move = true;
                wasOutOfOffest = true;
            }
        }
        // if player stopped moving  but you still need to move to your waypoint
        else if (Vector2.Distance(transform.position, wayPoint.position) > 0.1 && wasOutOfOffest)
        {
            move = true;
        }
        else
            StopMoving();

        if (move)
            Move();
        direction =-1* (transform.position- playerMovement.transform.position).normalized;

        // always look at where you will be going next 
        if (direction.x != 0 || direction.y != 0)
            {
                animator.SetFloat("x", direction.x);
                animator.SetFloat("y", direction.y);
            }
    }

    void FindWaypoint()
    {
        input = playerMovement.Input;

        // Find the absolute values of input vector components
        float absX = Mathf.Abs(input.x);
        float absY = Mathf.Abs(input.y);

        // Determine the predominant direction based on the greater absolute component
        if (absY > absX)
        {
            if (input.y > 0)
                wayPoint = followPositionUp;
            else
                wayPoint = followPositionDown;
        }
        else
        {
            if (input.x > 0)
                wayPoint = followPositionRight;
            else
                wayPoint = followPositionLeft;
        }
    }

    void Move()
    {
        move = true;
        animator.SetBool("Moving", true);
        transform.position = Vector3.MoveTowards(transform.position, wayPoint.position, speed * Time.deltaTime);

    }
    void StopMoving()
    {
        if (move)
        {
            wasOutOfOffest = false;
            move = false;
            animator.SetBool("Moving", false);
        }
    }

}