using System;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon_TurnManager : MonoBehaviour
{
    private static readonly List<IDungeon_TurnInterface> EntityControllers = new List<IDungeon_TurnInterface>();
    private static Queue<IDungeon_TurnInterface> Queue = new Queue<IDungeon_TurnInterface>();

    public static Dungeon_TurnManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static void AddController(IDungeon_TurnInterface ntt)
    {
        if (EntityControllers.Contains(ntt ))
            return;
        
        EntityControllers.Add(ntt);
    }

    public static void ClearControllerList()
    {
        EntityControllers.Clear();
        Queue.Clear();
    }

    public void SetNextPlayer()
    {
        if (Queue.Count == 0)
        {
            Queue = GetSortQueue();
        } 
        var ntt = Queue.Dequeue();
        if (ntt == null)
        {
            SetNextPlayer();
            return;
        }
        ntt.StartTurn();
    }

    protected virtual Queue<IDungeon_TurnInterface> GetSortQueue()
    {
        return new Queue<IDungeon_TurnInterface>(EntityControllers);
    }
}
