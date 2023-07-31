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

    //Viuals
    GameObject visualGameObject;
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
        if (Physics.Raycast(stateMachine.transform.position + new Vector3(0, 0.1f, 0), stateMachine.transform.forward, out RaycastHit raycastHit, interactDistance, stateMachine.isStationLayer))
        {
            if (raycastHit.transform.TryGetComponent(out BaseStation baseStation))
            {
                visualGameObject = baseStation.transform.GetChild(0).gameObject;
                if (baseStation != stateMachine.selectedStation)
                {
                    stateMachine.SetSelectedStation(baseStation);
                    stateMachine.Show(visualGameObject);

                }
            }
            else
            {
                stateMachine.SetSelectedStation(null);
            }
        }
        else
        {
            stateMachine.Hide(visualGameObject);
            stateMachine.SetSelectedStation(null);
        }
        Debug.DrawRay(stateMachine.transform.position + new Vector3(0, 0.5f, 0), stateMachine.transform.forward, Color.green);

        // Pick ingredient from floor
        float floriIteractDistance = 3.0f;
        if (Physics.Raycast(stateMachine.transform.position, stateMachine.transform.forward, out RaycastHit raycastHitIngredient, floriIteractDistance, stateMachine.isIngredientLayer))
        {
            if (stateMachine.GetNumberOfIngredients() <= 4)
            {
                if (raycastHitIngredient.transform.TryGetComponent(out  floorIngredient))
                {
                    ingredienSO = floorIngredient.IngredientSO;

                    if (mouse.leftButton.wasPressedThisFrame)
                        stateMachine.GrabIngedientFromFloor(floorIngredient, ingredienSO);
                }
            }
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

