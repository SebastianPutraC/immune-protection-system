using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecisionInfoSO", menuName = "Scriptable Objects/DecisionInfoSO")]
public class DecisionInfoSO : ScriptableObject
{
    public int decisionId;
    public Sprite DecisionInfoImage;

    [TextArea(5, 10)]
    public string DecisionInfoText;

    public List<DecisionButtonSO> DecisionButtons;
}
