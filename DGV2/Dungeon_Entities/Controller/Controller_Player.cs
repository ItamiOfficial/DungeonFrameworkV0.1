using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller_Player : Dungeon_Controller, IDungeon_TurnInterface
{
    protected bool MyTurn;

    private Dungeon_InputActions InputActions;
    private Vector2Int WantedDirection;
    
    // ==== Unity Events ==== //
    private void Awake()
    {
        InputActions = new Dungeon_InputActions();
        InputActions.Dungeon.Move.performed += AddMoveInput;
        InputActions.Dungeon.Move.canceled += ResetMoveInput;

        SetCharacter(GetComponent<Entity_Character>());
    }


    private void OnEnable()
    {
        InputActions.Enable();
    }

    private void OnDisable()
    {
        InputActions.Disable();
    }

    private void Update()
    {
        if (MyTurn && !Character.IsMoving && WantedDirection != Vector2Int.zero && Dungeon_MapManager.CanStepOnTile(WantedDirection + Character.GetLocation()))
        {
            Character.SetLocation(WantedDirection + Character.GetLocation(),false);
            
            if (!StandsOnStairs())
            {
                EndTurn();    
            }


        }
    }



    private bool StandsOnStairs()
    {
        if (Character.GetLocation() == Dungeon_Manager.Instance.Exit.GetLocation())
        {
            Dungeon_Manager.Instance.Exit.GoToNextFloor();
            return true;
        }

        return false;
    }
    
    // ==== Input ==== //
    private void AddMoveInput(InputAction.CallbackContext ctx)
    {
        WantedDirection = Vector2Int.RoundToInt(ctx.ReadValue<Vector2>());
    }   
    
    private void ResetMoveInput(InputAction.CallbackContext obj)
    {
        WantedDirection = Vector2Int.zero;
    }

    // ==== Movement ==== //
    
    
    // ==== Turn Interface ==== //
    public void StartTurn()
    {
        MyTurn = true;
    }

    public void EndTurn()
    {
        MyTurn = false;
        Dungeon_TurnManager.Instance.SetNextPlayer();
    }
}
