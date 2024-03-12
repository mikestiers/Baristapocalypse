using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using Cinemachine;
using System.Linq;
using static AISupervisor;
//using UnityEditor.ShaderGraph;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour, IIngredientParent, IPickupObjectParent, ISpill
{
    // Player Instance
    [HideInInspector] public static PlayerController Instance { get; private set; }

    [Header("Player Attributes")]
    public List<GameObject> bootsParticles = new List<GameObject>();
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravityMoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float ingredientThrowForce;

    [Header("Ground Check")]
    [SerializeField] private LayerMask isGroundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldownTime;
    private bool isDashing = false;
    private bool isGrounded;

    [Header("Interactables")]
    [SerializeField] private LayerMask isStationLayer;
    [SerializeField] private LayerMask isIngredientLayer;
    [SerializeField] private LayerMask isCustomerLayer;
    [SerializeField] private float stationsSphereCastRadius;
    [SerializeField] private float customersSphereCastRadius;
    [SerializeField] private float stationInteractDistance;
    [SerializeField] private float customerInteractDistance;
    [SerializeField] private GameObject InteractzoneStart;
    [SerializeField] private BrewingStation brewingStation1;
    [SerializeField] private BrewingStation brewingStation2;
    private Spill selectedSpill;

    private BaseStation selectedStation;
    private Base selectedCustomer;
    public int currentBrewingStation = 0;
    private BrewingStation interactingBrewingStation;

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
    private Vector3 moveDirection;
    public float rotationSpeed;

    [Header("Mess Data")]
    [SerializeField] private LayerMask isMopLayer;
    [SerializeField] private LayerMask isMessLayer;
    [SerializeField] private LayerMask isGravityAffectedLayer;
    [SerializeField] private MessSO spillPrefab;
    [SerializeField] private Transform spillSpawnPoint;
    [SerializeField] private Spill spill;

    [Header("Pickups")]
    [SerializeField] public Transform pickupLocation;
    [SerializeField] private Pickup pickup;
    public float pickupThrowForce;
    private string mopSoName = "Mop";
    private string messCupSoName = "MessCup";

    // Animations
    private readonly int MovementWithCupHash = Animator.StringToHash("MovementWithCup");
    private readonly int MovementHash = Animator.StringToHash("Movement");
    private readonly int MovementWithCustomerHash = Animator.StringToHash("MovementWithCustomer");
    private readonly int MovementWithVacHash = Animator.StringToHash("MovementWithVac");
    private readonly int BP_Barista_Floor_PickupHash = Animator.StringToHash("BP_Barista_Floor_Pickup");
    private readonly int BP_Barista_Pickup_VacHash = Animator.StringToHash("BP_Barista_Pickup_Vac");
    private readonly int BP_Barista_Pickup_CustHash = Animator.StringToHash("BP_Barista_Pickup_Cust");
    private readonly int BP_Barista_Throw_CupHash = Animator.StringToHash("BP_Barista_Throw_Cup");
    private readonly int BP_Barista_Throw_CustHash = Animator.StringToHash("BP_Barista_Throw_Cust");
    private readonly int BP_Barista_Cleaning_VacHash = Animator.StringToHash("BP_Barista_Cleaning_Vac");

    private bool isAnimating = false;

    private const float CrossFadeDuration = 0.1f;

    private CinemachineVirtualCamera virtualCamera;
    public PlayerColorChoice playerVisual;
    private bool tutorialMessageActive = false;

    [HideInInspector]
    public Pickup Pickup
    {
        get
        {
            if (HasPickup())
                return pickup;
            return null;
        }
    }

    // to organize
    private IngredientSO ingredientSO;
    private PickupSO pickupSo;
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

    // Toggles
    public bool movementToggle = true;

    private void Awake()
    {
        if (Instance != null) { Instance = this; }

        if (!inputManager) { inputManager = GetComponent<InputManager>(); }
    }

    private void Start()
    {
        if (IsOwner && SceneManager.GetActiveScene().name == Loader.Scene.T5M3_BUILD.ToString())
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            virtualCamera.Follow = gameObject.transform;
            virtualCamera.LookAt = gameObject.transform;
        }

        //Get components
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        //Set variables if null
        if (moveSpeed <= 0) moveSpeed = 7.5f;
        if (gravityMoveSpeed <= 0) gravityMoveSpeed = 4.0f;
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (dashForce <= 0) dashForce = 130.0f;
        if (dashTime <= 0) dashTime = 0.1f;
        if (dashCooldownTime <= 0) dashCooldownTime = 1.0f;
        if (ingredientThrowForce <= 0) ingredientThrowForce = 10f;
        if (groundCheckRadius <= 0) groundCheckRadius = 0.05f;
        if (stationsSphereCastRadius <= 0) stationsSphereCastRadius = 0.5F;
        if (customersSphereCastRadius <= 0) customersSphereCastRadius = 1.0F;
        if (stationInteractDistance <= 0) stationInteractDistance = 1.5F;
        if (customerInteractDistance <= 0) customerInteractDistance = 5.0F;
        if (rotationSpeed <= 0) rotationSpeed = 10.0F;

        //Define the interactable layer mask to include station, ingredient, and mess layers.
        interactableLayerMask = isStationLayer | isIngredientLayer | isMessLayer | isMopLayer | isCustomerLayer | isGravityAffectedLayer;

        RayCastOffset = new Vector3(0, 0.4f, 0);
        // Set color of the player based on color selection at the lobby
        PlayerData playerData = BaristapocalypseMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(BaristapocalypseMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        SetSelectedSpill(null);
    }

    private void OnEnable()
    {
        inputManager.DashEvent += OnDash;
        inputManager.ThrowEvent += OnThrow;
        inputManager.InteractEvent += Interact;
        inputManager.InteractAltEvent += InteractAlt;
        inputManager.DebugConsoleEvent += ShowDebugConsole;
        inputManager.BrewingStationSelect1Event += OnChangeBrewingStationSelect1;
        inputManager.BrewingStationSelect2Event += OnChangeBrewingStationSelect2;
        inputManager.BrewingStationEmptyEvent += OnBrewingStationEmptyServerRpc;

        if (AISupervisor.Instance)
        {
            AISupervisor.Instance.OnTutorialMessageReceived += TutorialMessage;
        }
    }

    private void OnDisable()
    {
        inputManager.DashEvent -= OnDash;
        inputManager.ThrowEvent -= OnThrow;
        inputManager.InteractEvent -= Interact;
        inputManager.InteractAltEvent -= InteractAlt;
        inputManager.DebugConsoleEvent -= ShowDebugConsole;
        inputManager.BrewingStationSelect1Event -= OnChangeBrewingStationSelect1;
        inputManager.BrewingStationSelect2Event -= OnChangeBrewingStationSelect2;
        inputManager.BrewingStationEmptyEvent -= OnBrewingStationEmptyServerRpc;

        if (brewingStation1 != null)
            brewingStation1.animationSwitch -= OnAnimationSwitch;
        if (brewingStation2 != null) 
            brewingStation2.animationSwitch -= OnAnimationSwitch;

        if (AISupervisor.Instance)
        {
            AISupervisor.Instance.OnTutorialMessageReceived -= TutorialMessage;
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name != Loader.Scene.T5M3_BUILD.ToString()) { return; }

        // Ground Check
        IsGrounded();
        // player movement

        // Gravity Storm Effect on player
        if (movementToggle && !GameManager.Instance.isGravityStorm.Value)
            Move(moveSpeed);
        else if (movementToggle && GameManager.Instance.isGravityStorm.Value)
            Move(gravityMoveSpeed);

        if (!movementToggle)
        {
            anim.SetFloat("vertical", 0);
            anim.SetFloat("horizontal", 0);
            return;
        }

        // Spherecast Interactions
        if (Physics.SphereCast(InteractzoneStart.transform.position + RayCastOffset, stationsSphereCastRadius, InteractzoneStart.transform.forward, out RaycastHit hit, 
                stationInteractDistance, interactableLayerMask))
        {
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
        }
        else
        {
            // No interactable object hit, clear selected objects.
            SetSelectedStation(null);
        }
        
        // Perform a single SphereCast to detect any interactable object on the floor.
        if (Physics.SphereCast(transform.position + RayCastOffset, stationsSphereCastRadius, transform.forward, out RaycastHit floorHit, stationInteractDistance, interactableLayerMask))
        {
            // Logic for PickUp Interaction
            if (floorHit.transform.TryGetComponent(out Pickup pickup))
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    DoPickup(pickup);
                }
                else if (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame)
                {
                    DoPickup(pickup);
                }
            }
            else if (floorHit.transform.TryGetComponent(out Spill spill))
            {
                //bool _hasMop = false;
                if (Pickup)
                {
                    if (Pickup.attributes.Contains(Pickup.PickupAttribute.CleansUpSpills)) 
                    {
                        //_hasMop = true;
                        if(selectedSpill == null) SetSelectedSpill(spill);
                        selectedSpill.ShowUi();
                        if(Keyboard.current.qKey.wasPressedThisFrame)
                        {
                            DoMop(selectedSpill);
                        }
                        else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                        {
                            DoMop(selectedSpill);
                        }
                    } 
                }
                //else _hasMop = false;

            }
        
            // Logic for Ingredient on floor Interaction 
            else if (floorHit.transform.TryGetComponent(out Ingredient ingredient))
            {
                if (GetNumberOfIngredients() <= GetMaxIngredients() && !HasPickup())
                {
                    IngredientSO ingredientSORef = ingredient.GetIngredientSO();
        
                    if (mouse.leftButton.wasPressedThisFrame)
                    {
                        GrabIngredientFromFloor(ingredient, ingredientSORef);
                    }
                    else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                    {
                        GrabIngredientFromFloor(ingredient, ingredientSORef);
                    }
                }
            }
        }
        else
        {
            if (selectedSpill != null)
            {
                selectedSpill.HideUi();
                OnAnimationSwitch();
            }
           
            // No interactable object hit, clear selected objects.
            SetSelectedSpill(null);
            SetSelectedStation(null);
            //Hide(visualGameObject);
        }

        // Customer Interaction Logic
        if (Physics.SphereCast(InteractzoneStart.transform.position + RayCastOffset, customersSphereCastRadius, InteractzoneStart.transform.forward, out RaycastHit hitCustomer, customerInteractDistance, interactableLayerMask))
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

        // Check if the inputDevice has changed
        //HandleMouseVisibility();

        //Debug.LogWarning("HasPickup() " + HasPickup());
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

        //curMoveInput = inputManager.moveDir * moveSpeed * Time.deltaTime; // movement does not take camera position into calculation so the map has to be rotated in a certain way
        //curMoveInput = Camera.main.transform.forward * inputManager.moveDir.z * moveSpeed * Time.deltaTime;
        //curMoveInput += Camera.main.transform.right * inputManager.moveDir.x * moveSpeed * Time.deltaTime;

        Vector3 moveInput = new Vector3(inputManager.moveDir.x, 0, inputManager.moveDir.z);
        moveDirection = Camera.main.transform.TransformDirection(moveInput);
        moveDirection.y = 0; // stay grounded
        Vector3 curMoveInput = moveDirection.normalized * moveSpeed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            //transform.forward = moveDirection.normalized; // Orient the character to face the direction of movement
            // Calculate the target rotation based on the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);

            // Interpolate between the current rotation and the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        rb.MovePosition(rb.position + curMoveInput);

        //transform.forward = inputManager.moveDir;

        // Check movement direction
        float forwardDot = Vector3.Dot(inputManager.moveDir, transform.right);
        float rightDot = Vector3.Dot(inputManager.moveDir, transform.forward);
        //float forwardDot = Vector3.Dot(inputManager.moveDir, Camera.main.transform.forward);
        //float rightDot = Vector3.Dot(inputManager.moveDir, Camera.main.transform.right);
        anim.SetFloat("vertical", forwardDot);
        anim.SetFloat("horizontal", rightDot);

        //rb.MovePosition(rb.position + curMoveInput);
    }

    public void Interact()
    {
        if (SceneManager.GetActiveScene().name == Loader.Scene.CharacterSelectScene.ToString()) return;
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (!IsOwner) return;

        if (selectedStation)
        {
            selectedStation.Interact(this);
        }

        if (selectedCustomer)
        {
            selectedCustomer.Interact(this);
        }

        if (tutorialMessageActive)
        {
            tutorialMessageActive = false;
            Time.timeScale = 1.0f;
        }

    }

    public void InteractAlt()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (!IsLocalPlayer) return;

        if (selectedStation)
        {
            selectedStation.InteractAlt(this);
        }
    }

    public void OnDash()
    {
        if (!IsLocalPlayer) return;
        if (isDashing) return;

        if (movementToggle)
            StartCoroutine(Dash());
        //SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.dash);
        //Instantiate(spillPrefab.prefab, spillSpawnPoint.position, Quaternion.identity);

        // left for testing just incase we need to change something
        if (GetNumberOfIngredients() > 0)
        {
            if (CheckIfHoldingLiquid() > 0)//stateMachine.ingredient.GetIngredientSO().objectTag == "Milk")
            {
                if (spillPrefab != null)
                {
                    Spill.CreateSpill(spillPrefab, this);
                }
                else
                {
                    Debug.Log("MessSO is null");
                }
            }
        }
        else return;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            rb.AddForce(moveDirection * dashForce * Time.deltaTime, ForceMode.Impulse);
            if (ingredientsList.Count > 0  || HasPickup())
            {
                anim.SetBool("isDashingWithCup", isDashing);
            }
            else
            {
                anim.SetBool("isDashing", isDashing);
            }
            yield return null;
        }

        yield return new WaitForSeconds(dashCooldownTime);

        isDashing = false;
        if (ingredientsList.Count > 0 || HasPickup())
        {
            anim.SetBool("isDashingWithCup", isDashing);
        }
        else
        {
            anim.SetBool("isDashing", isDashing);
        }

    }

    public void OnThrow()
    {
        if (!IsLocalPlayer) return;
        if (HasPickup())
        {
            ThrowPickup();
        }
        if (GetNumberOfIngredients() > 0)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.throwIngredient);
            ThrowIngredient();
        }
    }

    public void SetSelectedStation(BaseStation baseStation)
    {
        selectedStation = baseStation;
        if (!brewingStation1)
        {
            brewingStation1 = FindObjectOfType<BrewingStation>();
            brewingStation1.animationSwitch += OnAnimationSwitch;
        }
        if (!brewingStation2)
        {
            brewingStation2 = FindObjectOfType<BrewingStation>();
            brewingStation2.animationSwitch += OnAnimationSwitch;
        }
    }

    public void SetSelectedSpill(Spill spill)
    {
        selectedSpill = spill;
    }
    public void SetSelectedCustomer(Base customer)
    {
        selectedCustomer = customer;
    }

    public int GetNumberOfIngredients()
    {
        UpdateNumberOfIngredients();
        return numberOfIngredientsHeld;
    }

    public void UpdateNumberOfIngredients()
    {
        numberOfIngredientsHeld = ingredientsList.Count;
    }

    public int CheckIfHoldingLiquid()
    {
        int count = 0;
        foreach (Ingredient i in ingredientsList)
        {
            if (i.GetIngredientSO().objectTag == "CoffeeCup")
            {
                count++;
            }
        }
        return count;
    }

    public Transform GetNextHoldPoint()
    {
        if (GetNumberOfIngredients() > GetMaxIngredients())
        {
            return null;
        }
        Debug.Log("getting hold points " + (numberOfIngredientsHeld - 1));
        return ingredientHoldPoints[GetNumberOfIngredients() - 1];
    }

    public Transform GetIngredientTransform()
    {
        return GetNextHoldPoint();
    }

    public void SetIngredient(Ingredient ingredient)
    {
        if (GetNumberOfIngredients() < GetMaxIngredients())
        {
            ingredientsList.Add(ingredient);
            GetNumberOfIngredients();
            SetIngredientIndicator();
        }
    }

    public Ingredient GetIngredient()
    {
        return ingredientsList[0];
    }

    public List<Ingredient> GetIngredientsList()
    {
        return ingredientsList;
    }

    public int GetMaxIngredients()
    {
        return maxIngredients;
    }

    public void ThrowIngredient()
    {
        ThrowIngredientServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowIngredientServerRpc()
    {
        ThrowIngredientClientRpc();
    }

    [ClientRpc]
    private void ThrowIngredientClientRpc()
    {
        if (isAnimating == true) return;
        isAnimating = true;
        StartCoroutine(ThrowIngredientAnimation()); //Play throw ingredient
    }

    public void GrabIngredientFromFloor(Ingredient floorIngredient, IngredientSO ingredientSO)
    {
        if (HasPickup())
            return;
        if (GetNumberOfIngredients() >= GetMaxIngredients())
        {
            Debug.Log("max ingredients spawned!");
            return;
        }

        Ingredient.DestroyIngredient(floorIngredient);
        Ingredient.SpawnIngredient(ingredientSO, this);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
    }

    // Ingredient intarface Implementation
    public void ClearIngredient()
    {
        //ingredientsList.Clear();
    }

    public void RemoveIngredientInListAtIndex(int index)
    {
        ingredientsList.RemoveAt(index);
        if (ingredientsList.Count > 0)
        {
            ingredientsList.Insert(0, ingredientsList[0]);
            ingredientsList.RemoveAt(1);
            ingredientsList[0].followTransform.SetTargetTransform(ingredientHoldPoints[0]);
        }
        SetIngredientIndicator();
    }

    public void RemoveIngredientInListByReference(Ingredient ingredient)
    {
        ingredientsList.Remove(ingredient);
        if (ingredientsList.Count > 0)
        {
            ingredientsList.Insert(0, ingredientsList[0]);
            ingredientsList.RemoveAt(1);
            ingredientsList[0].followTransform.SetTargetTransform(ingredientHoldPoints[0]);
        }

        SetIngredientIndicator();
    }

    public bool HasIngredient()
    {
        return GetNumberOfIngredients() > 0;
    }

    public Transform GetPickupTransform()
    {
        return pickupLocation;
    }

    public void SetPickup(Pickup pickup)
    {
        this.pickup = pickup;
    }

    public Pickup GetPickup()
    {
        return pickup;
    }

    public void ClearPickup()
    {
        this.pickup = null;
        pickupSo = null;
    }

    public bool HasPickup()
    {
        return pickup != null;
    }

    public Transform GetSpillTransform()
    {
        return spillSpawnPoint;
    }

    public void SetSpill(Spill spill)
    {
        this.spill = spill;
    }

    public void ClearSpill()
    {
        spill = null;
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
        SetIngredientIndicatorServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetIngredientIndicatorServerRpc()
    {
        SetIngredientIndicatorClientRpc();
    }

    [ClientRpc]
    private void SetIngredientIndicatorClientRpc()
    {
        currentIndicator = null;
        foreach (Ingredient i in ingredientsList)
        {
            if (i != null)
            {
                currentIndicator += (i.name + "\n");
            }
        }
        ingredientIndicatorText.text = currentIndicator;
    }

    public void DoMop(Spill spill)
    {
        if(!HasNoIngredients)return;
        anim.CrossFadeInFixedTime(BP_Barista_Cleaning_VacHash, CrossFadeDuration);
        spill.Interact(this);
  
    }

    public void DoPickup(Pickup pickup)
    {
        if (HasPickup() || !HasNoIngredients || isAnimating == true)
            return;

        isAnimating = true;
        PickupSO pickupSo = pickup.GetPickupObjectSo();

        if (pickupSo != null)
        {
            StartCoroutine(PickUpAnimation(pickup)); // Play trash pick up and set trash parent
        }
        else if (pickup.IsCustomer && pickup.GetCustomer().GetCustomerState() == CustomerBase.CustomerState.Loitering)
        {
            StartCoroutine(PickUpCustomerAnimation(pickup));
        }
    }

    public void ThrowPickup()
    {
        ThrowPickupServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowPickupServerRpc()
    {
        ThrowPickupClientRpc();
    }

    [ClientRpc]
    private void ThrowPickupClientRpc()
    {
        if (!HasPickup() || isAnimating == true)
            return;

        isAnimating = true;
        if (pickup.IsCustomer)
        {
            Debug.Log("Customer dead");
            pickup.GetCustomer().Dead();
            //pickup.AddRigidbody();
            pickup.GetComponentInChildren<Rigidbody>().isKinematic = false;
        }

        StartCoroutine(ThrowPickUpAnimation());
    }

    public void ShowDebugConsole()
    {
        UIManager.Instance.debugConsole.SetActive(!UIManager.Instance.debugConsoleActive);
        UIManager.Instance.debugConsoleActive = !UIManager.Instance.debugConsoleActive;
    }

    public void OnChangeBrewingStationSelect1()
    {
        if (OrderManager.Instance.brewingStations.Length > 1)
        {
            // Increment the currentBrewingStation index, wrapping around using modulo
            //int nextBrewingStation = (currentBrewingStation + 1) % OrderManager.Instance.brewingStations.Length;
            OrderManager.Instance.orderStats[0].selectedByPlayerImage.SetActive(true);
            OrderManager.Instance.orderStats[1].selectedByPlayerImage.SetActive(false);
            currentBrewingStation = 0;
        }
    }
    
    public void OnChangeBrewingStationSelect2()
    {
        if (OrderManager.Instance.brewingStations.Length > 1)
        {
            // Increment the currentBrewingStation index, wrapping around using modulo
            //int nextBrewingStation = (currentBrewingStation + 1) % OrderManager.Instance.brewingStations.Length;
            OrderManager.Instance.orderStats[1].selectedByPlayerImage.SetActive(true);
            OrderManager.Instance.orderStats[0].selectedByPlayerImage.SetActive(false);
            currentBrewingStation = 1;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnBrewingStationEmptyServerRpc()
    {
        if (OrderManager.Instance.brewingStations[currentBrewingStation].ingredientSOList.Count > 0 && OrderManager.Instance.brewingStations[currentBrewingStation].canEmptyBrewingStation.Value)
        {
            AISupervisor.Instance.SupervisorMessageToDisplay("Throwing away product? I'm taking that out of your tips!");
            GameManager.Instance.moneySystem.AdjustMoneyByAmount(3, false);
            OrderManager.Instance.brewingStations[currentBrewingStation].Empty();
            OrderManager.Instance.orderStats[currentBrewingStation].ResetAll();
            OrderManager.Instance.brewingStations[currentBrewingStation].canEmptyBrewingStation.Value = false;
            StartCoroutine(CanEmptyBrewingStation());
        }
    }

    private IEnumerator CanEmptyBrewingStation()
    {
        yield return new WaitForSeconds(OrderManager.Instance.brewingStations[currentBrewingStation].brewingStationEmptyCooldown);
        OrderManager.Instance.brewingStations[currentBrewingStation].canEmptyBrewingStation.Value = true;
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
        if (clientId == OwnerClientId && HasIngredient()) // HasIngredient 
        {
            for (int i = 0; i < ingredientsList.Count; i++)
            {
                Ingredient.DestroyIngredient(ingredientsList[i]);
            }
        }
    }

    private void TutorialMessage()
    {
        tutorialMessageActive = true;
        if (tutorialMessageActive)
            Time.timeScale = 0f;
    }

    // Normalized time to handle animations
    public float GetNormalizedTime(Animator animator, string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }

    public void OnAnimationSwitch()
    {
        if (HasIngredient())
        {
            anim.CrossFadeInFixedTime(MovementWithCupHash, CrossFadeDuration);
        }
        else if(GetPickup()!= null && GetPickup().GetPickupObjectSo().objectName == mopSoName)
        {
            anim.CrossFadeInFixedTime(MovementWithVacHash, CrossFadeDuration);
        }
        else if (GetPickup() != null && GetPickup().IsCustomer)
        {
            anim.CrossFadeInFixedTime(MovementWithCustomerHash, CrossFadeDuration);
        }
        else
        {
            anim.CrossFadeInFixedTime(MovementHash, CrossFadeDuration);
        }
    }

    // Play trash pick up and set trash parent while new player statemachine is done
    private IEnumerator PickUpAnimation(Pickup pickup)
    {
        if (!pickup.IsCustomer)
        {
            PickupSO pickupSo = pickup.GetPickupObjectSo();

            if (pickupSo.objectName == mopSoName)
            {
                anim.CrossFadeInFixedTime(BP_Barista_Pickup_VacHash, CrossFadeDuration);
            }
            else
            {
                anim.CrossFadeInFixedTime(BP_Barista_Floor_PickupHash, CrossFadeDuration);
            }

            movementToggle = false;

            yield return new WaitForSeconds(1f); // hard coded while new player statemachine is done

            pickup.SetPickupObjectParent(this);
            pickup.DisablePickupColliders(pickup);
            movementToggle = true;
            isAnimating = false;
        } 
    }

    // Play customer pick up and set customer parent
    private IEnumerator PickUpCustomerAnimation(Pickup pickup)
    {
        anim.CrossFadeInFixedTime(BP_Barista_Pickup_CustHash, CrossFadeDuration);
        movementToggle = false;

        yield return new WaitForSeconds(1.0f); // hard coded while new player statemachine is done

        pickup.GetNavMeshAgent().enabled = false;
        pickup.GetCustomer().SetCustomerState(CustomerBase.CustomerState.PickedUp);
        pickup.GetCustomer().isPickedUp = true;
        pickup.SetPickupObjectParent(this);
        pickup.DisablePickupColliders(pickup);

        if (pickup.GetCustomer().inLine == true)
        {
            int _CustomerPos = pickup.GetCustomer().currentPosInLine;
            CustomerManager.Instance.LineQueue.RemoveCustomerInPos(_CustomerPos);
            CustomerManager.Instance.ReduceCustomerLeftoServe();
        }
        
        movementToggle = true;
        isAnimating= false;
    }

    // Play throw pick up
    private IEnumerator ThrowPickUpAnimation()
    {
        if (pickup.IsCustomer)
        {
            anim.CrossFadeInFixedTime(BP_Barista_Throw_CustHash, CrossFadeDuration);
        }
        else
        {
            anim.CrossFadeInFixedTime(BP_Barista_Throw_CupHash, CrossFadeDuration);
        }
        movementToggle = false;

        yield return new WaitForSeconds(1.0f); // hard coded while new player statemachine is done

        if (pickup != null) 
        {
            pickup.GetComponent<IngredientFollowTransform>().SetTargetTransform(pickup.transform);
            pickup.EnablePickupColliders(pickup);

            pickup.transform.GetComponent<Rigidbody>().AddForce(transform.forward * (pickupThrowForce * pickup.GetThrowForceMultiplier()));
            if (pickup.gameObject.GetComponent<MopBehavior>() != null) pickup.gameObject.GetComponent<MopBehavior>().ReturnMop();
            pickup.ClearPickupOnParent(); 
        }
        movementToggle = true;
        isAnimating = false;
    }

    // Play throw ingredient 
    private IEnumerator ThrowIngredientAnimation()
    {
        anim.CrossFadeInFixedTime(BP_Barista_Throw_CupHash, CrossFadeDuration);
        movementToggle = false;

        yield return new WaitForSeconds(1.0f); // hard coded while new player statemachine is done

        for (int i = 0; i < ingredientsList.Count; i++)
        {
            if (ingredientsList[i] == null) continue;

            //Detach child from hold point
            ingredientsList[i].GetComponent<IngredientFollowTransform>().SetTargetTransform(ingredientsList[i].transform);

            //Enable collider
            ingredientsList[i].EnableIngredientCollision(ingredientsList[i]);

            // Apply the throw force to the ingredient
            Rigidbody ingredientRb = ingredientsList[i].GetComponent<Rigidbody>();
            if (ingredientRb != null)
            {
                ingredientRb.isKinematic = false;
                ingredientRb.AddForce(transform.forward * ingredientThrowForce, ForceMode.Force);
                ingredientRb.useGravity = true;
            }
            ingredientIndicatorText.SetText("");
            RemoveIngredientInListAtIndex(i);
            OnAnimationSwitch();
        }
        movementToggle = true;
        isAnimating = false;
    }
}