using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon_Tile : IHeapItem<Dungeon_Tile>
{
    public GameObject Tile;
    public Vector2Int Location;

    public Dungeon_Tile(GameObject tile, Vector2Int location)
    {
        Location = location;
        Tile = tile;
    }
    
    // Pathfinding, dont touch
    public int gCost;
    public int hCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public Dungeon_Tile parent;
    public int CompareTo(Dungeon_Tile other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
    private int heapIndex;
    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }
}
