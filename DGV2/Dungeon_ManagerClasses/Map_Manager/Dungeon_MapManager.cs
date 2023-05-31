using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dungeon_MapManager : MonoBehaviour
{
    protected static Dictionary<Vector2Int, Dungeon_Tile> Map;
    public bool isMapReady = false;
    private void Awake()
    {
        Map = new Dictionary<Vector2Int, Dungeon_Tile>();
        isMapReady = Map != null && Map.Count > 0;
    }

    // ==== Map ==== //
    public static void SetMap(Dictionary<Vector2Int, Dungeon_Tile> map)
    {
        Map = map;
    }
    
    public static Dictionary<Vector2Int, Dungeon_Tile> GetMap() { return Map; }
    
    // ==== Tile Getter ==== //
    public static Dungeon_Tile GetTile(Vector2Int v)
    {
        Dungeon_Tile t;
        if (Map.TryGetValue(v,out t))
        {
            return t;
        }

        return null;
    }
    public static Dungeon_Tile GetRandomTile(bool avoidItems, bool avoidCharacter)
    {
        Dictionary<Vector2Int, Dungeon_Tile> temp = new Dictionary<Vector2Int, Dungeon_Tile>(Map);

        // We Filter our Dictionary
        if (avoidItems)
        {
            foreach (var item in Dungeon_Manager.Items)
            {
                temp.Remove(item.GetLocation());
            }
        }
        if (avoidCharacter)
        {
            foreach (var character in Dungeon_Manager.Characters)
            {
                temp.Remove(character.GetLocation());
            }
        }
        
        //  We return a random item from our now filtered list...
        return temp.Values.ElementAt(Random.Range(0,temp.Count));  
    }

    public static HashSet<Dungeon_Tile> GetNeighbours4D(Vector2Int centre)
    {
        // Neighbour Coords
        Vector2Int[] coords = new[] { new Vector2Int(1, 0),new Vector2Int(0, 1),new Vector2Int(-1, 0),new Vector2Int(0, -1) };
        
        // Temp return array with temp dungeontile
        HashSet<Dungeon_Tile> tiles = new HashSet<Dungeon_Tile>();
        Dungeon_Tile t;
        
        foreach (var vec in coords)
        {
            if (Map.TryGetValue(centre + vec, out t))
            {
                tiles.Add(t);
            }
        }

        return tiles;
    }
    
    public static HashSet<Dungeon_Tile> GetNeighbours8D(Vector2Int centre)
    {
        // Neighbour Coords
        Vector2Int[] coords = new[]
        {
            new Vector2Int(1, 0),new Vector2Int(0, 1),new Vector2Int(-1, 0),new Vector2Int(0, -1) ,
            new Vector2Int(1, 1),new Vector2Int(-1, 1),new Vector2Int(-1, -1),new Vector2Int(1, -1) 
        };
        
        // Temp return array with temp dungeontile
        HashSet<Dungeon_Tile> tiles = new HashSet<Dungeon_Tile>();
        Dungeon_Tile t;
        
        foreach (var vec in coords)
        {
            if (Map.TryGetValue(centre + vec, out t))
            {
                tiles.Add(t);
            }
        }

        return tiles;
    }

    public static bool CanStepOnTile(Vector2Int v)
    {
        bool isTile = Map.ContainsKey(v);
        bool isObstacleOnTile = IsObstacleOnTile(v);
        return isTile && !isObstacleOnTile;
    }

    public static bool IsObstacleOnTile(Vector2Int v)
    {
        return Dungeon_Manager.IsCharacterOnTile(v);
    }
}
