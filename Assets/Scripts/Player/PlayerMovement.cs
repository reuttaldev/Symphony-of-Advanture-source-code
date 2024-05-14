using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    InputActionReference moveAction;
    [SerializeField]
    AudioClip grassFootsteps, gravelFootsteps, floorFootsteps;
    Rigidbody2D rb;
    Animator animator;
    Vector2 input;
    public bool Moving;
    public GroundMaterial groundMaterial = GroundMaterial.Gravel;
    [SerializeField]
    float rayDistance = 1;
    public Vector2 Input => input;
    AudioSource audioSource;
    [SerializeField]
    LayerMask layerMask;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }


    void FixedUpdate()
    {
        input = moveAction.action.ReadValue<Vector2>();
        if (input.x != 0 || input.y != 0)
            Move();
        else
            StopMoving();
        Debug.DrawRay(transform.position, Vector2.down * rayDistance, Color.green); // Draw a green line to visualize the raycast


    }
    void Move()
    {
        Moving = true;
        rb.velocity = input * speed;
        animator.SetFloat("x", input.x);
        animator.SetFloat("y", input.y);
        animator.SetBool("Moving", true);
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
        Debug.Log(LayerMask.LayerToName(1 << 3));
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
