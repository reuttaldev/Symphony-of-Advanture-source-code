using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CompanionMovement : MonoBehaviour
{

    [SerializeField]
    float speed;
    Rigidbody2D rb;
    Animator animator;
    Vector2 input;
    [SerializeField]
    InputActionReference moveAction;

    private bool moving = false, waiting = false, followPlayer = true;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void FollowPlayer()
    {
        followPlayer = true;
    }
    public void StopFollowingPlayer()
    {
        followPlayer = false;
        StopMoving();
    }

    private void StopMoving()
    {
        animator.SetBool("Moving", false);
        rb.velocity = new Vector2(0, 0);
        rb.bodyType = RigidbodyType2D.Static;
    }

    private void FixedUpdate()
    {
        if (!followPlayer || waiting)
            return;

        input = moveAction.action.ReadValue<Vector2>();
        rb.velocity = input * speed;
        if (input.x != 0 || input.y != 0)
        {
            // if now just starting to move I want to have some delay so it looks like the comanion is following the player 
            if (!moving)
            {
                StartCoroutine(WaitCoroutine());
            }
            animator.SetFloat("x", input.x);
            animator.SetFloat("y", input.y);
            animator.SetBool("Moving", true);
            moving = true;
        }
        else
            StopMoving();

    }

    IEnumerator WaitCoroutine()
    { 
        waiting = true;
        StopMoving();
        yield return new WaitForSeconds(0.3f);
        waiting = false;
    }
}
