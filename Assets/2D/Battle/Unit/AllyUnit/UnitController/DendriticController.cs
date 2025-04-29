using System;
using UnityEngine;

public class DendriticController : AllyUnitController, IInflammator, IPresenter
{
    private bool isHoldingAntigen = false;
    private int resourcePointWhenDamaged = 20;

    [SerializeField] private DetectionTrigger antigenDetect;
    [SerializeField] private Transform antigenCarryPosition;
    private AntigenController carriedAntigen;

    [SerializeField] private ParticleSystem inflammationEffect;

    public Action<int> InflammatoryActivated { get; set; }
    public Action<int> AntigenPresented { get; set; }

    protected override void Awake()
    {
        base.Awake();
        antigenDetect.enemyDetected += GetAntigen;
    }

    public void GetAntigen(UnitController antigen)
    {
        if (antigen is not AntigenController)
        {
            return;
        }
        else
        {
            carriedAntigen = (AntigenController)antigen;
            carriedAntigen.ObjectCarried();
            carriedAntigen.transform.parent = antigenCarryPosition;
            carriedAntigen.transform.position = antigenCarryPosition.position;

            ReturnToBase();
        }
    }
    public void ReturnToBase()
    {
        isReturning = true;
        GetNewWaypoint(false, isReturning);
    }
    public void ArrivedAtBase()
    {
        AntigenPresented?.Invoke(carriedAntigen.intelAmount);
        Destroy(this.gameObject);
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
