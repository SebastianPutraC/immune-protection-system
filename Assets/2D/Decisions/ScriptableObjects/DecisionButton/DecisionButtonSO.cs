using UnityEngine;

[CreateAssetMenu(fileName = "DecisionButtonSO", menuName = "Scriptable Objects/DecisionButtonSO")]
public class DecisionButtonSO : ScriptableObject
{
    public string DecisionButtonText;
    public StatModifier StatMod;

    public bool IsPickNextInfo;
    public int NextDecisionID;

    public bool IsQuiz;
    public bool IsCorrect;
}
