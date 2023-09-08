using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerGroundState : PlayerBaseState
{

    //Viuals
    [SerializeField] private GameObject visualGameObject;
    //calls base constructor and pass in base stateMachine 
    public PlayerGroundState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    private Ingredient floorIngredient;
    private IngredientSO ingredienSO;
    private Mouse mouse = Mouse.current;
    private LayerMask interactableLayerMask; // A single LayerMask for all interactable objects
    private Vector3 RayCastOffset; // Temp for raising Raycast poin of origin
  
     public override void Enter()
    {

        stateMachine.inputManager.JumpEvent += OnJump;
        stateMachine.inputManager.DashEvent += OnDash;
        stateMachine.inputManager.ThrowEvent += OnThrow;

        // Define the interactable layer mask to include station, ingredient, and mess layers.
        interactableLayerMask = stateMachine.isStationLayer | stateMachine.isIngredientLayer | stateMachine.isMessLayer | stateMachine.isMopLayer;

        InputManager.Instance.playerInput.Player.Interact.performed += context => stateMachine.Interact(context);
        InputManager.Instance.playerInput.Player.InteractAlt.performed += context => stateMachine.InteractAlt(context);
        Debug.Log("Player enter moving state");

        RayCastOffset = new Vector3(0, 0.3f, 0);
     }
   

    public override void Tick(float deltaTime)
    {
        // Ground Check
        stateMachine.IsGrounded();
        // player movement
        stateMachine.Move(stateMachine.moveSpeed);

        stateMachine.GetNumberOfIngredients();
        stateMachine.SetIngredientIndicator();

        // Perform a single raycast to detect any interactable object.
        float interactDistance = 6.0f;
        if (Physics.Raycast(stateMachine.transform.position + RayCastOffset, stateMachine.transform.forward, out RaycastHit hit, interactDistance, interactableLayerMask))
        {
            // Check the type of the hit object.
            // Logic for Station Interaction
            if (hit.transform.TryGetComponent(out BaseStation baseStation))
            {
                visualGameObject = baseStation.transform.GetChild(0).gameObject;
                if (baseStation != stateMachine.selectedStation)
                {
                    stateMachine.SetSelectedStation(baseStation);
                    stateMachine.Show(visualGameObject);
                }
            }
            // Logic for Ingredient Interaction
            else if (hit.transform.TryGetComponent(out Ingredient ingredient))
            {
                if (stateMachine.GetNumberOfIngredients() <= stateMachine.maxIngredients)
                {
                    ingredienSO = ingredient.IngredientSO;

                    if (mouse.leftButton.wasPressedThisFrame)
                        stateMachine.GrabIngedientFromFloor(ingredient, ingredienSO);
                }
            }
            // Logic for Mess Interaction
            else if (hit.transform.TryGetComponent(out MessBase mess))
            {
                if (mess != stateMachine.selectedMess)
                {
                    stateMachine.SetSelectedMess(mess);
                }
                Debug.Log("Detecting " + mess);
            }
            // logic for Mop Interaction
            else if (hit.transform.TryGetComponent(out Mop Mop)) 
            {
                if (Mop != stateMachine.selectedMop) 
                {
                    stateMachine.SetSelectedMop(Mop);
                }
                Debug.Log("Geting " + Mop);
            }
        }
        else
        {
            // No interactable object hit, clear selected objects.
            stateMachine.Hide(visualGameObject);
            stateMachine.SetSelectedStation(null);
            stateMachine.SetSelectedMess(null);
            stateMachine.SetSelectedMop(null);
        }
        Debug.DrawRay(stateMachine.transform.position + RayCastOffset, stateMachine.transform.forward, Color.green);


        // //  Pick ingredient from station 
        // float interactDistance = 6.0f;
        // if (Physics.Raycast(stateMachine.transform.position + new Vector3(0, 0.1f, 0), stateMachine.transform.forward, out RaycastHit raycastHit, interactDistance, stateMachine.isStationLayer))
        // {
        //     if (raycastHit.transform.TryGetComponent(out BaseStation baseStation))
        //     {
        //         visualGameObject = baseStation.transform.GetChild(0).gameObject;
        //         if (baseStation != stateMachine.selectedStation)
        //         {
        //             stateMachine.SetSelectedStation(baseStation);
        //             stateMachine.Show(visualGameObject);
        //         }
        //     }
        //     else
        //     {
        //         stateMachine.SetSelectedStation(null);
        //     }
        // }
        // else
        // {
        //     stateMachine.Hide(visualGameObject);
        //     stateMachine.SetSelectedStation(null);
        // }
        // Debug.DrawRay(stateMachine.transform.position + new Vector3(0, 0.5f, 0), stateMachine.transform.forward, Color.green);
        //
        // // Pick ingredient from floor
        // float floriIteractDistance = 3.0f;
        // if (Physics.Raycast(stateMachine.transform.position, stateMachine.transform.forward, out RaycastHit raycastHitIngredient, floriIteractDistance, stateMachine.isIngredientLayer))
        // {
        //     if (stateMachine.GetNumberOfIngredients() <= stateMachine.maxIngredients)
        //     {
        //         if (raycastHitIngredient.transform.TryGetComponent(out  floorIngredient))
        //         {
        //             ingredienSO = floorIngredient.IngredientSO;
        //
        //             if (mouse.leftButton.wasPressedThisFrame)
        //                 stateMachine.GrabIngedientFromFloor(floorIngredient, ingredienSO);
        //         }
        //     }
        // }
        //
        // //  Interact with mess
        // if (Physics.Raycast(stateMachine.transform.position, stateMachine.transform.forward, out RaycastHit raycastHitMess, floriIteractDistance, stateMachine.isMessLayer))
        // {
        //    
        //     if (raycastHitMess.transform.TryGetComponent(out MessBase mess))
        //     {
        //         if (mess != stateMachine.selectedMess)
        //         {
        //             stateMachine.SetSelectedMess(mess);
        //             
        //         }
        //         Debug.Log("Detecting)" + mess);
        //     }
        //     else
        //     {
        //         stateMachine.SetSelectedMess(null);
        //     }
        // }
        // else
        // {
        //     stateMachine.SetSelectedMess(null);
        // }
    }

    public override void Exit()
    {
        stateMachine.inputManager.JumpEvent -= OnJump;
        stateMachine.inputManager.DashEvent -= OnDash;
        stateMachine.inputManager.ThrowEvent -= OnThrow;
    }

    public void OnJump()
    {
        if (!stateMachine.IsGrounded()) { return; }

        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.jump);
    }

    public void OnDash()
    {
        stateMachine.StartCoroutine(Dash());
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.dash);
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        Debug.Log("dash");

        while (Time.time < startTime + stateMachine.dashTime)
        {
            stateMachine.GetComponent<Rigidbody>().AddForce(stateMachine.inputManager.moveDir * stateMachine.dashForce, ForceMode.Acceleration);
            yield return null;
        }

    }

    public void OnThrow()
    {
        if (stateMachine.GetNumberOfIngredients() > 0)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.throwIngredient);
            stateMachine.ThrowIngedient();

            stateMachine.numberOfIngredientsHeld = 0;

        }
    }

   
}

