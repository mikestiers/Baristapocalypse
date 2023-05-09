using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;

    Rigidbody rb;
    Vector3 curMoveInput;
    Vector3 moveDir;

    public LayerMask isGroundLayer;
    public float groundCheckRadius;
    public Transform groundCheck;
    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();

        //Set variables if null
        if (moveSpeed <= 0) moveSpeed = 10.0f;
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (groundCheckRadius <= 0) groundCheckRadius = 2.0f;

        //Get player actions
        GameManager.Instance.playerInput.Player.Move.performed += ctx => Move(ctx);
        GameManager.Instance.playerInput.Player.Move.canceled += ctx => Move(ctx);
        GameManager.Instance.playerInput.Player.Interact.performed += ctx => Interact(ctx);
        GameManager.Instance.playerInput.Player.Jump.performed += ctx => Jump(ctx);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, isGroundLayer).Length > 0;

        curMoveInput.y = rb.velocity.y;
        rb.velocity = curMoveInput;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.action == null) return;
        if (ctx.canceled)
        {
            curMoveInput = Vector3.zero;
            moveDir = Vector3.zero;
            return;
        }

        Vector2 move = ctx.action.ReadValue<Vector2>();
        move.Normalize();

        moveDir = new Vector3(move.x, 0, move.y).normalized;
        curMoveInput = moveDir * moveSpeed;
    }

    public void Interact(InputAction.CallbackContext ctx)
    {

    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!isGrounded) return;

        rb.AddForce(jumpForce * Vector3.up);
    }
}
