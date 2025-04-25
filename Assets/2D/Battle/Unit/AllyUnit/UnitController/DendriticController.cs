using System;
using UnityEngine;

public class DendriticController : AllyUnitController, IInflammator
{
    private bool isHoldingAntigen = false;
    private int resourcePointWhenDamaged = 20;

    [SerializeField] private DetectionTrigger antigenDetect;
    [SerializeField] private Transform antigenCarryPosition;

    [SerializeField] private ParticleSystem inflammationEffect;

    public Action<int> InflammatoryActivated { get; set; }

    protected override void Awake()
    {
        base.Awake();
        antigenDetect.enemyDetected += GetAntigen;
    }

    protected void Update()
    {
        if (isMoveOrder)
        {
            Move();
        }
        else
        {
            Stop();
        }
    }

    private void GetAntigen(UnitController antigen)
    {

    }
    private void ReturnToBase()
    {

    }

    public override void DealDamage(float damage)
    {
        base.DealDamage(damage);
        InflammatoryActivated?.Invoke(resourcePointWhenDamaged);
        inflammationEffect.Play();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        antigenDetect.enemyDetected -= GetAntigen;
    }
}
