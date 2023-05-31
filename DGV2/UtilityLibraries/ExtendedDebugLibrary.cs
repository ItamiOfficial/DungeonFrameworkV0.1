using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// ==== Coloring ==== //
public enum EColor {red,blue,green,white,black}
public static class ExtendedDebugLibrary 
{
    public static void Log(string s,EColor color)
    {
        string p = "<color=" + GetColor(color) + ">" + s + "</color>";
        Debug.Log(p);
    }

    
    
    // ==== Coloring ==== //
    private static string GetColor(EColor color)
    {
        switch (color)
        {
            case EColor.black:
                return "black";
            case EColor.blue:
                return "blue";
            case EColor.green:
                return "green";
            case EColor.red:
                return "red";
            case EColor.white:
                return "white";
            default:
                return "white";
        }
    }
}


