using UnityEngine;
using TMPro;

public class BattlePanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI allyBaseHpText;
    [SerializeField] private TextMeshProUGUI enemyBaseHpText;
    [SerializeField] private TextMeshProUGUI resourcePointText;
    [SerializeField] private TextMeshProUGUI intelPointText;

    [SerializeField] private TutorialController tutorial;

    public void UpdateAllyBaseUI(int baseHp, int maxHp)
    {
        allyBaseHpText.text = "HP :\n" + baseHp + " / " + maxHp;
    }
    public void UpdateEnemyBaseUI(int currRp, int maxRp)
    {
        enemyBaseHpText.text = "Repair Points :\n" + currRp + " / " + maxRp;
    }
    public void UpdateResourcePointUI(float rp, float maxRp)
    {
        resourcePointText.text = "Resource Points : " + rp + " / " + maxRp;
    }
    public void UpdateIntelPointUI(float currentIntel, float maxIntel)
    {
        intelPointText.text = "Intel Points : " + currentIntel + " / " + maxIntel;
    }
}
