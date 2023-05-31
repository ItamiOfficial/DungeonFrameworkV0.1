using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SSA_Stat
{
    [SerializeField]
    private int CurrentLevel;
    [SerializeField]
    private int MaxLevel;

    public Dictionary<SSA_Attribute,SSA_StatInfluence_Base_SO> InfluencedAttributes = new Dictionary<SSA_Attribute, SSA_StatInfluence_Base_SO>();

    public SSA_Stat(int level)
    {
        CurrentLevel = level;
        MaxLevel = 99;
    }
    public SSA_Stat(int level, int maxLevel)
    {
        CurrentLevel = level;
        MaxLevel = maxLevel;    
    }

    public int GetLevel()
    {
        return CurrentLevel;
    }
    public void RaiseLevel()
    {
        CurrentLevel = Mathf.Min(CurrentLevel + 1,MaxLevel);
    }

    // For each Attribute our stat Influences, we apply it here
    public void UpdateAttributes()
    {
        foreach (var pair in InfluencedAttributes)
        {
            pair.Key.SetBaseValue(pair.Value.GetStatValue(pair.Key,this));
        }
    }
    
}
