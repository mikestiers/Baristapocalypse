using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour //, IIngredientParent
{ 
    /*
    [Header("Player Attributes")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Ground Check")]
    [SerializeField] private LayerMask isGroundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private bool isGrounded;

    [Header("Interactables")]
    [SerializeField] private LayerMask isStationLayer;
    private BaseStation selectedStation;

    [SerializeField] private Transform ingredientHoldPoint;
    private Ingredient ingredient;

    Rigidbody rb;
    Vector3 curMoveInput;
    Vector3 moveDir;

    private void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();

        //Set variables if null
        if (moveSpeed <= 0) moveSpeed = 10.0f;
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (groundCheckRadius <= 0) groundCheckRadius = 0.05f;

        //Get player actions
        GameManager.Instance.playerInput.Player.Move.performed += ctx => Move(ctx);
        GameManager.Instance.playerInput.Player.Move.canceled += ctx => Move(ctx);
        GameManager.Instance.playerInput.Player.Interact.performed += ctx => Interact(ctx);
        GameManager.Instance.playerInput.Player.Jump.performed += ctx => Jump(ctx);
        GameManager.Instance.playerInput.Player.InteractAlt.performed += ctx => InteractAlt(ctx);

    }

    // Update is called once per frame
    private void Update()
    {
        //Movement
        isGrounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, isGroundLayer).Length > 0;

        curMoveInput.y = rb.velocity.y;
        rb.velocity = curMoveInput;

        float interactDistance = 6.0f;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, interactDistance, isStationLayer))
        {
            if (raycastHit.transform.TryGetComponent(out BaseStation baseStation))
            {
                if (baseStation != selectedStation)
                {
                    SetSelectedStation(baseStation);
                }
            }
            else
            {
                SetSelectedStation(null);
            }
        }
        else
        {
            SetSelectedStation(null);
        }
    }

    public void SetSelectedStation(BaseStation baseStation)
    {
        selectedStation = baseStation;
    }

    private void Move(InputAction.CallbackContext ctx)
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

        transform.forward = moveDir;
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (selectedStation)
        {
            selectedStation.Interact(this);
        }
    }

    private void InteractAlt(InputAction.CallbackContext ctx)
    {
        if (selectedStation)
        {
            selectedStation.InteractAlt(this);
        }
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (!isGrounded) return;

        rb.AddForce(jumpForce * Vector3.up);
    }

    public Transform GetIngredientTransform()
    {
        return ingredientHoldPoint;
    }

    public void SetIngredient(Ingredient ingredient)
    {
        this.ingredient = ingredient;
    }

    public Ingredient GetIngredient()
    {
        return ingredient;
    }

    public void ClearIngredient()
    {
        ingredient = null;
    }

    public bool HasIngredient()
    {
        return ingredient != null;
    }
    */
}
