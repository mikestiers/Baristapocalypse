using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour, IIngredientParent
{
    // Player Instance
    [HideInInspector] public static PlayerController Instance { get; private set; }

    [Header("Player Attributes")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float ingredienThrowForce;

    [Header("Ground Check")]
    [SerializeField] private LayerMask isGroundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashTime;
    private bool isGrounded;

    [Header("Interactables")]
    [SerializeField] private LayerMask isStationLayer;
    [SerializeField] private LayerMask isIngredientLayer;
    [SerializeField] private LayerMask isCustomerLayer;
    //[SerializeField] private GameObject ingredientInstanceHolder;
    private BaseStation selectedStation;
    private Base selectedCustomer;
    public float sphereCastRadius = 0.5f;
    //private Collider ingredientCollider;

    [Header("Ingredients Data")]
    [SerializeField] public Transform[] ingredientHoldPoints; // Array to hold multiple ingredient
    [SerializeField] private List<Ingredient> ingredientsList = new List<Ingredient>();
    private int currentHoldPointIndex = 0; // keep track of the current HoldPoint index
    private int numberOfIngredientsHeld = 0; // Keep track of the number of ingredients held
    private int maxIngredients = 2; // Keep track of the maximum number of ingredients the player can have

    [SerializeField] public Rigidbody rb { get; private set; }
    [SerializeField] public Animator anim { get; private set; }
    private Vector3 curMoveInput;
    private Vector3 moveDir;

    [Header("Mess Data")]
    [SerializeField] private LayerMask isMopLayer;
    [SerializeField] private LayerMask isMessLayer;
    [SerializeField] private MessSO spillPrefab;
    [SerializeField] private Transform spillSpawnPoint;

    [Header("Pickups")]
    public Transform pickupLocation;
    public float pickupThrowForce;

    // Testing Spawnpoints
    public Transform spawnpoint1;
    public Transform spawnpoint2;
    public Transform spawnpoint3;
    public Transform spawnpoint4;

    [HideInInspector]
    public Pickup Pickup
    {
        get
        {
            if (IsHoldingPickup)
                return pickupLocation.GetChild(0).GetComponent<Pickup>();
            return null;
        }
    }

    // to organize
    private IngredientSO ingredientSO;
    public bool HasNoIngredients => GetNumberOfIngredients() == 0;
    private Mouse mouse = Mouse.current;
    private LayerMask interactableLayerMask; // A single LayerMask for all interactable objects
    private Vector3 RayCastOffset; // Temp for raising Raycast poin of origin

    // When station selected
    private GameObject visualGameObject;

    [Header("Other Components")]
    [SerializeField] private InputManager inputManager;

    // UI player ingrerdien indicator
    [SerializeField] private TextMeshPro ingredientIndicatorText;
    private string currentIndicator;

    private void Awake()
    {
        if (Instance != null) { Instance = this; }

        if (!inputManager) { inputManager = GetComponent<InputManager>(); }
    }

    private void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        //Set variables if null
        if (moveSpeed <= 0) moveSpeed = 10.0f;
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (dashForce <= 0) dashForce = 4000f;
        if (dashTime <= 0) dashTime = 0.1f;
        if (ingredienThrowForce <= 0) ingredienThrowForce = 10f;
        if (groundCheckRadius <= 0) groundCheckRadius = 0.05f;

        //Define the interactable layer mask to include station, ingredient, and mess layers.
        interactableLayerMask = isStationLayer | isIngredientLayer | isMessLayer | isMopLayer | isCustomerLayer ;

        RayCastOffset = new Vector3(0, 0.4f, 0);
    }

    private void OnEnable()
    {
        //inputManager.JumpEvent += OnJump;
        inputManager.DashEvent += OnDash;
        inputManager.ThrowEvent += OnThrow;
        inputManager.InteractEvent += Interact ;
        inputManager.InteractAltEvent += InteractAlt;
        inputManager.DebugConsoleEvent += ShowDebugConsole;
    }

    private void OnDisable()
    {
        //inputManager.JumpEvent -= OnJump;
        inputManager.DashEvent -= OnDash;
        inputManager.ThrowEvent -= OnThrow;
        inputManager.InteractEvent -= Interact;
        inputManager.InteractAltEvent -= InteractAlt;
        inputManager.DebugConsoleEvent -= ShowDebugConsole;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        // Ground Check
        IsGrounded();
        // player movement
        Move(moveSpeed);

        GetNumberOfIngredients();
        SetIngredientIndicator();

        // Perform a single raycast to detect any interactable object.
        float interactDistance = 2.0f;
        if (Physics.Raycast(transform.position + RayCastOffset, transform.forward, out RaycastHit hit, interactDistance, interactableLayerMask))
        {
            // Logic for PickUp Interaction
            if (hit.transform.TryGetComponent(out Pickup pickup))
            {
                if (mouse.rightButton.wasPressedThisFrame)
                    this.DoPickup(pickup);
                else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                {
                    this.DoPickup(pickup);
                }

            }
            // Logic for Station Interaction
            if (hit.transform.TryGetComponent(out BaseStation baseStation))
            {
                visualGameObject = baseStation.transform.GetChild(0).gameObject;
                if (baseStation != selectedStation)
                {
                    SetSelectedStation(baseStation);
                    Show(visualGameObject);
                }
            }
            else if (hit.transform.TryGetComponent(out Spill spill))
            {
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    spill.Interact(this);
                }
                else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                {
                    spill.Interact(this);
                }
            }

            // Logic for Ingredient  on floor Interaction 
            else if (hit.transform.TryGetComponent(out Ingredient ingredient))
            {
                if (GetNumberOfIngredients() <= maxIngredients && !IsHoldingPickup)
                {
                    ingredientSO = ingredient.IngredientSO;

                    if (mouse.leftButton.wasPressedThisFrame)
                    {
                        GrabIngredientFromFloor(ingredient, ingredientSO);
                    }
                    else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                    {
                        GrabIngredientFromFloor(ingredient, ingredientSO);
                    }
                }
            }
        }
        else
        {
            // No interactable object hit, clear selected objects.
            SetSelectedStation(null);
            //Hide(visualGameObject);
        }

        // Customer Interaction Logic
        float customerInteractDistance = 4.0f;
        //if (Physics.Raycast(transform.position + RayCastOffset, transform.forward, out RaycastHit hitCustomer, customerInteractDistance, interactableLayerMask))
        if (Physics.SphereCast(transform.position + RayCastOffset, sphereCastRadius, transform.forward, out RaycastHit hitCustomer, customerInteractDistance, interactableLayerMask))
        {
            if (hitCustomer.transform.TryGetComponent(out Base customerBase))
            {
                

                visualGameObject = customerBase.transform.GetChild(0).gameObject;
                if (customerBase != selectedCustomer)
                {
                    SetSelectedCustomer(customerBase);
                    //Show(visualGameObject);
                }
            }
        }
        else
        {
            // No interactable object hit, clear selected objects.
            SetSelectedCustomer(null);
            //Hide(visualGameObject);
        }

        Debug.DrawRay(transform.position + RayCastOffset, transform.forward, Color.green);
        Debug.DrawRay(transform.position + RayCastOffset, transform.forward * customerInteractDistance, Color.red);
        
    }



    public bool IsGrounded()
    {
        isGrounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, isGroundLayer).Length > 0;
        return isGrounded;
    }

    public void Move(float moveSpeed)
    {
        if (inputManager.moveDir == Vector3.zero)
        {
            anim.SetFloat("vertical", 0);
            anim.SetFloat("horizontal", 0);
            return;
        }
        
        curMoveInput = inputManager.moveDir * moveSpeed * Time.deltaTime;
        transform.forward = inputManager.moveDir;

        // Check movement direction
        float forwardDot = Vector3.Dot(inputManager.moveDir, transform.right);
        float rightDot = Vector3.Dot(inputManager.moveDir, transform.forward);
        anim.SetFloat("vertical", forwardDot);
        anim.SetFloat("horizontal", rightDot);

        rb.MovePosition(rb.position + curMoveInput);
    }


    public void Interact()
    {
        if (SceneManager.GetActiveScene().name == Loader.Scene.CharacterSelectScene.ToString()) return;
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (selectedStation)
        {
            selectedStation.Interact(this);

        }

        if (selectedCustomer)
        {
            selectedCustomer.Interact(this);
        }
    }

    public void InteractAlt()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (selectedStation)
        {
            selectedStation.InteractAlt(this);
        }
    }

   //public void OnJump()
   //{
   //    // Jump logic if we want jumping
   //}

    public void OnDash()
    {
        StartCoroutine(Dash());
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.dash);

        if (GetNumberOfIngredients() > 0)
        {
            if (CheckIfHoldingLiquid() > 0)//stateMachine.ingredient.GetIngredientSO().objectTag == "Milk")
            {
                Instantiate(spillPrefab.prefab, spillSpawnPoint.position, Quaternion.identity);
                ThrowIngedient();
            }
        }
        else return;
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            rb.AddForce(inputManager.moveDir * dashForce * Time.deltaTime, ForceMode.Acceleration);
            yield return null;
        }
    }

    public void OnThrow()
    {
        if (IsHoldingPickup)
        {
            ThrowPickup();
        }
        if (GetNumberOfIngredients() > 0)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.throwIngredient);
            ThrowIngedient();

            numberOfIngredientsHeld = 0;

        }
    }

    public void SetSelectedStation(BaseStation baseStation)
    {
        selectedStation = baseStation;

    }

    public void SetSelectedCustomer(Base customer)
    {
        selectedCustomer = customer;
    }

    public int GetNumberOfIngredients()
    {
        numberOfIngredientsHeld = ingredientsList.Count;
        return numberOfIngredientsHeld;
    }

    public int CheckIfHoldingLiquid()
    {
        int count = 0;
        foreach (Transform holdPoint in ingredientHoldPoints)
        {
            if (holdPoint.childCount > 0)
            {
                Ingredient ingredient = holdPoint.GetChild(0).GetComponent<Ingredient>();
                
                if (ingredient.GetIngredientSO().objectTag == "Milk")
                {
                    count++;
                }
            }
        }
        return count;

    }

    public Transform GetNextHoldPoint()
    {
        if(GetNumberOfIngredients() >= GetMaxIngredients())
        {
            return null;
        }
        return ingredientHoldPoints[GetNumberOfIngredients()];
    }

    public Transform GetIngredientTransform()
    {
        return GetNextHoldPoint();
    }

    public void SetIngredient(Ingredient ingredient)
    {
        if(GetNumberOfIngredients() < GetMaxIngredients())
        {
            ingredientsList.Add(ingredient);
        }
    }

    public Ingredient GetIngredient()
    {
        return ingredientsList[0];
    }

    public int GetMaxIngredients()
    {
        return maxIngredients;
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

    public void GrabIngredientFromFloor(Ingredient floorIngredient, IngredientSO ingredientSO)
    {
        if (IsHoldingPickup)
            return;
        floorIngredient.SetIngredientParent(this);
        Ingredient.DestroyIngredient(floorIngredient);

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

    // Ingredient intarface Implementation
    public void ClearIngredient()
    {
        ingredientsList.Clear();
    }

    public bool HasIngredient()
    {
        return GetNumberOfIngredients() > 0;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
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
    public bool IsHoldingPickup => pickupLocation.childCount > 0;
    public void DoPickup(Pickup pickup)
    {
        if (IsHoldingPickup || !HasNoIngredients)
            return;

        Pickup p = Instantiate(pickup, pickupLocation) as Pickup;

        if (p.IsCustomer)
        {
            p.GetNavMeshAgent().enabled = false;
            p.GetCustomer().SetCustomerStateServerRpc(CustomerBase.CustomerState.PickedUp);
        }

        p.RemoveRigidBody();
        p.transform.localRotation = Quaternion.Euler(p.holdingRotation);
        p.transform.localPosition = p.holdingPosition;
        p.GetCollider().enabled = false;
        Destroy(pickup.gameObject);
    }

    public void ThrowPickup()
    {
        if (pickupLocation.childCount == 0)
            return;

        Pickup p = pickupLocation.GetChild(0).GetComponent<Pickup>();

        if (p.IsCustomer)
        {
            p.GetCustomer().Dead();
        }

        p.transform.SetParent(null);
        p.GetCollider().enabled = true;
        p.AddRigidbody();
        p.transform.GetComponent<Rigidbody>().AddForce(transform.forward * (pickupThrowForce * p.GetThrowForceMultiplier()));
    }

    // temp for debugging 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 4, sphereCastRadius);
    }

    public void ShowDebugConsole()
    {
        UIManager.Instance.debugConsole.SetActive(!UIManager.Instance.debugConsoleActive);
        UIManager.Instance.debugConsoleActive = !UIManager.Instance.debugConsoleActive;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Instance = this;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == OwnerClientId && HasIngredient()) // HasIngredient 
        {
            Ingredient.DestroyIngredient(GetIngredient());
        }
    }
}
