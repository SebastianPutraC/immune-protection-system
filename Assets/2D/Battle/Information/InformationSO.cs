using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InformationSO", menuName = "Scriptable Objects/InformationSO")]
public class InformationSO : ScriptableObject
{
    public Sprite InfoMascot;
    public Sprite InfoImage;

    [TextArea(5, 10)]
    public string InfoText;
}
