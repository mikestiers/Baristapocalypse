using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundState : PlayerBaseState
{

    //calls base constructor and pass in base stateMachine 
    public PlayerGroundState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    Ingredient floorIngredient;
    IngredientSO ingredienSO;
    Mouse mouse = Mouse.current;

   
     public override void Enter()
    {
        stateMachine.inputManager.JumpEvent += OnJump;
        stateMachine.inputManager.DashEvent += OnDash;
        stateMachine.inputManager.ThrowEvent += OnThrow;
        

        InputManager.Instance.playerInput.Player.Interact.performed += context => stateMachine.Interact(context);
        InputManager.Instance.playerInput.Player.InteractAlt.performed += context => stateMachine.InteractAlt(context);
        Debug.Log("Player enter moving state");

    }
   

    public override void Tick(float deltaTime)
    {
        // Ground Check
        stateMachine.IsGrounded();
        // player movement
        stateMachine.Move(stateMachine.moveSpeed);

     

        //  Pick ingredient from station 
        float interactDistance = 6.0f;
        if (Physics.Raycast(stateMachine.transform.position, stateMachine.transform.forward, out RaycastHit raycastHit, interactDistance, stateMachine.isStationLayer))
        {
            if (raycastHit.transform.TryGetComponent(out BaseStation baseStation))
            {
                if (baseStation != stateMachine.selectedStation)
                {
                    stateMachine.SetSelectedStation(baseStation);
                }
            }
            else
            {
                stateMachine.SetSelectedStation(null);
            }
        }
        else
        {
            stateMachine.SetSelectedStation(null);
        }

        //  Pick ingredient from floor 
        if (Physics.Raycast(stateMachine.transform.position, stateMachine.transform.forward, out RaycastHit raycastHitIngredient, 3, stateMachine.isIngredientLayer))
        {
            if (!stateMachine.HasIngredient())
            {
                if (raycastHitIngredient.transform.TryGetComponent(out  floorIngredient))
                {
                    ingredienSO = floorIngredient.IngredientSO;

                    if(mouse.leftButton.wasPressedThisFrame)
                        stateMachine.GrabIngedientFromFloor(floorIngredient, ingredienSO);
                }
            }
        }
        //if player has/get ingredient
        if (stateMachine.HasIngredient())
        {
            stateMachine.TurnOffIngredientCollider();

        }
        
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

    }

    public void OnDash()
    {
        AudioManager.Instance.Playoneshot(stateMachine.dashSFX, false);
        stateMachine.DashEffect.Play();
        stateMachine.StartCoroutine(Dash());

    }

    IEnumerator Dash()
    {
        float startTime = Time.time;

        while (Time.time < startTime + stateMachine.dashTime)
        {
            Debug.Log("esta funcionando....");
            stateMachine.Move(stateMachine.dashSpeed);
            yield return null;
        }

    }

    public void OnThrow()
    {
        if (stateMachine.HasIngredient())
        {
            stateMachine.SwitchState(new PlayerThrowState(stateMachine));
        }
    }

    public void OnGrab()
    {

    }




}

