using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DungeonEntity_SAA_Component : SSA_EntityComponent
{
    public int Level;
    [Header("Stats")]
    public  SSA_Stat S_Vigor = new SSA_Stat(10);
    
    [Header("Attributes")]
    public SSA_Attribute A_MaxHealth = new SSA_Attribute();
    public SSA_Attribute A_CurrentHealth = new SSA_Attribute();

    public SSA_StatInfluence_Base_SO InfluenceMethod;
    
    private void Awake()
    {
        S_Vigor.InfluencedAttributes.Add(A_MaxHealth,InfluenceMethod);
        S_Vigor.UpdateAttributes();
        
        A_CurrentHealth.SetMax(A_MaxHealth);

        Level = S_Vigor.GetLevel();
    }
}
