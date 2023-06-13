using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerStateMachine : stateMachine, IIngredientParent
{
    //[Header("Player Attributes")]
    [field: SerializeField] public float moveSpeed { get; private set; }
    [field: SerializeField] public float jumpForce { get; private set; }
    [field: SerializeField] public float ingredienThrowForce { get; private set; }
    

    //[Header("Ground Check")]
    [field: SerializeField] public LayerMask isGroundLayer { get; private set; }
    [field: SerializeField] public float groundCheckRadius { get; private set; }
    [field: SerializeField] public Transform groundCheck { get; private set; }
    [field: SerializeField] public bool isGrounded;
    [field: SerializeField] public float dashSpeed;
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

    //Components
    [field: SerializeField] public InputManager inputManager { get; private set; }

    public AudioClip WalkingSFX;
    public AudioClip JumpSFX;
    public AudioClip ThrowSFX;
    public AudioClip PickupSFX; 
    public AudioClip dashSFX;
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepSounds;
    
    private bool isFootstepPlaying = false;

    public GameObject ThrowEffect;
    public GameObject DashEffect;
    // Start is called before the first frame update
    private void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();

        //Set variables if null
        if (moveSpeed <= 0) moveSpeed = 10.0f;
        if (jumpForce <= 0) jumpForce = 200.0f;
        if (dashSpeed <= 0) dashSpeed = 17.0f;
        if (dashTime <= 0) dashTime = 0.1f;
        if (dashSpeed <= 0) dashSpeed = 17.0f;
        if (ingredienThrowForce <= 0) ingredienThrowForce = 10f;
        if (groundCheckRadius <= 0) groundCheckRadius = 0.05f;
        hasIngredient = false;
     

        SwitchState(new PlayerGroundState(this)); // Start player state

        

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
            StopFootstepSound();
            return;
        }

            
            curMoveInput = inputManager.moveDir * moveSpeed * Time.deltaTime;
        transform.forward = inputManager.moveDir;

        rb.MovePosition(rb.position + curMoveInput);

         if (isGrounded && inputManager.playerIsmoving)
        {
            PlayFootstepSound();
        }
        else
        {
            StopFootstepSound();
        }
        
    }

   
    public void Interact(InputAction.CallbackContext context)
    {
        if (selectedStation)
        {
            AudioManager.Instance.Playoneshot(PickupSFX, false);
            selectedStation.Interact(this);
        }

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
        Instantiate(ThrowEffect, transform.position, transform.rotation);
        
        rb.isKinematic = false;
        rb.AddForce(transform.forward * ingredienThrowForce, ForceMode.Impulse);
        AudioManager.Instance.Playoneshot(ThrowSFX, false);

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
        hasIngredient = false;
        ingredient = null;
    }

    public bool HasIngredient()
    {

        hasIngredient = true;
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

    public void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0 || footstepAudioSource == null || isFootstepPlaying)
        {
            return;
        }

      if (inputManager.playerIsmoving)
      {
          AudioClip footstepClip = footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Length)];
          footstepAudioSource.clip = footstepClip;
          footstepAudioSource.loop = true;
          footstepAudioSource.Play();
      
          isFootstepPlaying = true;
      }

    }



    public void StopFootstepSound()
    {
        if (footstepAudioSource != null && isFootstepPlaying)
        {
            footstepAudioSource.Stop();
            isFootstepPlaying = false;
            Debug.Log("set footstep false");
        }
        
    }

    public IEnumerator TimeDelay(float time)
    {
        

        yield return new WaitForSeconds(time);

        
    }

    
}
