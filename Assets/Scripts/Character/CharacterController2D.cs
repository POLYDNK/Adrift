// this script should allow the player to run and jump
// but is not fuckin working rn so idk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class CharacterController2D : MonoBehaviour
{
    // ------------------------ variables: ------------------------
    // movement parameters
    public float runSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravityScale = 20.0f;

    // components attached to player
    private BoxCollider2D coll;
    private Rigidbody2D rb;

    private bool isGrounded = false;


    // ------------------------ functions: ------------------------
    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = gravityScale;
    }


    private void Update()
    {
        UpdateIsGrounded();
        //Debug.Log("isGrounded:" + isGrounded);
        HandleHorizontalMovement();
        HandleJumping();
    }


    private void UpdateIsGrounded()
    {
        Bounds colliderBounds = coll.bounds;
        float colliderRadius = coll.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        
        // checks if player is grounded
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);
        
        this.isGrounded = false;
        
        // checks if any of the overlapping colliders are not player colliders
        // if so, set isGrounded to true
        if (colliders.Length > 0) {
            for (int i = 0; i < colliders.Length; i++) {
                if (colliders[i] != coll) {
                    this.isGrounded = true;
                    break;
                }
            }
        }
    }


    private void HandleHorizontalMovement()
    {
        //Vector2 moveDirection = InputManager.GetInstance().GetMoveDirection();
        //rb.velocity = new Vector2(moveDirection.x * runSpeed, rb.velocity.y);

        float xDirection = Input.GetAxis("Horizontal");
        Vector3 moveVector = new Vector3(xDirection, 0.0f, 0.0f);
        transform.position += moveVector * Time.deltaTime * runSpeed;
    }


    private void HandleJumping()
    {
        //bool jumpPressed = InputManager.GetInstance().GetJumpPressed();
        if (isGrounded && Input.GetKey(KeyCode.Space))
        {
            isGrounded = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

}
// ------------------------ end of file ------------------------
