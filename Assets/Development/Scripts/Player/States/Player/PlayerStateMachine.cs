using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


[RequireComponent(typeof(Rigidbody))]
public class PlayerStateMachine : StateMachine, IIngredientParent, IMessParent
{
    // Player Singleton
    [HideInInspector] public static PlayerStateMachine Instance { get; private set; }

    // [Header("Player Attributes")]
    [field: SerializeField] public float moveSpeed { get; private set; }
    [field: SerializeField] public float jumpForce { get; private set; }
    [field: SerializeField] public float ingredienThrowForce { get; private set; }

    // [Header("Ground Check")]
    [field: SerializeField] public LayerMask isGroundLayer { get; private set; }
    [field: SerializeField] public float groundCheckRadius { get; private set; }
    [field: SerializeField] public Transform groundCheck { get; private set; }
    [HideInInspector] public bool isGrounded;
    [SerializeField] public float dashForce;
    [SerializeField] public float dashTime;

    // [Header("Interactables")]
    [field: SerializeField] public LayerMask isStationLayer { get; private set; }
    [field: SerializeField] public LayerMask isIngredientLayer { get; private set; }
    [HideInInspector] public BaseStation selectedStation { get; private set; }
    [HideInInspector] public Collider ingredientCollider;
    [SerializeField] private GameObject ingredientInstanceHolder;
    
    // [Header("Ingredients Data")]
    [field: SerializeField] public Transform ingredientHoldPoint { get; private set; }// changing this for an array of 5 points, not deleted yet for testing
    public Ingredient ingredient { get; private set; }
    [HideInInspector] public int currentHoldPointIndex = 0; // keep track of the current HoldPoint index
    [HideInInspector] public int numberOfIngredientsHeld = 0; // Keep track of the number of ingredients held
    [HideInInspector] public int maxIngredients = 4; // Keep track of the maximum number of ingredients the player can have
    [SerializeField] public Transform[] ingredientHoldPoints; // Array to hold multiple ingredient

    [HideInInspector] public Rigidbody rb { get; private set; }
    [HideInInspector] public Vector3 curMoveInput;
    [HideInInspector] public Vector3 moveDir;

    //  Mess Data
    [HideInInspector] public MessBase selectedMess { get; private set; }
    [HideInInspector] public Collider messCollider;
    [field: SerializeField] public LayerMask isMessLayer { get; private set; }
    


    // When station selected
    private GameObject visualGameObject;

    // Components
    [HideInInspector] public InputManager inputManager;

    // UI player ingrerdien indicator
    [SerializeField] private TextMeshPro ingredientIndicatorText;
    private string currentIndicator;

    private void Awake()
    {
        if (Instance != null ) 
              
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();
        inputManager = FindObjectOfType<InputManager>();

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

        if (selectedMess)
        {
            selectedMess.Interact(this);
        }

    }

    public void InteractAlt(InputAction.CallbackContext ctx)
    {
        if (selectedStation)
        {
            selectedStation.InteractAlt(this);
        }
        if (selectedMess)
        {
            selectedMess.InteractAlt(this);
        }
    }


    public void SetSelectedStation(BaseStation baseStation)
    {
        selectedStation = baseStation;

    }
    public void SetSelectedMess(MessBase mess)
    {
        selectedMess = mess;

    }

    public int GetNumberOfIngredients()
    {
        int count = 0;
        foreach (Transform holdPoint in ingredientHoldPoints)
        {
            if (holdPoint.childCount > 0)
            {
                Ingredient ingredient = holdPoint.GetChild(0).GetComponent<Ingredient>();
                if (ingredient != null)
                {
                    count++;
                }
            }
        }
        numberOfIngredientsHeld = count;
        return numberOfIngredientsHeld;
    }

    public Transform GetNextHoldPoint()
    {
        for (int i = 0; i < ingredientHoldPoints.Length; i++)
        {
            currentHoldPointIndex = (currentHoldPointIndex + 1) % ingredientHoldPoints.Length;
            if (ingredientHoldPoints[currentHoldPointIndex].childCount == 0)
            {
                return ingredientHoldPoints[currentHoldPointIndex];
            }
        }

        // If all hold points are occupied, return null
        return null;
    }

    public Transform GetIngredientTransform()
    {
        return GetNextHoldPoint();
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
        for (int i = 0; i < ingredientHoldPoints.Length; i++)
        {
            Transform holdPoint = ingredientHoldPoints[i];
            if (holdPoint.childCount > 0)
            {
                
                // Detach the child (ingredient) from the hold point
                Transform ingredientTransform = holdPoint.GetChild(0);
                ingredientTransform.SetParent(null);

                // Enable the collider for the thrown ingredient
                Collider ingredientCollider = ingredientTransform.GetComponent<Collider>();
                if (ingredientCollider != null)
                {
                    ingredientCollider.enabled = true;
                }

                // Apply the throw force to the ingredient
                Rigidbody ingredientRb = ingredientTransform.GetComponent<Rigidbody>();
                if (ingredientRb != null)
                {
                    ingredientRb.isKinematic = false;
                    ingredientRb.AddForce(transform.forward * ingredienThrowForce, ForceMode.Impulse);
                }
                ingredientIndicatorText.SetText("");
                
            }
        }
    }

    public void GrabIngedientFromFloor(Ingredient floorIngredient,IngredientSO ingredientSO )
    {
        floorIngredient.SetIngredientParent(this);
        floorIngredient.DestroyIngredient();

        Transform nextHoldPoint = GetNextHoldPoint();
        if (nextHoldPoint != null)
        {
            Ingredient.SpawnIngredient(ingredientSO, this);
            GetNumberOfIngredients();
        }
        else
        {
            Debug.Log("Cannot grab more ingredients");
        }
       
    }


    public void ClearIngredient()
    {
        ingredient = null;
    }

    public void ClearIngredients()
    {
       
    }

    public bool HasIngredient()
    {
        return GetNumberOfIngredients() >= maxIngredients;
    }

    public void Show(GameObject visualGameObject)
    {
        visualGameObject.SetActive(true);
    }

    public void Hide(GameObject visualGameObject)
    {
        visualGameObject?.SetActive(false);
    }

    // Add ingredient name to UI indicator on player
    public void SetIngredientIndicator()
    {
        currentIndicator = null;
        foreach (Transform holdPoint in ingredientHoldPoints)
        {
            if (holdPoint.childCount > 0)
            {
                Ingredient ingredient = holdPoint.GetComponentInChildren<Ingredient>();
                currentIndicator += (ingredient.name + "\n");
            }
        }
        ingredientIndicatorText.text = currentIndicator;
    }


    // Mess Interface implementation

    public Transform GetMessTransform()
    {
        throw new NotImplementedException();
    }

    public void SetMess(MessBase mess)
    {
        this.selectedMess = mess;
    }

    public MessBase GetMess()
    {
        return selectedMess;
    }

    public void ClearMess()
    {
        selectedMess = null; ;
    }

    public bool HasMess()
    {
        return selectedMess != null;
    }
}
