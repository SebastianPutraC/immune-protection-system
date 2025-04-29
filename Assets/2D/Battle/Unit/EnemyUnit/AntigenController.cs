using UnityEngine;

public class AntigenController : UnitController
{
    public int intelAmount;
    private float destroyTimer = 3f;
    public bool isOnGround = false;

    public void InitializeAntigen(int intelAmount)
    {
        unitHp = 1f;
        this.intelAmount = intelAmount;
        isOnGround = true;
    }

    private void Update()
    {
        if (isOnGround)
        {
            SelfDestructTimer();   
        }
        else
        {
            destroyTimer = 3f;
        }
    }

    private void SelfDestructTimer()
    {
        if (destroyTimer <= 0)
        {
            Destroy(this.gameObject);
        }

        destroyTimer -= Time.deltaTime;
    }
    public void ObjectCarried()
    {
        isOnGround = false;
        Destroy(this.GetComponent<BoxCollider2D>()); //Change this prob
    }
}
