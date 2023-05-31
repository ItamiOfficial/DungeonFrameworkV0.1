
using System.Collections.Generic;
using UnityEngine;

public class Controller_AI : Dungeon_Controller, IDungeon_TurnInterface
{
    
    private void Awake()
    {
        Dungeon_TurnManager.AddController(this);
        SetCharacter(GetComponent<Entity_Character>());
    }
    // ==== Turn Interface ==== //

    public void StartTurn()
    {
       MakeTurn();
    }

    protected virtual void MakeTurn()
    {
        Dungeon_PathfindingComponent pathfinding = GetComponent<Dungeon_PathfindingComponent>();
        if (pathfinding.Path == null || pathfinding.Path.Count == 0 || !Dungeon_MapManager.CanStepOnTile(pathfinding.PeekNextTile().Location))
        {
            pathfinding.FindPathToTarget(Character.GetLocation(),Dungeon_MapManager.GetRandomTile(false,true).Location,false);    
        }

        if (!Dungeon_MapManager.CanStepOnTile(pathfinding.PeekNextTile().Location))
        {
            EndTurn();
            return;
        }
        
        Character.SetLocation(pathfinding.GetNextTile().Location,false);
        EndTurn();
        return;
    }

    public void EndTurn()
    {
        Dungeon_TurnManager.Instance.SetNextPlayer();
    }
}
