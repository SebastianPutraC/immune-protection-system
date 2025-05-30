using System;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public int spawnId;

    private float defendCooldown;
    private readonly float maxCooldown = 15f;

    public Action<int, UnitId> DefendBase;

    public void InitializeSpawn(int spawnId)
    {
        this.spawnId = spawnId;
    }

    private void Update()
    {
        if (defendCooldown > 0)
        {
            defendCooldown -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AllyUnit"))
        {
            if (defendCooldown <= 0)
            {
                defendCooldown = maxCooldown;
                CallForHelp(collision.GetComponent<DeployedUnitController>().UnitSO.unitId);
            }
        }
    }
    private void CallForHelp(UnitId unitId)
    {
        defendCooldown = maxCooldown;
        DefendBase?.Invoke(spawnId, unitId);
    }
}
