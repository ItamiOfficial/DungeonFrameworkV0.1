
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "New Floor Generator", menuName = "Dungeon/Floor Generator", order = 1)]
public class Dungeon_MapGenMethod_Base_SO : ScriptableObject
{
    private Dictionary<Vector2Int, Dungeon_Tile> Map;
    private Dictionary<Vector2Int, Dungeon_Room> Rooms;
    private bool[,] UsedCells;


    [Header("Room Settings")]
    [Header("Count")]
    [Range(2,10)] public int MinRoomCount = 3;
    [Range(2,10)] public int MaxRoomCount = 5;

    [Header("Size")]
    [Range(0.0f,1.0f)] public float MinRoomSize = 0.4f;
    [Range(0.0f,1.0f)] public float MaxRoomSize = 0.8f;

    [Header("Room Growth")] 
    [Range(0,6)] public int MaxGrowsPerRoom = 2;
    [Range(0,6)] public int MinGrowsTotal = 0;
    [Range(0,6)] public int MaxGrowsTotal = 3;

    [Header("Cell Settings")]
    [Header("Amount")]
    [Range(2,8)]public int CellsX = 4;
    [Range(2,8)] public int CellsY = 2;
    
    [Header("Size")]
    [Range(8,24)]public int CellWidth = 8;
    [Range(8,24)]public int CellHeight = 8;

    [Header("Cell Settings")] 
    public GameObject TilePrefab;
    
    
    // Include Tilemap in this
    public Dictionary<Vector2Int, Dungeon_Tile> GenerateMap()
    {
        ClearMapTiles();
        
        Map = new Dictionary<Vector2Int, Dungeon_Tile>();
        Rooms = new Dictionary<Vector2Int, Dungeon_Room>();
        UsedCells = new bool[CellsX, CellsY];
        
        RandomizeDungeon();
        AddRoomTilesToMap();
        
        return Map;
    }

    public void ClearMapTiles()
    {
        if (Map == null)
            return;
        
        D_RoomGroup.GetGroups().Clear();
        
        List<Dungeon_Tile> temp = new List<Dungeon_Tile>(Map.Values);
        
        if (Map != null && Map.Count > 0)
        {
            for(int i = 0; i < temp.Count;i++)
            {
                Dungeon_Tile t = temp[i];
                if (t != null && t.Tile != null)
                {
                    Destroy(t.Tile);
                }
            }
        }
        
    }

    // ==== Base Generation Functions ==== //
    
