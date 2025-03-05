using UnityEngine;

[CreateAssetMenu(fileName = "AllyUnit", menuName = "Scriptable Objects/AllyUnit")]
public class AllyUnit : ScriptableObject
{
    public string UnitName;
    public int UnitHp;
    public int UnitAtk;
    public int UnitCost;

    [TextArea(5, 10)]
    public string UnitDesc;

    public Sprite UnitSprite;
}
