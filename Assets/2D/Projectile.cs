using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private Sprite sprite; // Will be used later
    private UnitController target;

    private bool isInitialized = false;

    [SerializeField] private UnitSide unitSide;
    [SerializeField] private Rigidbody2D projectileRb;

    public void InitializeProjectile(float speed, int damage, UnitController target)
    {
        this.speed = speed;
        this.damage = damage;
        this.target = target;

        isInitialized = true;
    }

    private void Update()
    {
        if (isInitialized && target == null)
        {
            Destroy(gameObject);
        }

        if (isInitialized && target != null)
        {
            Move();
        }
        else if (isArtilleryInitialized)
        {
            MoveArtilerry();
            if (transform.position == (Vector3)artilleryTargetPosition && !reachedTarget)
            {
                reachedTarget = true;
                AttackArtilerry();
            }
        }
    }

    protected void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
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
        if (!isArtilleryInitialized)
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


    //Artilerry Stuff
    private Vector2 artilleryTargetPosition;
    private float Radius;
    private bool isArtilleryInitialized = false;
    private bool reachedTarget = false;

    public void InitializeArtilleryProjectile(float speed, int damage, Vector2 target, float radius)
    {
        this.speed = speed;
        this.damage = damage;
        this.artilleryTargetPosition = target;
        this.Radius = radius;

        isArtilleryInitialized = true;
    }
    protected void MoveArtilerry()
    {
        transform.position = Vector2.MoveTowards(transform.position, artilleryTargetPosition, speed * Time.deltaTime);
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

        List<Collider2D> enemyOnAOE = Physics2D.OverlapCircleAll(this.transform.position, Radius, enemy).ToList();

        for (int i = 0; i < enemyOnAOE.Count; i++)
        {
            Debug.Log(enemyOnAOE[i]);
            if (enemyOnAOE[i].CompareTag("EnemyUnit"))
            {
                enemyOnAOE[i].gameObject.GetComponent<UnitController>().DealDamage(damage);
            }
        }

        this.GetComponent<ParticleSystem>().Play();
    }
}
