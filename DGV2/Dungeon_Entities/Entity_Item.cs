using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Item : Dungeon_Entity
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void RemoveEntity()
    {
        Dungeon_Manager.Items.Remove(this);
        base.RemoveEntity();
    }
    
    public override void InitEntity(Vector2Int spawnPosition)
    {
        Dungeon_Manager.Items.Add(this);
        base.InitEntity(spawnPosition);
    }
}
