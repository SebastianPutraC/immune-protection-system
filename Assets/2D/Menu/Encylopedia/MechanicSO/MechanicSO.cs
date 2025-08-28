using UnityEngine;

[CreateAssetMenu(fileName = "MechanicSO", menuName = "Scriptable Objects/MechanicSO")]
public class MechanicSO : ScriptableObject
{
    public string mechanicTitle;
    [TextArea(5, 10)]
    public string mechanicDescription;
}
