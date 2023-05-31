using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Influence Methods", menuName = "Stats & Attributes / Stat Influence Method", order = 3)]
public class SSA_StatInfluence_AdaptViaList : SSA_StatInfluence_Base_SO
{
    public int[] AttributeIncrements;
    
    public override float GetStatValue(SSA_Attribute attribute, SSA_Stat stat)
    {
        float val = 0;
        
        for (int i = 0; i < stat.GetLevel(); i++)
        {
            val += AttributeIncrements[i % AttributeIncrements.Length];
            Debug.Log(val);
        }
        
        return val;
    }
}
