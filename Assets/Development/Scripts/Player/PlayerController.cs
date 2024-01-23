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

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour, IIngredientParent, IPickupObjectParent,ISpill
{
    // Player Instance
    [HideInInspector] public static PlayerController Instance { get; private set; }

    [Header("Player Attributes")]
    [SerializeField] private float moveSpeed;
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
    //[SerializeField] private GameObject ingredientInstanceHolder;
    private BaseStation selectedStation;
    private Base selectedCustomer;
    public float sphereCastRadius = 1f;
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
    [SerializeField] private Spill spill;
    [Header("Pickups")]
    public Transform pickupLocation;
    public float pickupThrowForce;
    [SerializeField] private Pickup pickup;

    private CinemachineVirtualCamera virtualCamera;

    public PlayerColorChoice playerVisual;

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
        }

        //Get components
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        //Set variables if null
        if (moveSpeed <= 0) moveSpeed = 10.0f;
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (dashForce <= 0) dashForce = 4000f;
        if (dashTime <= 0) dashTime = 0.1f;
        if (dashCooldownTime <= 0) dashCooldownTime = 1.0f;
        if (ingredientThrowForce <= 0) ingredientThrowForce = 10f;
        if (groundCheckRadius <= 0) groundCheckRadius = 0.05f;

        //Define the interactable layer mask to include station, ingredient, and mess layers.
        interactableLayerMask = isStationLayer | isIngredientLayer | isMessLayer | isMopLayer | isCustomerLayer ;

        RayCastOffset = new Vector3(0, 0.4f, 0);

        // Set color of the player based on color selection at the lobby
        PlayerData playerData = BaristapocalypseMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(BaristapocalypseMultiplayer.Instance.GetPlayerColor(playerData.colorId));
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
        if(movementToggle)
            Move(moveSpeed);

        if (!movementToggle)
        {
            anim.SetFloat("vertical", 0);
            anim.SetFloat("horizontal", 0);
            return;
        }

        // Perform a single raycast to detect any interactable object.
        float interactDistance = 2.5f;
        if (Physics.SphereCast(transform.position + RayCastOffset, sphereCastRadius, transform.forward, out RaycastHit hit, interactDistance, interactableLayerMask))
        {
            // Logic for PickUp Interaction
            if (hit.transform.TryGetComponent(out Pickup pickup))
            {
                if (mouse.rightButton.wasPressedThisFrame)
                    DoPickup(pickup);
                else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                {
                    DoPickup(pickup);
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
                if (GetNumberOfIngredients() <= GetMaxIngredients() && !IsHoldingPickup)
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
            // No interactable object hit, clear selected objects.
            SetSelectedStation(null);
            //Hide(visualGameObject);
        }

        // Customer Interaction Logic
        float customerInteractDistance = 5.0f;
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
        if (!IsOwner) return;

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
        if (!IsLocalPlayer) return;

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
        if (!IsLocalPlayer) return;
        if (isDashing) return;

        if (movementToggle)
            StartCoroutine(Dash());
       // SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.dash);
       //Instantiate(spillPrefab.prefab, spillSpawnPoint.position, Quaternion.identity);

        if (GetNumberOfIngredients() > 0)
        {
           if (CheckIfHoldingLiquid() > 0)//stateMachine.ingredient.GetIngredientSO().objectTag == "Milk")
           {
               if (spillPrefab != null)
               {
                 Spill.PlayerCreateSpill(spillPrefab, this);
               }
               else
               {
                   Debug.Log("MessSO is null");
               }
               ThrowIngredient();
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
            rb.AddForce(inputManager.moveDir * dashForce * Time.deltaTime, ForceMode.Acceleration);
            yield return null;
        }

        yield return new WaitForSeconds(dashCooldownTime);
        isDashing = false;

    }

    public void OnThrow()
    {
        if(!IsLocalPlayer) return;
        if (IsHoldingPickup)
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
        foreach(Ingredient i in ingredientsList)
        {
            if(i.GetIngredientSO().objectTag == "Milk")
            {
                count++;
            }
        }
        return count;
    }

    public Transform GetNextHoldPoint()
    {
        if(GetNumberOfIngredients() > GetMaxIngredients())
        {
            return null;
        }
        Debug.Log("getting hold points " + (numberOfIngredientsHeld-1));
        return ingredientHoldPoints[GetNumberOfIngredients() - 1];
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
                ingredientRb.AddForce(transform.forward * ingredientThrowForce, ForceMode.Impulse);
            }
            ingredientIndicatorText.SetText("");
            RemoveIngredientInListAtIndex(i);
        }
    }

    public void GrabIngredientFromFloor(Ingredient floorIngredient, IngredientSO ingredientSO)
    {
        if (IsHoldingPickup)
            return;
        if(GetNumberOfIngredients() >= GetMaxIngredients())
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
        return GetNextHoldPoint();
    }

    public void SetPickup(Pickup pickup)
    {
        this.pickup = pickup;
    }

    public void ClearPickup()
    {
        pickupSo = null;
    }

    public bool HasPickup()
    {
        return pickupSo != null;
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

    // Mess Interface implementation
    public bool IsHoldingPickup => pickupLocation.childCount > 0;
    public void DoPickup(Pickup pickup)
    {
        if (IsHoldingPickup || !HasNoIngredients)
            return;
       
        // Pickup p = Instantiate(pickup, pickupLocation) as Pickup;
        //Pickup.SpawnPickupItem(pickupSo, this);
        PickupSO pickupSo = pickup.GetPickupObjectSo();

        if (pickupSo != null)
        {
            Pickup.SpawnPickupItem(pickupSo, this);
        }
        // if (p.IsCustomer)
        // {
        //     p.GetNavMeshAgent().enabled = false;
        //     p.GetCustomer().SetCustomerStateServerRpc(CustomerBase.CustomerState.PickedUp);
        // }
        //
        // p.RemoveRigidBody();
        // p.transform.localRotation = Quaternion.Euler(p.holdingRotation);
        // p.transform.localPosition = p.holdingPosition;
        // p.GetCollider().enabled = false;
        //Destroy(pickup.gameObject);
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
            for(int i=0; i<ingredientsList.Count; i++) {
                Ingredient.DestroyIngredient(ingredientsList[i]);
            }
        }
    }
    
}
