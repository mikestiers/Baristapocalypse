using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
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
        stateMachine.inputManager.InteractEvent += Interact;
        stateMachine.inputManager.InteractAltEvent += InteractAlt;

        // Define the interactable layer mask to include station, ingredient, and mess layers.
        interactableLayerMask = stateMachine.isStationLayer | stateMachine.isIngredientLayer | stateMachine.isMessLayer | stateMachine.isMopLayer;


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
        float interactDistance = 2.0f;
        if (Physics.Raycast(stateMachine.transform.position + RayCastOffset, stateMachine.transform.forward, out RaycastHit hit, interactDistance, interactableLayerMask))
        {
            // Check the type of the hit object.
            // Logic for Station Interaction
            if (hit.transform.TryGetComponent(out BaseStation baseStation) && !stateMachine.hasMop)
            {

                visualGameObject = baseStation.transform.GetChild(0).gameObject;
                if (baseStation != stateMachine.selectedStation)
                {
                    stateMachine.SetSelectedStation(baseStation);
                    stateMachine.Show(visualGameObject);
                }
            }

            // Logic for Ingredient  on floor Interaction 
            else if (hit.transform.TryGetComponent(out Ingredient ingredient))
            {

                if (stateMachine.GetNumberOfIngredients() <= stateMachine.maxIngredients && !stateMachine.hasMop)
                {
                    ingredienSO = ingredient.IngredientSO;

                    if (mouse.leftButton.wasPressedThisFrame)
                    {
                        stateMachine.GrabIngedientFromFloor(ingredient, ingredienSO);
                    }
                    else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                    {
                        stateMachine.GrabIngedientFromFloor(ingredient, ingredienSO);
                    }
                }


            }

            // Logic for Mess Interaction
            else if (hit.transform.TryGetComponent(out MessBase mess))
            {
                visualGameObject = mess.transform.GetChild(0).gameObject;
                if (mess != stateMachine.selectedMess)
                {
                    stateMachine.SetSelectedMess(mess);
                    if (stateMachine.hasMop)
                    {
                        stateMachine.Show(visualGameObject);
                    }
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
            if (visualGameObject)
            {
                stateMachine.Hide(visualGameObject);
            }
            stateMachine.SetSelectedStation(null);
            stateMachine.SetSelectedMess(null);
            stateMachine.SetSelectedMop(null);
        }
        Debug.DrawRay(stateMachine.transform.position + RayCastOffset, stateMachine.transform.forward, Color.green);

    }

    public override void Exit()
    {
        stateMachine.inputManager.JumpEvent -= OnJump;
        stateMachine.inputManager.DashEvent -= OnDash;
        stateMachine.inputManager.ThrowEvent -= OnThrow;
        stateMachine.inputManager.InteractEvent -= Interact;
        stateMachine.inputManager.InteractAltEvent -= InteractAlt;
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

        if (stateMachine.GetNumberOfIngredients() > 0) 
        { 
            if (stateMachine.CheckIfHoldingLiquid() > 0)//stateMachine.ingredient.GetIngredientSO().objectTag == "Milk")
            {
                MessBase.SpawnMess(stateMachine.spillPrefab , stateMachine.spillSpawnPoint);
                stateMachine.ThrowIngedient();
                Debug.Log("tienes Leche " + stateMachine.spillPrefab.name);
                
            }
        }
        else return;
        
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

    public void Interact()
    {
        if (stateMachine.selectedStation)
        {
            stateMachine.selectedStation.Interact(stateMachine);
        }

        if (stateMachine.selectedMess)
        {
            stateMachine.selectedMess.Interact(stateMachine);
        }

    }

    public void InteractAlt()
    {
        if (stateMachine.selectedStation)
        {
            stateMachine.selectedStation.InteractAlt(stateMachine);
        }
        if (stateMachine.selectedMess)
        {
            stateMachine.selectedMess.InteractAlt(stateMachine);
        }
        if (stateMachine.selectedMop)
        {
            stateMachine.selectedMop.InteractAlt(stateMachine);
        }
    }
}

