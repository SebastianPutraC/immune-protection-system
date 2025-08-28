using UnityEngine;

[CreateAssetMenu(fileName = "EncyclopediaAbilitySO", menuName = "Scriptable Objects/EncyclopediaAbilitySO")]
public class EncyclopediaAbilitySO : ScriptableObject
{
    public AbilityId AbilityID;
    public string AbilityEncyclopediaName;
    public Sprite AbilityEncyclopediaImage;

    public Sprite AbilityPrimaryImage;
    public Sprite AbilitySecondaryImage;

    public string AbilityPrimaryName;
    public string AbilitySecondaryName;

    public bool IsManual;

    [TextArea (5, 10)]
    public string AbilityDescription;
}
