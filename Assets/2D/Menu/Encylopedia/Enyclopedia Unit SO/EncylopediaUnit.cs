using UnityEngine;

[CreateAssetMenu(fileName = "EncylopediaUnit", menuName = "Scriptable Objects/EncylopediaUnit")]
public class EncylopediaUnit : ScriptableObject
{
    public UnitId unitId;
    public UnitType unitType;
    public string unitName;
    public Sprite unitSprite;
    public string unitDescription;
}
