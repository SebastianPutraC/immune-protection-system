using UnityEngine;
using TMPro;

public class BonusPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bonusText;

    public void UpdateBonusText(StatType stat, float bonus)
    {
        if (stat == StatType.NONE)
        {
            return;
        }

        string bonusTextToAdd = "";
        if (bonusText.text == "")
        {
            bonusTextToAdd = stat.ToString() + " : " + bonus * 100 + "%";
        }
        else
        {
            bonusTextToAdd = "<br>" + stat.ToString() + " : " + bonus * 100 + "%";
        }

        bonusText.text += bonusTextToAdd;
    }
}
