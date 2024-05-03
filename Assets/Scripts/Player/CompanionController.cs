using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CompanionController : MonoBehaviour
{
    [SerializeField]
    DialogueIntractable dialogueIntractable;


    [SerializeField]
    float speed;
    Rigidbody2D rb;
    Animator animator;
    Vector2 input;
    [SerializeField]
    InputActionReference moveAction;

    private bool moving = false, waiting = false, followPlayer = true;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        FollowPlayer();
    }

    public void FollowPlayer()
    {
        followPlayer = true;
    }
    public void StopFollowingPlayer()
    {
        followPlayer = false;
    }

    private void FixedUpdate()
    {
        if (!followPlayer)
            return;
        if (waiting)
        {
            animator.SetBool("Moving", false);
            rb.velocity = new Vector2(0, 0); ;
            return;
        }

        input = moveAction.action.ReadValue<Vector2>();
        rb.velocity = input * speed;
        if (input.x != 0 || input.y != 0)
        {
            // if now just starting to move I want to have some delay so it looks like the comanion is following the player 
            if (!moving)
            {
                Debug.Log("waiting");
                StartCoroutine(WaitCoroutine());
            }
            animator.SetFloat("x", input.x);
            animator.SetFloat("y", input.y);
            animator.SetBool("Moving", true);
            moving = true;
        }
        else
        {
            animator.SetBool("Moving", false);
            moving = false;
        }
    }

    IEnumerator WaitCoroutine()
    { 
        waiting = true;
        yield return new WaitForSeconds(0.3f);
        waiting = false;
    }
}
