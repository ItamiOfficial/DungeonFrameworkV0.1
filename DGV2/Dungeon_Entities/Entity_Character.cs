using UnityEngine;

public class Entity_Character : Dungeon_Entity
{
    public override void RemoveEntity()
    {
        Dungeon_Manager.Characters.Remove(this);
        //ExtendedDebugLibrary.Log("Destroy gameobject" + gameObject.name,EColor.red);
        base.RemoveEntity();
    }

    public override void InitEntity(Vector2Int spawnPosition)
    {
        //Debug.Log("Entity Spawned: " + gameObject.name);
        Dungeon_Manager.Characters.Add(this);
        base.InitEntity(spawnPosition);
    }
}
