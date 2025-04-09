using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    public UnitId unitId;

    public string UnitName;
    public UnitType unitType;

    public float UnitHp;
    public int UnitCost;
    public float UnitSpeed;
    public AttackType attackType;
    public float UnitAtk;
    public float UnitAtkSpeed;
    public float UnitXRange;
    public float UnitYRange;

    public float UnitRangedAtk;
    public float UnitRangedAtkSpeed;
    public float UnitRangedXRange;
    public float UnitRangedYRange;
    public Projectile RangedProjectile;

    public float UnitArtilleryOffset;
    public float UnitArtilleryRadius;

    [TextArea(5, 10)]
    public string UnitSimpleDesc;
    [TextArea(5, 10)]
    public string UnitFullDesc;

    public Sprite UnitSprite;

    public List<AbilityType> AbilityType;

    public List<EncyclopediaAbilitySO> UnitAbility;
}
