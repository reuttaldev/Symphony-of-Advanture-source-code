using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speed, runMultiplier = 4f;
    [SerializeField]
    InputActionReference moveAction,runAction;
    [SerializeField]
    AudioClip grassFootsteps, gravelFootsteps, floorFootsteps;
    Rigidbody2D rb;
    Animator animator;
    Vector2 input,lastInput;
    public bool Moving;
    public GroundMaterial groundMaterial = GroundMaterial.Gravel;
    [SerializeField]
    float rayDistance = 1;
    public Vector2 Input => lastInput;
    AudioSource audioSource;
    [SerializeField]
    LayerMask layerMask;
    bool canMove = true;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void CanMove()
    {
        canMove = true;

    }
    public void CantMove()
    {
        canMove = false;
    }
    void FixedUpdate()
    {
        if (!canMove)
            return;
        input = moveAction.action.ReadValue<Vector2>();
        if (input.x != 0 || input.y != 0)
        {
            Move(runAction.action.IsPressed());
            lastInput = input;
        }
        else
            StopMoving();
        Debug.DrawRay(transform.position, Vector2.down * rayDistance, Color.green); // Draw a green line to visualize the raycast


    }
    void Move(bool run)
    {
        Moving = true;
        if (run)
            rb.velocity = input * speed *runMultiplier;
        else
            rb.velocity = input * speed;
        animator.SetFloat("x", input.x);
        animator.SetFloat("y", input.y);
        animator.SetBool("Moving", true);
    }


    public IEnumerator MoveTowardsCoroutine(Transform walkTo)
    {
            Vector2 direction = (transform.position - walkTo.position).normalized;
        while (Vector2.Distance(walkTo.position, transform.position) > 0.2)
        {
            if (direction.x != 0 || direction.y != 0)
            {
                animator.SetFloat("x", direction.x);
                animator.SetFloat("y", direction.y);
            }
            transform.position = Vector3.MoveTowards(transform.position, walkTo.position, speed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
    void StopMoving()
    {
        if (Moving)
        {
            Moving = false;
            rb.velocity = Vector2.zero;
            animator.SetBool("Moving", false);
        }
    }


    // this method will be called from animation trigger each time the player seems to take a step
    // trigger footsteps sound effects based on which tile the player is walking on 
    public void Step()
    {
        DetectGroundMaterial();
        switch(groundMaterial)
        {
            case GroundMaterial.Gravel:
                audioSource.PlayOneShot(gravelFootsteps);
                break;
            case GroundMaterial.Grass:
                audioSource.PlayOneShot(grassFootsteps);
                break;
            case GroundMaterial.WoodFloor:
                audioSource.PlayOneShot(floorFootsteps);
                break;
        }
    }

    // detect which type of tile the player is walking on, so we know which footstep audioclip to play
    void DetectGroundMaterial()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, layerMask);
        // Does the ray intersect any objects in the ground layer

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            switch(hit.collider.name)
            {
                case "Grass":
                    groundMaterial = GroundMaterial.Grass;
                    break;
                case "Path":
                    groundMaterial = GroundMaterial.Gravel;
                    break;
                case "Floor":
                    groundMaterial = GroundMaterial.WoodFloor;
                    break;
            }
        }
    }

    public enum GroundMaterial
    {
        WoodFloor, Gravel,Grass
    }
}
