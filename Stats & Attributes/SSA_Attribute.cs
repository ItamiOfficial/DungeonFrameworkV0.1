using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangedAttributeValueEvent : UnityEvent<float> { }

[Serializable]
public class SSA_Attribute 
{
    private static readonly bool AddFirst = true;
    
    [SerializeField] private float BaseValue;
    private SSA_Attribute MaxValue;
    

    private List<float> MOD_ADD = new List<float>();
    private List<float> MOD_MULT = new List<float>();

    public ChangedAttributeValueEvent OnValueChanged = new ChangedAttributeValueEvent();
    
    public SSA_Attribute(float baseValue)
    {
        BaseValue = baseValue;
    }
    public SSA_Attribute()
    {

    }
    
    public SSA_Attribute(float baseValue, SSA_Attribute maxValue)
    {
        MaxValue = maxValue;
        SetBaseValue(baseValue);
    }
    public SSA_Attribute(SSA_Attribute maxValue)
    {
        MaxValue = maxValue;
        SetBaseValue(MaxValue.GetCurrentValue());
    }

    public void SetMax(SSA_Attribute max)
    {
        MaxValue = max;
    }
    
    // ==== Setter ==== //
    public void SetBaseValue(float baseValue)
    {
        if (MaxValue == null)
        {
            BaseValue = baseValue;    
        } else {
            BaseValue = Mathf.Min(baseValue,MaxValue.GetCurrentValue());
        }

        OnValueChanged?.Invoke(GetCurrentValue());
    }

    // ==== Getter ==== //
    public int GetCurrentValue()
    {
        return Mathf.RoundToInt(CalculateValue());
    }

    public int GetBaseValue()
    {
        return Mathf.RoundToInt(BaseValue);
    }
    private float CalculateValue()
    {
        var value = BaseValue;

        if (AddFirst)
        {
            foreach (var a in MOD_ADD)
            {
                value += a;
            }

            foreach (var m in MOD_MULT)
            {
                value *= 1 + m;
            }
        } else {
            foreach (var m in MOD_MULT)
            {
                value *= 1 + m;
            }  
            
            foreach (var a in MOD_ADD)
            {
                value += a;
            }
        }
        return value;
    }
    
    // ==== Adder ==== //
    // Hier Weiter Machen
}
