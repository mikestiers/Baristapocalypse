using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IIngredientParent, IMessParent
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
    [SerializeField] private GameObject ingredientInstanceHolder;
    private BaseStation selectedStation;
    private Collider ingredientCollider;

    [Header("Ingredients Data")]
    [SerializeField] public Transform[] ingredientHoldPoints; // Array to hold multiple ingredient
    private Ingredient ingredient;
    private int currentHoldPointIndex = 0; // keep track of the current HoldPoint index
    private int numberOfIngredientsHeld = 0; // Keep track of the number of ingredients held
    private int maxIngredients = 4; // Keep track of the maximum number of ingredients the player can have

    [SerializeField] public Rigidbody rb { get; private set; }
    private Vector3 curMoveInput;
    private Vector3 moveDir;

    [Header("Mess Data")]
    [SerializeField] private LayerMask isMopLayer;
    [SerializeField] private LayerMask isMessLayer;
    [SerializeField] private MessSO spillPrefab;
    [SerializeField] private Transform spillSpawnPoint;
    private MessBase selectedMess;
    private Mop selectedMop;
    //private Collider messCollider;
    [HideInInspector] public bool hasMop;

    [field: SerializeField] public GameObject mopOnPlayer { get; private set; }

    // to organize
    private IngredientSO ingredienSO;
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

        //Set variables if null
        if (moveSpeed <= 0) moveSpeed = 10.0f;
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (dashForce <= 0) dashForce = 4000f;
        if (dashTime <= 0) dashTime = 0.1f;
        if (ingredienThrowForce <= 0) ingredienThrowForce = 10f;
        if (groundCheckRadius <= 0) groundCheckRadius = 0.05f;

        //Define the interactable layer mask to include station, ingredient, and mess layers.
        interactableLayerMask = isStationLayer | isIngredientLayer | isMessLayer | isMopLayer;

        RayCastOffset = new Vector3(0, 0.3f, 0);
    }

    private void OnEnable()
    {
        inputManager.JumpEvent += OnJump;
        inputManager.DashEvent += OnDash;
        inputManager.ThrowEvent += OnThrow;
        inputManager.InteractEvent += Interact;
        inputManager.InteractAltEvent += InteractAlt;
    }

    private void OnDisable()
    {
        inputManager.JumpEvent -= OnJump;
        inputManager.DashEvent -= OnDash;
        inputManager.ThrowEvent -= OnThrow;
        inputManager.InteractEvent -= Interact;
        inputManager.InteractAltEvent -= InteractAlt;
    }

    private void Update()
    {
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
            // Check the type of the hit object.
            // Logic for Station Interaction
            if (hit.transform.TryGetComponent(out BaseStation baseStation) && !hasMop)
            {
                visualGameObject = baseStation.transform.GetChild(0).gameObject;
                if (baseStation != selectedStation)
                {
                    SetSelectedStation(baseStation);
                    Show(visualGameObject);
                }
            }

            // Logic for Ingredient  on floor Interaction 
            else if (hit.transform.TryGetComponent(out Ingredient ingredient))
            {
                if (GetNumberOfIngredients() <= maxIngredients && !hasMop)
                {
                    ingredienSO = ingredient.IngredientSO;

                    if (mouse.leftButton.wasPressedThisFrame)
                    {
                        GrabIngedientFromFloor(ingredient, ingredienSO);
                    }
                    else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                    {
                        GrabIngedientFromFloor(ingredient, ingredienSO);
                    }
                }
            }

            // Logic for Mess Interaction
            else if (hit.transform.TryGetComponent(out MessBase mess))
            {
                visualGameObject = mess.transform.GetChild(0).gameObject;
                if (mess != selectedMess)
                {
                    SetSelectedMess(mess);
                    if (hasMop)
                    {
                        Show(visualGameObject);
                    }
                }
                Debug.Log("Detecting " + mess);
            }
            // logic for Mop Interaction
            else if (hit.transform.TryGetComponent(out Mop Mop))
            {
                if (Mop != selectedMop)
                {
                    SetSelectedMop(Mop);
                }
                Debug.Log("Geting " + Mop);
            }
        }
        else
        {
            // No interactable object hit, clear selected objects.
            if (visualGameObject)
            {
                Hide(visualGameObject);
            }
            SetSelectedStation(null);
            SetSelectedMess(null);
            SetSelectedMop(null);
        }
        Debug.DrawRay(transform.position + RayCastOffset, transform.forward, Color.green);
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
   
    public void Interact()
    {
        if (selectedStation)
        {
            selectedStation.Interact(this);
        }
      
        if (selectedMess)
        {
            selectedMess.Interact(this);
        }
    }

    public void InteractAlt()
    {
        if (selectedStation)
        {
            selectedStation.InteractAlt(this);
        }
        if (selectedMess)
        {
            selectedMess.InteractAlt(this);
        }
        if (selectedMop) 
        { 
            selectedMop.InteractAlt(this);
        }
    }

    public void OnJump()
    {
        // Jump logic if we want jumping
    }

    public void OnDash()
    {
        StartCoroutine(Dash());
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.dash);

        if (GetNumberOfIngredients() > 0)
        {
            if (CheckIfHoldingLiquid() > 0)//stateMachine.ingredient.GetIngredientSO().objectTag == "Milk")
            {
                MessBase.SpawnMess(spillPrefab, spillSpawnPoint);
                ThrowIngedient();
            }
        }
        else return;
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        Debug.Log("dash");

        while (Time.time < startTime + dashTime)
        {
            rb.AddForce(inputManager.moveDir * dashForce * Time.deltaTime, ForceMode.Acceleration);
            yield return null;
        }
    }

    public void OnThrow()
    {
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
    public void SetSelectedMess(MessBase mess)
    {
        selectedMess = mess;

    }

    public void SetSelectedMop(Mop mop) 
    { 
       selectedMop = mop;
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

    // Ingredient intarface Implementation
    public void ClearIngredient()
    {
        ingredient = null;
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
        return selectedMess.transform;
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