    // ---- Room Generation & Extension ---- //
    protected void AddRoomToCell(Vector2Int cell)
    {
        // If there is already a room, we dont add another one there
        if (Rooms.ContainsKey(cell))
            return;
        
        Vector2Int cellCentre = new Vector2Int(cell.x * CellWidth + (CellWidth / 2), cell.y * CellHeight + (CellHeight / 2));
        
        int x1 = cellCentre.x - Mathf.RoundToInt(Mathf.Max(CellWidth * 0.5f * Random.Range(MinRoomSize, MaxRoomSize),2));
        int x2 = cellCentre.x + Mathf.RoundToInt(Mathf.Max(CellWidth * 0.5f * Random.Range(MinRoomSize, MaxRoomSize),2));
        int y1 = cellCentre.y - Mathf.RoundToInt(Mathf.Max(CellWidth * 0.5f * Random.Range(MinRoomSize, MaxRoomSize),2));
        int y2 = cellCentre.y + Mathf.RoundToInt(Mathf.Max(CellWidth * 0.5f * Random.Range(MinRoomSize, MaxRoomSize),2));
        
        Rooms.Add(cell,new Dungeon_Room(x1,y1,x2,y2,cell));
        UsedCells[cell.x, cell.y] = true;
    }
    protected void AddRoomTilesToMap()
    {
        foreach (var room in Rooms) {
            foreach (var tile in room.Value.Tiles)
            {
                AddTile(tile);
            }
        }
    }
    protected void ExtendRoom(Dungeon_Room room,Vector2Int dir)
    {
        Rooms.Add(room.Cell + dir,room);
        
        // Bottom left Point of new tiles
        Vector2Int p1;
        // Top Right Point of new tiles
        Vector2Int p2;

        if (dir == Vector2Int.right)
        {
            p1 = new Vector2Int(room.p2.x,room.p1.y);
            p2 = new Vector2Int(room.p2.x + CellWidth, room.p2.y);
            room.p2 = p2;
            room.RecalculateCentre();
        }
        
        else if (dir == Vector2Int.down)
        {
            p1 = new Vector2Int(room.p1.x,room.p1.y - CellHeight);
            p2 = new Vector2Int(room.p2.x, room.p1.y);  
            room.p1 = p1;
            room.RecalculateCentre();
        }
        
        else if (dir == Vector2Int.left)
        {
            p1 = new Vector2Int(room.p1.x - CellWidth,room.p1.y);
            p2 = new Vector2Int(room.p1.x, room.p2.y);       
            room.p1 = p1;
            room.RecalculateCentre();
        }
        
        else if (dir == Vector2Int.up)
        {
            p1 = new Vector2Int(room.p1.x,room.p2.y);
            p2 = new Vector2Int(room.p2.x, room.p2.y + CellHeight);       
            room.p2 = p2;
            room.RecalculateCentre();
        }
        else
        {
            // If we dont have a valid vector for room growth we return
            return;
        }

        for (int x = p1.x; x <= p2.x; x++)
        {
            for (int y = p1.y; y <= p2.y; y++)
            {
                room.Tiles.Add(new Vector2Int(x,y));
            }    
        }
    }
    protected void ExtendRandomRooms()
    {
        List<Dungeon_Room> viableList = new List<Dungeon_Room>(Rooms.Values);
        int roomsExtended = Random.Range(MinGrowsTotal,MaxGrowsTotal + 1);

        while (viableList.Count > 0 && roomsExtended > 0)
        {
            Dungeon_Room r = viableList[Random.Range(0,viableList.Count)];
            List<Vector2Int> neighbours = GetFreeNeighbourCells(r);
            
            if (neighbours.Count > 0)
            {
                var n = neighbours[Random.Range(0, neighbours.Count)];
                ExtendRoom(r,n - r.Cell);
                UsedCells[n.x, n.y] = true;
                roomsExtended--;
                r.GrownTimes++;
                if (r.GrownTimes >= MaxGrowsPerRoom)
                    viableList.Remove(r);
            }
            else
            {
                viableList.Remove(r);
            }
        }
    }
    
    // ---- Path Connection ---- //

    /*
     *  This Method basically looks at 4 Directions and creates a Path if some there is another room close by.
     *  DON'T OVERRIDE!
     */
    protected void ConnectRooms_LineOfSight(Dungeon_Room room)
    {
        // Going Right
        for (int i = room.Cell.x + 1; i < CellWidth; i++)
        {
            if (Rooms.ContainsKey(new Vector2Int(i, room.Cell.y)))
            {
                ConnectRooms(room,Rooms[new Vector2Int(i, room.Cell.y)]);
                break;
            }
        }
        // Going Left
        for (int i = room.Cell.x - 1; i >= 0; i--)
        {
            if (Rooms.ContainsKey(new Vector2Int(i, room.Cell.y)))
            {
                ConnectRooms(room,Rooms[new Vector2Int(i, room.Cell.y)]);
                break;
            }    
        }
        // Going Up
        for (int i = room.Cell.y + 1; i < CellHeight; i++)
        {
            if (Rooms.ContainsKey(new Vector2Int(room.Cell.x, i)))
            {
                ConnectRooms(room,Rooms[new Vector2Int(room.Cell.x, i)]);
                break;
            } 
        }
        // Going Down
        for (int i = room.Cell.y - 1; i >= 0; i--)
        {
            if (Rooms.ContainsKey(new Vector2Int(room.Cell.x, i)))
            {
                ConnectRooms(room,Rooms[new Vector2Int(room.Cell.x, i)]);
                break;
            }     
        }
    }

