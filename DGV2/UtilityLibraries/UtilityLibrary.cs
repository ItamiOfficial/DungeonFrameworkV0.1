using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityLibrary 
{
    public static bool GetRandomBool()
    {
        return (Random.value > 0.5f);
    }
}
