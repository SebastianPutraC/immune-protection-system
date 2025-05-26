using System;
using System.Collections.Generic;

[Serializable]
public class UnitStat
{
    public StatType Type;
    public float BaseValue;
    public float FinalValue { get
        {
            return ReturnFinalValue();
        }
    }
    public List<StatModifier> statModifiers = new List<StatModifier>();

    public UnitStat (float statValue, StatType type)
    {
        this.Type = type;
        this.BaseValue = statValue;
    }
    public UnitStat(float statValue, StatType type, List<StatModifier> statModifiers)
    {
        this.Type = type;
        this.BaseValue = statValue;
        this.statModifiers = statModifiers;
    }

    public void AddModifier(StatModifier mod)
    {
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
    }
    private int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
        {
            return -1;
        }
        else if (a.Order > b.Order)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            return true;
        }
        return false;
    }
    public bool RemoveModifersFromSource(int source)
    {
        bool removed = false;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            if (statModifiers[i].Source == source)
            {
                removed = true;
                statModifiers.RemoveAt(i);
            }
        }

        return removed;
    }
    public StatModifier GetStatFromSource(int source)
    {
        for (int i = 0; i < statModifiers.Count; i++)
        {
            if (statModifiers[i].Source == source)
            {
                return statModifiers[i];
            }
        }

        return null;
    }

    public float ReturnFinalValue()
    {
        float finalValue = BaseValue;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];
            float sumPercentAdd = 0;

            switch(mod.ModType)
            {
                case StatModType.FLAT:
                    finalValue += mod.ModValue;
                    break;
                case StatModType.PERCENTMULT:
                    finalValue *= 1 + mod.ModValue; 
                    break;
                case StatModType.PERCENTADD:
                    sumPercentAdd += mod.ModValue;

                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].ModType != StatModType.PERCENTADD)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                    break;
            }
            finalValue += statModifiers[i].ModValue;
        }

        return finalValue;
    }
}