    /*
     *  The top 2 Functions are for actually placing down a line of tiles.
     *  The bottom 2 Function are there to actually Connect 2 Rooms/Tiles
     *  DON'T OVERRIDE!
     *
     *  ! IMPORTANT !
     *  Room Connection should take Taken Rooms into account e.g. only carve Paths through non ocupied Cells
     *  Also Paths should count as occupations too, so paths can always connect to other paths (Make an Edge System maybe)
     */
    protected void ConnectRooms(Dungeon_Room r1, Dungeon_Room r2)
    {
        if (!r1.Group.Contains(r2))
        {
            if (UtilityLibrary.GetRandomBool())
            {
                r1.Group.CombineGroups(r2.Group);
            } else {
                r2.Group.CombineGroups(r1.Group);    
            }
        }
        else
        {
            return;
        }
        
        Vector2Int v1;
        Vector2Int v2;

        int horizontalDiff = Mathf.Abs(r2.Centre.x - r1.Centre.x);
        int verticalDiff = Mathf.Abs(r2.Centre.y - r1.Centre.y);
        
        // We first Check if the rooms are horizontally or vertically connectd and then we go 
        bool leftRight = horizontalDiff >= verticalDiff;

        
        // Hier weiter arbeiten!
        if (leftRight)
        {
            if (r1.Centre.x <= r2.Centre.x)
            {
                // v2 => Left
                v1 = new Vector2Int(r1.p2.x,Random.Range(r1.p1.y + 1,r1.p2.y));
                v2 = new Vector2Int(r2.p1.x,Random.Range(r2.p1.y + 1,r2.p2.y));
                
                ConnectTiles(v1,v2,true);
            }
            else
            {
                
                
                // v2 => Right
                v1 = new Vector2Int(r1.p1.x,Random.Range(r1.p1.y + 1,r1.p2.y));
                v2 = new Vector2Int(r2.p2.x,Random.Range(r2.p1.y + 1,r2.p2.y));       
                ConnectTiles(v2,v1,true);
            }

        } else {
            if (r1.Centre.y <= r2.Centre.y)
            {
                // v2 => Above
                v1 = new Vector2Int(Random.Range(r1.p1.x + 1,r1.p2.x),r1.p2.y);
                v2 = new Vector2Int(Random.Range(r2.p1.x + 1,r2.p2.x),r2.p1.y);

                ConnectTiles(v1,v2,false);
            }
            else
            {
                
                
                // v2 => Below
                v1 = new Vector2Int(Random.Range(r1.p1.x + 1,r1.p2.x),r1.p1.y);
                v2 = new Vector2Int(Random.Range(r2.p1.x + 1,r2.p2.x),r2.p2.y);     
                ConnectTiles(v2,v1,false);
            }    
        }
        
        
    }
    protected void ConnectTiles(Vector2Int v1, Vector2Int v2, bool horizontal)
    {
        //int horizontalDiff = Mathf.Abs(v2.x - v1.x);
        //int verticalDiff = Mathf.Abs(v2.y - v1.y);

        int centre;
        if (horizontal)
        {
            // when the hDiff is greater we let path go vertical and then meet in the centre
            centre = (v1.x + v2.x) / 2;
            PlaceHorizontalPath(v1.x,centre,v1.y);
            PlaceHorizontalPath(v2.x,centre,v2.y);
            PlaceVerticalPath(v1.y,v2.y,centre);
            Debug.Log("V1: " + v1 +" | V2: " + v2 + " | Centre X: " + centre);
        } else {
            centre = (v1.y + v2.y) / 2;
            PlaceVerticalPath(v1.y,centre,v1.x);   
            PlaceVerticalPath(v2.y,centre,v2.x);   
            PlaceHorizontalPath(v2.x,v1.x,centre);
            Debug.Log("V1: " + v1 +" | V2: " + v2 + " | Centre Y: " + centre);
        }
    }
    protected void PlaceVerticalPath(int y1,int y2, int x)
    {
        if (y1 > y2)
        {
            (y1, y2) = (y2, y1);
        }

        for (int y = y1; y <= y2; y++)
        {
            AddTile(new Vector2Int(x,y));
        }
    }
    protected void PlaceHorizontalPath(int x1,int x2, int y)
    {
        if (x1 > x2)
        {
            (x1, x2) = (x2, x1);
        }

        for (int x = x1; x <= x2; x++)
        {
            AddTile(new Vector2Int(x,y));
        }
    }
    protected void ConnectAllRoomGroups_IGNORINGCELLS()
    {
        List<D_RoomGroup> temp = new List<D_RoomGroup>(D_RoomGroup.GetGroups());
        
        while (D_RoomGroup.GetGroupCount() > 1)
        {
            // Here we need to find the 2 closest
            D_RoomGroup g1 = temp[Random.Range(0, temp.Count)];
            temp.Remove(g1);
            D_RoomGroup g2 = temp[Random.Range(0, temp.Count)];

            Dungeon_Room r1 = g1.GetRooms().Count == 1 ? g1.GetRooms()[0] : g1.GetRooms()[Random.Range(0, g1.GetRooms().Count)];
            Dungeon_Room r2 = g2.GetRooms().Count == 1 ? g2.GetRooms()[0] : g2.GetRooms()[Random.Range(0, g2.GetRooms().Count)];
            RoomPairDist closestPair = new RoomPairDist(r1, r2);

            for (int i = 0; i < g1.GetRooms().Count; i++) {
                for (int k = 0; k < g2.GetRooms().Count; k++)
                {
                    if (i == k)
                        break;
                    
                    RoomPairDist n = new RoomPairDist(g1.GetRooms()[i],g2.GetRooms()[k]);
                    if (closestPair.Distance > n.Distance)
                    {
                        closestPair = n;
                    }
                }
            }
            
            ConnectRooms(g1.GetRooms()[Random.Range(0, g1.GetRooms().Count)],
                g2.GetRooms()[Random.Range(0, g2.GetRooms().Count)]);
            // Updating temp;
            temp = new List<D_RoomGroup>(D_RoomGroup.GetGroups());
        }
    }
    
