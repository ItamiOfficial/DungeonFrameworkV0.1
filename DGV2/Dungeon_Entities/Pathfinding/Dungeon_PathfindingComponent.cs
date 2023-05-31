using System.Collections.Generic;
using UnityEngine;

/*
 *  The Complete Pathfinding is based on Sebastian Lague's tutorial on A* in Unity: https://www.youtube.com/watch?v=TFyEWDMQUKc
 *  All Credits to him for sharing his great Tutorial!
 */

public enum ENeighbourMode
{
    Small4D, Big8D,
};

public class Dungeon_PathfindingComponent : MonoBehaviour
{
    public ENeighbourMode NeighbourMode = ENeighbourMode.Big8D;
    public List<Dungeon_Tile> Path = new List<Dungeon_Tile>();
    private Dictionary<Vector2Int,Dungeon_Tile> GetSubGraph(Vector2Int centre,int range)
    {
        Dictionary<Vector2Int,Dungeon_Tile> SubGraph = new Dictionary<Vector2Int,Dungeon_Tile>();
        
        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++)
            {
                Vector2Int v = new Vector2Int(x,y) + centre;
                if(Dungeon_MapManager.GetMap().ContainsKey(v))
                {
                    SubGraph.Add(v,Dungeon_MapManager.GetMap()[v]);    
                }

            }    
        }

        return SubGraph;
    }

    public void FindPathToTarget(Vector2Int start, Vector2Int end,bool bUseSubGraph)
    {
        FindPath(Dungeon_MapManager.GetMap()[start],Dungeon_MapManager.GetMap()[end],bUseSubGraph,false);
    }
    public void FindPathToNeighbour(Vector2Int start, Vector2Int end,bool bUseSubGraph)
    {
        FindPath(Dungeon_MapManager.GetMap()[start],Dungeon_MapManager.GetMap()[end],bUseSubGraph,true);
    }
    
    
    protected void FindPath(Dungeon_Tile startNode, Dungeon_Tile endNode, bool bUseSubGraph,bool IsNeighbourEnaugh)
    {
        // We Create a smaller Graph with the Target as our Range indicator. This way we have a way smaller Graph, instead of the hole Map
        Dictionary<Vector2Int,Dungeon_Tile> subGraph = Dungeon_MapManager.GetMap();
        int iterCount = 0;
        
        if (bUseSubGraph)
        {
            subGraph = GetSubGraph(startNode.Location,Mathf.CeilToInt(Vector2Int.Distance(startNode.Location,endNode.Location)) + 1);
            if (!subGraph.ContainsKey(endNode.Location))
            {
                Debug.Log("Target is not within SubGraph Range");
                return;
            }
        }

        // Here Starts the A*
        Heap<Dungeon_Tile> openSet = new Heap<Dungeon_Tile>(Dungeon_MapManager.GetMap().Count);
        HashSet<Dungeon_Tile> closedSet = new HashSet<Dungeon_Tile>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Dungeon_Tile currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            // Sometimes we just want to get next to another Location
            if (IsNeighbourEnaugh)
            {
                if (GetNeighbours(currentNode.Location, subGraph,NeighbourMode).Contains(currentNode))
                {
                    Path = RetracePath(startNode, endNode);
                    return;    
                }
            } else {
                // Final Destination
                if (currentNode == endNode)
                {
                    Path = RetracePath(startNode, endNode);
                    return;
                }    
            }


            foreach (var neighbour in GetNeighbours(currentNode.Location,subGraph,NeighbourMode))
            {
                if (!IsTileWalkable(neighbour.Location) || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMoveCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMoveCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMoveCost;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = currentNode;
                    
                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }

            }

            iterCount++;
        }
        Path.Clear();
        Debug.Log("Found no Path");
    }

    public List<Dungeon_Tile> GetNeighbours(Vector2Int centre,Dictionary<Vector2Int,Dungeon_Tile> graph, ENeighbourMode mode)
    {
        List<Dungeon_Tile> neighbours = new List<Dungeon_Tile>();

        Vector2Int[] small = new[] {new Vector2Int(1,0), new Vector2Int(-1,0) , new Vector2Int(0,1)  , new Vector2Int(0,-1)  };
        Vector2Int[] big = new[]
        {
            new Vector2Int(1,0), new Vector2Int(-1,0) , new Vector2Int(0,1)  , new Vector2Int(0,-1),
            new Vector2Int(1,1), new Vector2Int(-1,1) , new Vector2Int(-1,-1)  , new Vector2Int(1,-1)
        };
        
        switch (mode)
        {
            case ENeighbourMode.Small4D:
                foreach (var v in small)
                {
                    if (graph.ContainsKey(v + centre))
                    {
                        neighbours.Add(graph[v + centre]);
                    }
                }
                break;
            
            case ENeighbourMode.Big8D:
                foreach (var v in big)
                {
                    if (graph.ContainsKey(v + centre))
                    {
                        neighbours.Add(graph[v + centre]);
                    }
                }
                break;
        }

        return neighbours;
    }

    bool IsTileWalkable(Vector2Int v)
    {
        var validTile = Dungeon_MapManager.GetMap().ContainsKey(v);
        //var entityOnPosition = DungeonManager.Instance.GetEntityOnTile(v);
        return validTile;
    }

    int GetDistance(Dungeon_Tile a, Dungeon_Tile b)
    {
        int dstX = Mathf.Abs(a.Location.x - b.Location.x);
        int dstY = Mathf.Abs(a.Location.y - b.Location.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);

    }

    List<Dungeon_Tile> RetracePath(Dungeon_Tile start, Dungeon_Tile end)
    {
        List<Dungeon_Tile> path = new List<Dungeon_Tile>();
        Dungeon_Tile currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    public Dungeon_Tile GetNextTile()
    {
        if (Path is { Count: > 0 })
        {   
            Dungeon_Tile temp = Path[0];
            Path.RemoveAt(0);
            return temp;
        }

        return null;
    }
    
    public Dungeon_Tile PeekNextTile()
    {
        if (Path is { Count: > 0 })
        {   
            Dungeon_Tile temp = Path[0];
            return temp;
        }

        return null;
    }
   
}
