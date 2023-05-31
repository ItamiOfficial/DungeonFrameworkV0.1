using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Class to define the way a stat influences an Attribute
public abstract class SSA_StatInfluence_Base_SO : ScriptableObject
{
    public abstract float GetStatValue(SSA_Attribute attribute,SSA_Stat stat);
}