    // ==== Utility==== //
    protected List<Vector2Int> GetFreeNeighbourCells(Dungeon_Room room)
    {
        // Neighbour Coords
        Vector2Int[] coords = new[] { new Vector2Int(1, 0),new Vector2Int(0, 1),new Vector2Int(-1, 0),new Vector2Int(0, -1) };
        List<Vector2Int> neighbours = new List<Vector2Int>();
        

        
        foreach (var vec in coords)
        {
            Vector2Int v = room.Cell + vec;
            if (!Rooms.ContainsKey(v) && v.x >= 0 & v.x < CellsX && v.y >= 0 && v.y < CellsY && !UsedCells[v.x,v.y])
            {
                neighbours.Add(v);
            }
        }

        return neighbours;
    }

    protected Dungeon_Room GetRandomRoom()
    {
        return Rooms.Values.ElementAt(Random.Range(0,Rooms.Count));
    }
    private void AddTile(Vector2Int tile)
    {
        GameObject g = Instantiate(TilePrefab, VecConversionLibrary.Vec2To3(tile), Quaternion.identity);
        Dungeon_Tile t = new Dungeon_Tile(g,tile);
        if (!Map.TryAdd(tile, t))
        {
            Destroy(g);
        }
    }

    public List<Dungeon_Room> GetRooms()
    {
        return new List<Dungeon_Room>(Rooms.Values);
    }
    
    // ==== Debug ==== //
    protected virtual void RandomizeDungeon()
    {
        // Cell Creation
        List<Vector2Int> posCells = new List<Vector2Int>();
        List<Vector2Int> chosenCells = new List<Vector2Int>();
        
        for (int x = 0; x < CellsX; x++)
        {
            for (int y = 0; y < CellsY; y++)
            {
                posCells.Add(new Vector2Int(x,y));
            }    
        }

        // Room Creation
        int roomCount = Random.Range(MinRoomCount,MaxRoomCount + 1);

        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int cell = posCells[Random.Range(0,posCells.Count - 1)];
            chosenCells.Add(cell);
            posCells.Remove(cell);
        }

