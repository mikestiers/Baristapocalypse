using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerStateMachine : StateMachine
{
    //Components
    public InputManager inputManager;
    public Transform groundCheck;
    [HideInInspector] public Rigidbody rb;

    //jump force
     public float jumpForce;

    //Graound check variables
    public LayerMask isGroundLayer;
    public float groundCheckRadius;
    public bool isGrounded;

    //Interaction vars
    private Vector3 lastInteractDirection;
    //public PickupObjects pickupObjects;
    public Transform target;
    public LayerMask layerMask;
    public Transform pickupPoint;
    public float dropForce;
    [HideInInspector] public bool isPickedup; 

    //movement vars
    float RotationDamping = 10;


    // Start is called before the first frame update
    private void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();

        //Set variables if null
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (groundCheckRadius <= 0) groundCheckRadius = 2.0f;
        target = GameObject.FindGameObjectWithTag("pickups").transform;
        
      

        SwitchState(new PlayerMoveState(this)); // Start player state

    }

    public void HandleMovement(float deltaTime)
    {
        //player movement
        inputManager.playerMovement.y = rb.velocity.y;
        rb.velocity = inputManager.playerMovement;

        //rb.transform.rotation = Quaternion.LookRotation

        FaceMovementDirection(inputManager.movInput, deltaTime);
        

    }

    public void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
       rb.transform.rotation = Quaternion.Lerp(
            rb.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * RotationDamping);
    }

    public bool IsGrounded()
    {
        isGrounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, isGroundLayer).Length > 0;
        return isGrounded;
    }



    public bool HandleInteractions()
    {
        float interactDistance = 2f;
        RaycastHit raycastHIt;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Debug.Log(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out raycastHIt, interactDistance, layerMask));
        return (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out raycastHIt, interactDistance, layerMask));
        //Debug.DrawLine(transform.position, target.position, Color.red);
        

        
       
        //Physics.CapsuleCast()
    }




}
