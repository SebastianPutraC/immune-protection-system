using UnityEngine;

[System.Serializable]
public class EncyclopediaData 
{
    public int UnitId; //Convert to UnitId Enum
    public bool Unlocked;

    public EncyclopediaData(int unitId, bool unlocked)
    {
        this.UnitId = unitId;
        this.Unlocked = unlocked;
    }
}