        foreach (var cell in chosenCells)
        {
            AddRoomToCell(cell);
        }
        
        // Room Extension
        ExtendRandomRooms();
        
        // Room Connection
        foreach (var room in Rooms.Values)
        {
            ConnectRooms_LineOfSight(room);
        }
        Debug.Log("Group Count before AllConnectAlgo: " + D_RoomGroup.GetGroupCount());
        ConnectAllRoomGroups_IGNORINGCELLS();
        Debug.Log("Group Count after AllConnectAlgo: " + D_RoomGroup.GetGroupCount());
    }
}

public class Dungeon_Room
{
    public List<Vector2Int> Tiles;
    public Vector2Int Centre;
    public Vector2Int Cell;
    public int GrownTimes = 0;
    public Vector2Int p1;
    public Vector2Int p2;
    public D_RoomGroup Group;

    public Dungeon_Room(int x1, int y1, int x2, int y2)
    {
        Tiles = new List<Vector2Int>();
        Centre = new Vector2Int((x1 + x2)/2,(y1+y2)/2);
        Group = new D_RoomGroup(this);
        
        
        if (x1 > x2)
        {
            (x1, x2) = (x2, x1);
        }
        if (y1 > y2)
        {
            (y1, y2) = (y2, y1);
        }
        
        p1 = new Vector2Int(x1, y1);
        p2 = new Vector2Int(x2, y2);

        for (int x = x1; x <= x2; x++) {
            for (int y = y1; y <= y2; y++)
            {
                Vector2Int v = new Vector2Int(x, y);
                Tiles.Add(v);
            }
        }
    }

    public Dungeon_Room(int x1, int y1, int x2, int y2,Vector2Int cell)
    {
        Centre = new Vector2Int((x1 + x2)/2,(y1+y2)/2);
        Cell = cell;
        Tiles = new List<Vector2Int>();
        Group = new D_RoomGroup(this);


        if (x1 > x2)
        {
            (x1, x2) = (x2, x1);
        }
        if (y1 > y2)
        {
            (y1, y2) = (y2, y1);
        }
        
        p1 = new Vector2Int(x1, y1);
        p2 = new Vector2Int(x2, y2);
        
        for (int x = x1; x <= x2; x++) {
            for (int y = y1; y <= y2; y++)
            {
                Vector2Int v = new Vector2Int(x, y);
                Tiles.Add(v);
            }
        }
    }

    public void RecalculateCentre()
    {
        Centre = new Vector2Int((p1.x+p2.x)/2,(p1.y+p2.y)/2);    
    }
}

// Only to use inside this class, nowhere else //
public class D_RoomGroup
{
    private List<Dungeon_Room> Group = new List<Dungeon_Room>();
    private static HashSet<D_RoomGroup> RoomGroups = new HashSet<D_RoomGroup>();

    public D_RoomGroup(Dungeon_Room room)
    {
        RoomGroups.Add(this);
        Group.Add(room);
    }
    
    public void CombineGroups(D_RoomGroup g2)
    {
        // Adding each room from other group to this one
        foreach (var room in g2.Group) 
        {
            Group.Add(room);
            room.Group = this;
        }

        RoomGroups.Remove(g2);
    }

    public static int GetGroupCount()
    {
        return RoomGroups.Count;
    }

    public bool Contains(Dungeon_Room room)
    {
        return Group.Contains(room);
    }
    
    public static HashSet<D_RoomGroup> GetGroups()
    {
        return RoomGroups;
    }
    
    public List<Dungeon_Room> GetRooms()
    {
        return Group;
    }

}

public struct RoomPairDist
{
    private Dungeon_Room[] Rooms;
    public float Distance;
    
    public RoomPairDist(Dungeon_Room r1, Dungeon_Room r2)
    {
        Rooms = new Dungeon_Room[2];
        Rooms[0] = r1;
        Rooms[1] = r2;
        Distance = Vector2Int.Distance(r1.Cell, r2.Cell);
    }
}



