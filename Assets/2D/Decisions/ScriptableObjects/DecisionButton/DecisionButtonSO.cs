using UnityEngine;

[CreateAssetMenu(fileName = "DecisionButtonSO", menuName = "Scriptable Objects/DecisionButtonSO")]
public class DecisionButtonSO : ScriptableObject
{
    public string DecisionButtonText;
    public UnitSide SideToBoost;
    public StatModifier StatMod;
}
