using System;
using UnityEngine;

public abstract class Dungeon_Controller : MonoBehaviour
{
    [SerializeField]
    protected Entity_Character Character;
    public void SetCharacter(Entity_Character character)
    {
        Character = character;
    }
    public Entity_Character GetCharacter()
    {
        return Character;
    }
}
