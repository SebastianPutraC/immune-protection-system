using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeployedUnitController : UnitController
{
    public UnitData UnitSO;
    [SerializeField]protected UnitStat unitAtk;
    [SerializeField]protected UnitStat unitRangedAtk;

    public bool isMoveOrder = true;
    protected bool isDetecting = false;
    protected bool isRangedDetecting = false;
    protected bool noMoreRoute = false;
    protected bool isReturning = false;
    protected bool isAbilityLocked = false; //USE THIS TO LOCK USE OF ABILITY, HIDE ABILITY ON STAT PANEL, HIDE IT ASAP

    protected float meleeCooldown;
    protected float rangedCooldown;

    [SerializeField] protected SpriteRenderer unitSprite;
    [SerializeField] protected Rigidbody2D unitRb;
    [SerializeField] protected DetectionTrigger enemyDetect;
    [SerializeField] protected DetectionTrigger enemyRangedDetect;
    [SerializeField] protected Slider healthBar;

    [SerializeField] protected UnitController targetEnemyUnitMelee;
    [SerializeField] protected UnitController targetEnemyUnitRanged;

    [SerializeField] protected Dictionary<int, UnitController> detectedUnitMelee = new Dictionary<int, UnitController>();
    [SerializeField] protected Dictionary<int, UnitController> detectedUnitRanged = new Dictionary<int, UnitController>();

    protected Route currentRouteTaken;
    protected int currentWaypointIndex;
    protected Transform currentTargetWaypoint;
    protected bool isArrived = true;

    protected virtual void Awake()
    {
        if(enemyDetect != null)
        {
            enemyDetect.enemyDetected += EnemyMeleeDetected;
            enemyDetect.enemyOutRange += EnemyMeleeLost;
        }
        if (enemyRangedDetect != null)
        {
            enemyRangedDetect.enemyDetected += EnemyRangedDetected;
            enemyRangedDetect.enemyOutRange += EnemyRangedLost;
        }
    }

    public virtual void InitializeUnit(UnitData unitSO, List<StatModifier> modifiers, Route initialRoute)
    {
        UnitSO = unitSO;
        unitHp = AdjustMaxHP(GetModifiersForType(StatType.HP, modifiers), UnitSO.UnitHp);
        unitAtk = new UnitStat(UnitSO.UnitAtk, StatType.ATK, GetModifiersForType(StatType.ATK, modifiers));
        unitRangedAtk = new UnitStat(UnitSO.UnitRangedAtk, StatType.RANGEDATK, GetModifiersForType(StatType.RANGEDATK, modifiers));
        unitSprite.sprite = UnitSO.UnitSprite;
        meleeCooldown = UnitSO.UnitAtkSpeed;
        rangedCooldown = UnitSO.UnitRangedAtkSpeed;
        unitSprite.size = new Vector2(1, 1); 
        UpdateHealthBar(unitHp, UnitSO.UnitHp);

        SetNewRoute(initialRoute);
    }
    protected List<StatModifier> GetModifiersForType(StatType statType, List<StatModifier> allModifiers)
    {
        return allModifiers.Where(mod => mod.StatToMod == statType).ToList();
    }
    public float AdjustMaxHP(List<StatModifier> hpModifier, float baseHp) // Only At Start
    {
        UnitStat maxHp = new UnitStat(baseHp, StatType.HP);
        for (int i = 0; i < hpModifier.Count; i++)
        {
            maxHp.AddModifier(hpModifier[i]);
        }
        return maxHp.ReturnFinalValue();
    }

    protected virtual void Update()
    {
        if (Vector2.Distance(currentTargetWaypoint.position, transform.position) <= 0.1f)
        {
            if (!isArrived)
            {
                isArrived = true;
                GetNewWaypoint(false, isReturning);
            }
        }
    }
    protected void FixedUpdate()
    {
        if (isMoveOrder && !isDetecting && !isRangedDetecting && !noMoreRoute && !isArrived && currentTargetWaypoint != null )
        {
            Move();
        }
        else
        {
            Stop();
        }
    }

    protected virtual void GetNewWaypoint(bool isStart, bool isReturning = false)
    {
        if (currentRouteTaken != null)
        {
            Transform waypoint = currentRouteTaken.GetNextWaypoint(currentWaypointIndex);

            if (waypoint == null)
            {
                Debug.Log(this.gameObject + "waypoint = null");
                UnitSide unitside = UnitSide.NONE;
                if (this is EnemyUnitController || isReturning)
                {
                    unitside = UnitSide.ENEMY;
                }
                else if (this is AllyUnitController)
                {
                    unitside = UnitSide.ALLY;
                }

                int nextRoute = currentRouteTaken.GetNextRoute(1, unitside);
                if (nextRoute == -1)
                {
                    isArrived = true;
                    noMoreRoute = true;
                    return;
                }
                UnitArrivedAtWaypoint?.Invoke(nextRoute, this);
            }
            else
            {
                currentTargetWaypoint = waypoint;
                isArrived = false;
                Debug.Log(this.gameObject + "new = " + currentTargetWaypoint);
            }
        }
    }
    public virtual void SetNewRoute(Route route)
    {

    }


    public virtual void ProcessAbility(AbilityId ability)
    {
        switch(ability)
        {
            case AbilityId.MOVE:
                if (isMoveOrder)
                {
                    isMoveOrder = false;
                }
                else
                {
                    isMoveOrder = true;
                }
                break;
            default:
                break;
        }
    }

    protected virtual void Move()
    {
        Debug.Log(this.gameObject + " : " + currentTargetWaypoint.position);
        Vector2 direction = (currentTargetWaypoint.position - transform.position).normalized;
        unitRb.linearVelocity = direction * UnitSO.UnitSpeed;

        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (this is EnemyUnitController || (this is AllyUnitController && isReturning))
        {
            transform.rotation = Quaternion.Euler(180, 0, -rotationZ);
        }
        else if (this is EnemyUnitController)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }
    }
    protected void Stop() //Change UI 
    {
        unitRb.linearVelocity = new Vector2(0, 0);
    }
    

    #region Detection
    protected void EnemyMeleeDetected(UnitController enemyUnit)
    {
        isDetecting = true;
        Stop();

        if (!detectedUnitMelee.ContainsKey(enemyUnit.GetInstanceID()))
        {
            detectedUnitMelee.Add(enemyUnit.GetInstanceID(), enemyUnit);
            if (!detectedUnitRanged.ContainsKey(enemyUnit.GetInstanceID()))
            {
                enemyUnit.ThisUnitKilled += EnemyDead;
            }

            if (targetEnemyUnitMelee == null)
            {
                targetEnemyUnitMelee = enemyUnit;
            }
        }
    }
    protected void EnemyRangedDetected(UnitController enemyUnit)
    {
        isRangedDetecting = true;
        Stop();

        if (!detectedUnitRanged.ContainsKey(enemyUnit.GetInstanceID()))
        {
            detectedUnitRanged.Add(enemyUnit.GetInstanceID(), enemyUnit);
            if (!detectedUnitMelee.ContainsKey(enemyUnit.GetInstanceID()))
            {
                enemyUnit.ThisUnitKilled += EnemyDead;
            }

            if (targetEnemyUnitRanged == null)
            {
                targetEnemyUnitRanged = enemyUnit;
            }
        }
    }
    protected void EnemyMeleeLost(UnitController enemyUnit)
    {
        if (detectedUnitMelee.ContainsKey(enemyUnit.GetInstanceID()))
        {
            if (enemyUnit == targetEnemyUnitMelee)
            {
                targetEnemyUnitMelee = null;
            }

            detectedUnitMelee.Remove(enemyUnit.GetInstanceID());

            if (targetEnemyUnitMelee == null && detectedUnitMelee.Count <= 0)
            {
                List<UnitController> values = detectedUnitMelee.Values.ToList();
                UnitController closestEnemy = GetClosestEnemy(values);
                targetEnemyUnitMelee = closestEnemy;
            }
        }

        if (detectedUnitMelee.Count <= 0)
        {
            isDetecting = false;
        }
    }
    protected void EnemyRangedLost(UnitController enemyUnit)
    {
        if (detectedUnitRanged.ContainsKey(enemyUnit.GetInstanceID()))
        {
            if (enemyUnit == targetEnemyUnitRanged)
            {
                targetEnemyUnitRanged = null;
            }

            detectedUnitRanged.Remove(enemyUnit.GetInstanceID());

            if (targetEnemyUnitRanged == null && detectedUnitRanged.Count <= 0)
            {
                List<UnitController> values = detectedUnitRanged.Values.ToList();
                UnitController closestEnemy = GetClosestEnemy(values);
                targetEnemyUnitRanged = closestEnemy;
            }
        }

        if (detectedUnitRanged.Count <= 0)
        {
            isRangedDetecting = false;
        }
    }
    protected UnitController GetClosestEnemy(List<UnitController> enemies)
    {
        UnitController bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (UnitController potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
    protected void EnemyDead(UnitController enemyUnit)
    {
        enemyUnit.ThisUnitKilled -= EnemyDead;
        EnemyMeleeLost(enemyUnit);
        EnemyRangedLost(enemyUnit);
    }
    #endregion Detection

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UnitLimit"))
        {
            DealDamage(9999);
        }
    }

    protected void UpdateHealthBar(float currentValue, float maxValue)
    {
        healthBar.value = currentValue / maxValue;
    }
    public override void DealDamage(float damage)
    {
        base.DealDamage(damage);
        UpdateHealthBar(unitHp, UnitSO.UnitHp);
    }

    protected virtual void OnDestroy()
    {
        if (enemyDetect != null)
        {
            enemyDetect.enemyDetected -= EnemyMeleeDetected;
            enemyDetect.enemyOutRange -= EnemyMeleeLost;
        }
        if (enemyRangedDetect != null)
        {
            enemyRangedDetect.enemyDetected -= EnemyRangedDetected;
            enemyRangedDetect.enemyOutRange -= EnemyRangedLost;
        }
    }
}
