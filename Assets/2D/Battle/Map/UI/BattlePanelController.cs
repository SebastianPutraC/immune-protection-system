using UnityEngine;
using TMPro;

public class BattlePanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI allyBaseHpText;
    [SerializeField] private TextMeshProUGUI enemyBaseHpText;
    [SerializeField] private TextMeshProUGUI resourcePointText;

    public void UpdateAllyBaseUI(int baseHp)
    {
        allyBaseHpText.text = "HP : " + baseHp;
    }

    public void UpdateEnemyBaseUI(int baseHp)
    {
        enemyBaseHpText.text = "Enemy HP : " + baseHp;
    }

    public void UpdateResourcePointUI(int rp)
    {
        resourcePointText.text = "Resource Points : " + rp;
    }
}
