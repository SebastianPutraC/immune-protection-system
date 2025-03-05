using UnityEngine;

[CreateAssetMenu(fileName = "EnemyUnit", menuName = "Scriptable Objects/EnemyUnit")]
public class EnemyUnit : ScriptableObject
{
    public string UnitName;
    public int UnitHp;
    public int UnitAtk;
    public int UnitCost;

    [TextArea(5, 10)]
    public string UnitDesc;

    public Sprite UnitSprite;
}
