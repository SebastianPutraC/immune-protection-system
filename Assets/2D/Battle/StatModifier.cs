using System;

[Serializable]
public class StatModifier
{
    public StatModType ModType;
    public StatType StatToMod;
    public float ModValue;
    public readonly int Order;
    public object Source;

    public StatModifier(float modValue, StatModType modType, StatType statType, int order, object source)
    {
        this.ModValue = modValue;
        this.ModType = modType;
        this.StatToMod = statType;
        this.Order = order;
        this.Source = source;
    }
}
