using UnityEngine;

[CreateAssetMenu(fileName = "EncyclopediaAbilitySO", menuName = "Scriptable Objects/EncyclopediaAbilitySO")]
public class EncyclopediaAbilitySO : ScriptableObject
{
    public Sprite AbilityImage; 
    public string AbilityName;
    [TextArea (5, 10)]
    public string AbilityDescription;
}
