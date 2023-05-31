using System;

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Dungeon Floor", menuName = "Dungeon/Dungeon Floor", order = 2)]
public class Dungeon_Floor_SO : ScriptableObject
{ 
    // ==== Generation Method ==== //
    [Header("Generation Method")] 
    public Dungeon_MapGenMethod_Base_SO Layout;
    
    // ==== Encounters ==== //
    [Header("Items")] 
    [Range(0, 10)] public int MinItemCount = 2;
    [Range(0, 10)] public int MaxItemCount = 5;
    public List<ChanceWrapper<GameObject>> Items;
    
    [Header("Enemies")] 
    [Range(0, 10)] public int MinEnemyCount = 3;
    [Range(0, 10)] public int MaxEnemyCount = 5;
    public List<ChanceWrapper<GameObject>> Enemies;
    
    [Header("NPC")] 
    // NPC's are Gurentied Spots
    public List<ChanceWrapper<GameObject>> NPC;

    public int GetRandomEntityCount()
    {
        return Random.Range(MinEnemyCount, MaxEnemyCount + 1);
    }

    public GameObject GetRandomEnemy()
    {
        return Enemies[Random.Range(0,Enemies.Count)].Object;
    }
}



// ==== A Wrapper Class to get give Objects a weight, and which takes a List (could easily be adjusted to work with an array) ==== 
[Serializable]
public class ChanceWrapper<T>
{
    public T Object;
    public float Chance = 1;
    
    public ChanceWrapper(T obj)
    {
        Object = obj;
        
    }

    public static T GetRandomEntity(List<ChanceWrapper<T>> list)
    {
        // Algorythm to get random weighted entity
        float randomChance = Random.Range(0,GetTotalWeightOfList(list));
        ChanceWrapper<T> theChosenOne = null;

        foreach (var entry in list) {
            if (randomChance <= entry.Chance) 
            {
                theChosenOne = entry;
                break;
            }

            randomChance -= theChosenOne.Chance;
        }

        return theChosenOne.Object;

    }

    private static float GetTotalWeightOfList(List<ChanceWrapper<T>> list)
    {
        float sum = 0;
        
        foreach (var entry in list) {
            sum += entry.Chance;
        }
        
        return sum;
    }

}
