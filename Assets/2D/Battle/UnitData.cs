using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    public string UnitName;
    public int UnitHp;
    public int UnitCost;
    public float UnitSpeed;
    public AttackType attackType;

    public int UnitAtk;
    public float UnitAtkSpeed;
    public float UnitXRange;
    public float UnitYRange;

    public int UnitRangedAtk;
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
