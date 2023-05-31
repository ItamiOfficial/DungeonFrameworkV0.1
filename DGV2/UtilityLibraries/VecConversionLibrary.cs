using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VecConversionLibrary 
{
    public static Vector3 Vec2To3(Vector2Int v)
    {
        return new Vector3(v.x,v.y,0);
    }
}
