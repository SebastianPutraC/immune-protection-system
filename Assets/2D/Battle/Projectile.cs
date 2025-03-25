using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private Sprite sprite; // Will be used later
    private ProjectileType projectileType;

    private UnitController target;
    private Vector2 lastTargetPosition;

    private bool isInitialized = false;

    [SerializeField] private UnitSide unitSide; // Set it on unit script
    [SerializeField] private Rigidbody2D projectileRb;

    public void InitializeBulletProjectile(float speed, int damage, UnitController target)
    {
        this.speed = speed;
        this.damage = damage;
        this.target = target;
        lastTargetPosition = target.transform.position;

        isInitialized = true;
        projectileType = ProjectileType.BULLET;
    }

    private void Update()
    {
        /*if (isInitialized && target == null)
        {
            Destroy(gameObject);
        }*/
        if (isInitialized)
        {
            if (target != null)
            {
                lastTargetPosition = target.transform.position;
            }
            Move();
        }

        if (projectileType == ProjectileType.BULLET)
        {
            if (transform.position == (Vector3)lastTargetPosition)
            {
                Destroy(this.gameObject);
            }
        }
        else if (projectileType == ProjectileType.ARITLLERY)
        {
            if (transform.position == (Vector3)lastTargetPosition && !artilerryExploded)
            {
                artilerryExploded = true; // This is for particle
                AttackArtilerry();
            }
        }
    }

    protected void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, lastTargetPosition, speed * Time.deltaTime);
    }

    protected void Attack(UnitController unitHit)
    {
        if (unitHit != null)
        {
            unitHit.DealDamage(damage);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (projectileType == ProjectileType.BULLET)
        {
            if (unitSide == UnitSide.ENEMY)
            {
                if (collision.CompareTag("AllyUnit") || collision.CompareTag("AllyBase"))
                {
                    if (isInitialized)
                    {
                        Attack(collision.GetComponent<UnitController>());
                    }
                }
            }
            else
            {
                if (collision.CompareTag("EnemyUnit") || collision.CompareTag("EnemyBase"))
                {
                    Attack(collision.GetComponent<UnitController>());
                }
            }
        }
    }

    // Artilerry
    private float radius;
    private bool artilerryExploded = false;

    public void InitializeArtilleryProjectile(float speed, int damage, UnitController target, float radius)
    {
        this.speed = speed;
        this.damage = damage;
        this.target = target;
        this.radius = radius;

        lastTargetPosition = target.transform.position;

        isInitialized = true;
        projectileType = ProjectileType.ARITLLERY;
    }
    protected void AttackArtilerry()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        LayerMask enemy = LayerMask.GetMask("Default");

        if (unitSide == UnitSide.ALLY)
        {
           enemy = LayerMask.GetMask("Enemy");
        }
        else if (unitSide == UnitSide.ENEMY)
        {
            enemy = LayerMask.GetMask("Ally");
        }

        List<Collider2D> enemyOnAOE = Physics2D.OverlapCircleAll(this.transform.position, radius, enemy).ToList();

        for (int i = 0; i < enemyOnAOE.Count; i++)
        {
            if (enemyOnAOE[i].CompareTag("EnemyUnit"))
            {
                enemyOnAOE[i].gameObject.GetComponent<UnitController>().DealDamage(damage);
            }
        }

        this.GetComponent<ParticleSystem>().Play();
    }
}
