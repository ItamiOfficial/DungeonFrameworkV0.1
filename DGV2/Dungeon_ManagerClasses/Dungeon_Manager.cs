using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Dungeon_MapManager))]
[RequireComponent(typeof(Dungeon_TurnManager))]
public class Dungeon_Manager : MonoBehaviour
{
    // ==== Singleton Pattern ==== //
    public static Dungeon_Manager Instance;
    private void CheckInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
    }
    // ==== Entity Lists ==== //
    private static List<Dungeon_Entity> Entities = new List<Dungeon_Entity>();
    public static List<Entity_Character> Characters = new List<Entity_Character>();
    public static List<Entity_Item> Items = new List<Entity_Item>();

    public static Entity_Character PlayerCharacter;
    public static Controller_Player PlayerController;
    public  GameObject PlayerPrefab;

    public Entity_Exit Exit;
    public GameObject ExitPrefab;
    
    // ==== Dungeon ==== //
    public Dungeon_Dungeon_SO Dungeon;
    public byte FloorIndex = 0;
    
    // ==== Unity Events ==== //
    private void Awake()
    {
        // Check Singleton Instance
        CheckInstance();
 
    }
    
    private void Start()
    {
        GenerateNextFloor();
    }

    public void GenerateNextFloor()
    {
        GenerateDungeonFloor(FloorIndex);
        FloorIndex++;
    }

    public void GenerateDungeonFloor(byte floorIndex)
    {
        ClearOldFloor();

        if (floorIndex == Dungeon.Floors.Length)
        {
            ExtendedDebugLibrary.Log("You Finished this Dungeon!",EColor.red);
            return;
        }
        
        // Generating new Dungeon
        Dungeon_MapManager.SetMap(GetCurrentFloor().Layout.GenerateMap());
        
        //Placing Exit
        List<Dungeon_Room> r = GetCurrentFloor().Layout.GetRooms();
        Dungeon_Room rr = r[Random.Range(0, r.Count)];
        PlaceExit(rr.Tiles[Random.Range(0,rr.Tiles.Count)]);
        
        // Spawning Player 
        SpawnPlayer(); 
        
        // Spawning Entities
        var nttCount = GetCurrentFloor().GetRandomEntityCount();
        
        for (int i = 0; i <= nttCount; i++)
        {
            SpawnCharacter(ChanceWrapper<GameObject>.GetRandomEntity(GetCurrentFloor().Enemies));
        }
        
        Dungeon_TurnManager.Instance.SetNextPlayer();
    }

    private void ClearOldFloor()
    {
        // Clear Map
        if (Dungeon_MapManager.GetMap().Count > 0 && FloorIndex > 0)
        {
            Dungeon.Floors[FloorIndex - 1].Layout.ClearMapTiles();
        }
        
        // Clear Controller
        Dungeon_TurnManager.ClearControllerList();

        // Remove all Entities
        RemoveAllEntities();
        
        Entities.Clear();
        Characters.Clear();
        Items.Clear();
    }

    private Dungeon_Floor_SO GetCurrentFloor()
    {
        return Dungeon.Floors[FloorIndex];
    }
    // ==== Entities & Players ==== //

    public static void AddEntity(Dungeon_Entity e)
    {
        Entities.Add(e);
    }
    
    private void SpawnPlayer()
    {
        if (PlayerCharacter == null)
        {
            // Player Character
            GameObject player = Instantiate(PlayerPrefab, transform.position, Quaternion.identity);
            PlayerCharacter = player.GetComponent<Entity_Character>();
            
            // Player Controller
            PlayerController = player.GetComponent<Controller_Player>();

            PlayerCharacter.InitEntity(Dungeon_MapManager.GetRandomTile(false, false).Location);
            Dungeon_TurnManager.AddController(PlayerController);
        }
        else
        {
            PlayerCharacter.SetLocation(Dungeon_MapManager.GetRandomTile(false, false).Location,true);
            Dungeon_TurnManager.AddController(PlayerController);
            
            // Player gets removed from CharacterList
            Characters.Add(PlayerCharacter);
            Dungeon_TurnManager.AddController(PlayerController);
        }
    }

    public void SpawnCharacter(GameObject entityPrefab)
    {
        var ntt = Instantiate(entityPrefab, transform.position, Quaternion.identity);
        
        // Character
        var character = ntt.GetComponent<Entity_Character>();

        character.InitEntity(Dungeon_MapManager.GetRandomTile(false, false).Location);
    }

    public void PlaceExit(Vector2Int location)
    {
        var exit = Instantiate(ExitPrefab, transform.position, Quaternion.identity);
        Exit = exit.GetComponent<Entity_Exit>();
        Exit.InitEntity(location);
    }

    // ==== Movement ==== //
    public static bool IsCharacterOnTile(Vector2Int v)
    {
        foreach (var c in Characters)
        {
            if (c.GetLocation() == v)
            {
                return true;
            }
        }

        return false;
    }

    private void RemoveAllEntities()
    {
        ExtendedDebugLibrary.Log("Removing " + Entities.Count + " Entities",EColor.red);
        for (int i = 0; i < Entities.Count; i++)
        {
            Dungeon_Entity e = Entities[i];
            if (e == null || e == PlayerCharacter)
                continue;
            
            e.RemoveEntity();
        }
    }
}
