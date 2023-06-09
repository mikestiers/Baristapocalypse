using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerStateMachine : StateMachine, IIngredientParent
{
    // Player Singleton
    [HideInInspector] public static PlayerStateMachine Instance { get; private set; }

    //[Header("Player Attributes")]
    [field: SerializeField] public float moveSpeed { get; private set; }
    [field: SerializeField] public float jumpForce { get; private set; }
    [field: SerializeField] public float ingredienThrowForce { get; private set; }
    

    //[Header("Ground Check")]
    [field: SerializeField] public LayerMask isGroundLayer { get; private set; }
    [field: SerializeField] public float groundCheckRadius { get; private set; }
    [field: SerializeField] public Transform groundCheck { get; private set; }
    [field: SerializeField] public bool isGrounded;
    [field: SerializeField] public float dashForce;
    [field: SerializeField] public float dashTime;



    //[Header("Interactables")]
    [field: SerializeField] public LayerMask isStationLayer { get; private set; }
    [field: SerializeField] public LayerMask isIngredientLayer { get; private set; }
    [HideInInspector] public BaseStation selectedStation { get; private set; }
    [HideInInspector] public Collider ingredientCollider;
    [field: SerializeField] public GameObject ingredientInstanceHolder;

    [field: SerializeField] public Transform ingredientHoldPoint { get; private set; }
    [HideInInspector] public Ingredient ingredient { get; private set; }
    [field: SerializeField] public bool hasIngredient;


    [HideInInspector] public Rigidbody rb { get; private set; }
    [HideInInspector] public Vector3 curMoveInput;
    [HideInInspector] public Vector3 moveDir;

    // When station selected
    private GameObject visualGameObject;
    
    // Components
    [field: SerializeField] public InputManager inputManager { get; private set; }

    private void Awake()
    {
        if (Instance != null ) 
            Debug.LogError("There is more than one player instance"); 
        
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();

        //Set variables if null
        if (moveSpeed <= 0) moveSpeed = 10.0f;
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (dashForce <= 0) dashForce = 25f;
        if (dashTime <= 0) dashTime = 0.1f;
        if (ingredienThrowForce <= 0) ingredienThrowForce = 10f;
        if (groundCheckRadius <= 0) groundCheckRadius = 0.05f;
        

        SwitchState(new PlayerGroundState(this)); // Start player state

        

    }

    
    public bool IsGrounded()
    {
        isGrounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, isGroundLayer).Length > 0;
        return isGrounded;
    }

    public void Move(float moveSpeed)
    {
        
        if (inputManager.moveDir == Vector3.zero) return;
        curMoveInput = inputManager.moveDir * moveSpeed * Time.deltaTime;
        transform.forward = inputManager.moveDir;

        rb.MovePosition(rb.position + curMoveInput);
    }

   
    public void Interact(InputAction.CallbackContext context)
    {
        if (selectedStation)
        {
            selectedStation.Interact(this);
        }
        //Debug.Log(selectedStation);

    }

   
    public void InteractAlt(InputAction.CallbackContext ctx)
    {
        if (selectedStation)
        {
            selectedStation.InteractAlt(this);
        }

    }


    public void SetSelectedStation(BaseStation baseStation)
    {
        selectedStation = baseStation;

       

       
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

    public void ThrowIngedient()
    {
        TurnOnIngredientCollider();
        Rigidbody rb = ingredientHoldPoint.GetComponentInChildren<Rigidbody>();
        ingredientHoldPoint.DetachChildren();
         
        rb.isKinematic = false;
        rb.AddForce(transform.forward * ingredienThrowForce, ForceMode.Impulse);
      
        ClearIngredient();
               
    }

    public void GrabIngedientFromFloor(Ingredient floorIngredient,IngredientSO ingredientSO )
    {
        floorIngredient.SetIngredientParent(this);
        floorIngredient.DestroyIngredient();
        Ingredient.SpawnIngredient(ingredientSO, this);
                
    }


    public void ClearIngredient()
    {
        ingredient = null;
    }

    public bool HasIngredient()
    {
        return ingredient != null;
    }

    public void TurnOffIngredientCollider()
    {
        ingredientCollider = ingredientInstanceHolder.GetComponentInChildren<BoxCollider>();
        ingredientCollider.enabled = false;
    }

    public void TurnOnIngredientCollider()
    {
        ingredientCollider = ingredientInstanceHolder.GetComponentInChildren<BoxCollider>();
        ingredientCollider.enabled = true;
    }

    public void Show(GameObject visualGameObject)
    {
        visualGameObject.SetActive(true);
    }

    public void Hide(GameObject visualGameObject)
    {
        visualGameObject?.SetActive(false);
    }


}
