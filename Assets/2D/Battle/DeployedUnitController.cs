using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeployedUnitController : UnitController
{
    public UnitData UnitSO;
    protected int unitAtk;
    protected int unitRangedAtk;
    protected AttackType attackType;

    public bool isMoveOrder = true; // public for now
    protected bool isDetecting = false;
    protected bool isRangedDetecting = false;

    private float meleeCooldown;
    private float rangedCooldown;
    private float deathTimer;

    [SerializeField] protected SpriteRenderer unitSprite;
    [SerializeField] protected Rigidbody2D unitRb;
    [SerializeField] protected DetectionTrigger enemyDetect;
    [SerializeField] protected DetectionTrigger enemyRangedDetect;
    [SerializeField] protected Slider healthBar;

    [SerializeField] protected Transform projectileSpawn;
    [SerializeField] protected ParticleSystem AoeEffect;

    [SerializeField] protected UnitController targetEnemyUnitMelee;
    [SerializeField] protected UnitController targetEnemyUnitRanged;

    [SerializeField] protected Dictionary<int, UnitController> detectedUnitMelee = new Dictionary<int, UnitController>();
    [SerializeField] protected Dictionary<int, UnitController> detectedUnitRanged = new Dictionary<int, UnitController>();

    protected void Awake()
    {
        enemyDetect.enemyDetected += EnemyDetected;
        enemyDetect.enemyOutRange += EnemyGoneMelee;
        enemyRangedDetect.enemyDetected += EnemyRangedDetected;
        enemyRangedDetect.enemyOutRange += EnemyGoneRanged;
    }

    public virtual void InitializeUnit(UnitData unitSO)
    {
        UnitSO = unitSO;
        unitHp = UnitSO.UnitHp;
        unitAtk = UnitSO.UnitAtk;
        unitRangedAtk = UnitSO.UnitRangedAtk;
        attackType = UnitSO.attackType;
        unitSprite.sprite = UnitSO.UnitSprite;
        meleeCooldown = UnitSO.UnitAtkSpeed;
        rangedCooldown = UnitSO.unitRangedAtkSpeed;
        unitSprite.size = new Vector2(1, 1); // May change on custom transform size
        UpdateHealthBar(unitHp, UnitSO.UnitHp);

        //Might make its own function
        if (attackType.HasFlag(AttackType.MELEE))
        {
            enemyDetect.InitializeRange(UnitSO.UnitXRange, UnitSO.UnitYRange, AttackType.MELEE);
            enemyDetect.gameObject.SetActive(true);
        }
        else if (attackType.HasFlag(AttackType.AOE))
        {
            enemyDetect.InitializeRange(UnitSO.UnitXRange, UnitSO.UnitYRange, AttackType.AOE);
            enemyDetect.gameObject.SetActive(true);
        }
        if (attackType.HasFlag(AttackType.RANGED))
        {
            enemyRangedDetect.InitializeRange(UnitSO.unitRangedXRange, UnitSO.unitRangedYRange, AttackType.RANGED);
            enemyRangedDetect.gameObject.SetActive(true);
        }
        else if (attackType.HasFlag(AttackType.ARTILERRY))
        {
            enemyRangedDetect.InitializeRange(UnitSO.unitRangedXRange, UnitSO.unitRangedYRange, AttackType.ARTILERRY);
            enemyRangedDetect.gameObject.SetActive(true);
        }    

    }

    protected void Update()
    {
        // Melee priority for now
        if (isDetecting) 
        {
            if (meleeCooldown > 0)
            {
                meleeCooldown -= Time.deltaTime;
            }
            else
            {
                if (attackType.HasFlag(AttackType.MELEE))
                {
                    AttackMelee();
                }
                else if (attackType.HasFlag(AttackType.AOE))
                {
                    AttackAOE();
                }
                meleeCooldown = UnitSO.UnitAtkSpeed;
            }
        }
        else if (isRangedDetecting)
        {
            if (rangedCooldown > 0)
            {
                rangedCooldown -= Time.deltaTime;
            }
            else
            {
                if (attackType.HasFlag(AttackType.RANGED))
                {
                    AttackRanged();
                }
                else if (attackType.HasFlag(AttackType.ARTILERRY))
                {
                    AttackArtillery();
                }
                rangedCooldown = UnitSO.unitRangedAtkSpeed;
            }
        }
        else
        {
            AoeEffect.Stop();
            if (isMoveOrder)
            {
                Move();
            }
            else
            {
                Stop();
            }
        }
    }
    public void ProcessAbility(AbilityType ability)
    {
        switch(ability)
        {
            case AbilityType.STOP:
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
        unitRb.linearVelocityX = UnitSO.UnitSpeed;
    }
    protected void Stop() //Change UI 
    {
        unitRb.linearVelocityX = 0;
    }
    

    #region Detection
    protected void EnemyDetected(UnitController enemyUnit) //Set up Attack Priority
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
    protected void EnemyRangedDetected(UnitController enemyUnit) // Set up Attack Priority
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
    protected void EnemyGoneMelee(UnitController enemyUnit)
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
    protected void EnemyGoneRanged(UnitController enemyUnit)
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
        EnemyGoneMelee(enemyUnit);
        EnemyGoneRanged(enemyUnit);
    }
    #endregion Detection

    #region Attack
    protected void AttackMelee()
    {
        if (targetEnemyUnitMelee != null)
        {
            targetEnemyUnitMelee.DealDamage(unitAtk);
        }
    }
    protected void AttackRanged()
    {
        if (targetEnemyUnitRanged != null)
        {
            Projectile spawnProjectile = Instantiate(UnitSO.rangedProjectile, projectileSpawn);
            spawnProjectile.InitializeProjectile(10, UnitSO.UnitRangedAtk, targetEnemyUnitRanged); // 10 is placeholder
        }
    }
    protected virtual void AttackAOE()
    {
        AoeEffect.Play();
    }
    protected void AttackArtillery()
    {
        if (targetEnemyUnitRanged != null)
        {
            Projectile spawnProjectile = Instantiate(UnitSO.rangedProjectile, projectileSpawn);

            float targetX = targetEnemyUnitRanged.transform.position.x;
            float predictedPositionX = targetX - 5; //5 Is Placeholder
            Vector2 predictedPosition = new Vector2(predictedPositionX, targetEnemyUnitRanged.transform.position.y);

            spawnProjectile.InitializeArtilleryProjectile(10, UnitSO.UnitRangedAtk, 
                predictedPosition, UnitSO.unitArtilleryRadius); // 10 is placeholder
        }
    }
    #endregion


    protected void UpdateHealthBar(float currentValue, float maxValue)
    {
        healthBar.value = currentValue / maxValue;
    }
    public override void DealDamage(int damage)
    {
        base.DealDamage(damage);
        UpdateHealthBar(unitHp, UnitSO.UnitHp);
    }

    protected void OnDestroy()
    {
        enemyDetect.enemyDetected -= EnemyDetected;
        enemyDetect.enemyOutRange -= EnemyGoneMelee;
        enemyRangedDetect.enemyDetected -= EnemyRangedDetected;
        enemyRangedDetect.enemyOutRange -= EnemyGoneRanged;
    }
}
