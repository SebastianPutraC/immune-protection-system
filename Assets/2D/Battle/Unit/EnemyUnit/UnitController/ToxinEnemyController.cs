using System.Linq;
using UnityEngine;

public class ToxinEnemyController : EnemyUnitController
{
    [SerializeField] protected ParticleSystem AoeEffect;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (isDetecting)
        {
            if (meleeCooldown > 0)
            {
                meleeCooldown -= Time.deltaTime;
            }
            else
            {
                ToxinAttack();
                meleeCooldown = meleeCooldownSpeed.ReturnFinalValue();
            }
        }
        else
        {
            AoeEffect.Stop();
        }
    }
    protected void ToxinAttack()
    {
        AoeEffect.Play();
        if (targetEnemyUnitMelee != null)
        {
            foreach (DeployedUnitController allyUnit in detectedUnitMelee.Values.ToList())
            {
                allyUnit.DealDamage(unitAtk.ReturnFinalValue());
            }
        }
    }



    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
