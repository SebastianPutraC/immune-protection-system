using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    public UnitId unitId;

    public string UnitName;
    public float UnitHp;
    public int UnitCost;
    public float UnitSpeed;
    public AttackType attackType;

    public float UnitAtk;
    public float UnitAtkSpeed;
    public float UnitXRange;
    public float UnitYRange;

    public float UnitRangedAtk;
    public float unitRangedAtkSpeed;
    public float unitRangedXRange;
    public float unitRangedYRange;
    public Projectile rangedProjectile;

    public float unitArtilleryOffset;
    public float unitArtilleryRadius;

    [TextArea(5, 10)]
    public string UnitDesc;

    public Sprite UnitSprite;

    public List<AbilityType> abilityType;
}
