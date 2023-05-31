using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon", menuName = "Dungeon/Dungeon", order = 1)]
public class Dungeon_Dungeon_SO : ScriptableObject
{
    public string DungeonName;
    public string DungeonDescription;
    public Dungeon_Floor_SO[] Floors;
    
}
