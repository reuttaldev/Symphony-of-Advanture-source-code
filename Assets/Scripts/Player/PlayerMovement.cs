using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speed;
    Rigidbody2D rb;
    Animator animator;
    Vector2 input;
    [SerializeField]
    InputActionReference moveAction;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        input = moveAction.action.ReadValue<Vector2>();
        rb.velocity = input * speed;
    }
}
