using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Exit : Dungeon_Entity
{
    public virtual void GoToNextFloor()
    {
        ExtendedDebugLibrary.Log("Loading Floor",EColor.blue);
        Dungeon_Manager.Instance.GenerateNextFloor();
    }
}
