using UnityEngine;
using TMPro;

public class BattlePanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI allyBaseHpText;
    [SerializeField] private TextMeshProUGUI enemyBaseHpText;
    [SerializeField] private TextMeshProUGUI resourcePointText;
    [SerializeField] private TextMeshProUGUI intelPointText;

    public void UpdateAllyBaseUI(float baseHp)
    {
        allyBaseHpText.text = "HP : " + baseHp;
    }
    public void UpdateEnemyBaseUI(float baseHp)
    {
        enemyBaseHpText.text = "Enemy HP : " + baseHp;
    }
    public void UpdateResourcePointUI(float rp)
    {
        resourcePointText.text = "Resource Points : " + rp;
    }
    public void UpdateIntelPointUI(float currentIntel, float maxIntel)
    {
        intelPointText.text = "Intel Points : " + currentIntel + " / " + maxIntel;
    }
}
