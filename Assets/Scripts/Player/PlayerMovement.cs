using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float smoothSpeed = 0.1f;
    Rigidbody2D rb;
    Animator animator;
    Vector2 input;
    Vector2 smoothedInput;
    Vector2 smoothedInputVelocity;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        smoothedInput = Vector2.SmoothDamp(smoothedInput, input, ref smoothedInputVelocity, smoothSpeed);
        rb.velocity = input  * speed;
    }
    private void OnMove(InputValue inputValue)
    {
        input = inputValue.Get<Vector2>();
    }
}
