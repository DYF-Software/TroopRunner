using System.Collections;
using UnityEngine;

public class TroopCombatHandler : MonoBehaviour
{
    [SerializeField] private float combatTriggerDistance = 5f;
    [SerializeField] private float combatInterval = 0.2f;

    private TroopSpawner troopSpawner;
    private TroopForwardMover forwardMover;
    private bool inCombat = false;

    public bool IsInCombat => inCombat;

    private void Start()
    {
        troopSpawner = GetComponent<TroopSpawner>();
        forwardMover = GetComponent<TroopForwardMover>();
    }

    void Update()
    {
        if (inCombat) return;

        EnemySpawnPoint nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, nearestEnemy.transform.position);

            forwardMover.SetTarget(nearestEnemy.transform);

            if (distance < combatTriggerDistance)
            {
                StartCoroutine(HandleCombat(nearestEnemy));
            }
        }
        else
        {
            forwardMover.SetTarget(null);
        }
    }

    IEnumerator HandleCombat(EnemySpawnPoint enemySpawn)
    {
        inCombat = true;
        forwardMover.SetTarget(null); 

        while (troopSpawner.TroopCount > 0 && enemySpawn.EnemyCount > 0)
        {
            troopSpawner.RemoveOneTroop();
            enemySpawn.RemoveOneEnemy();
            yield return new WaitForSeconds(combatInterval);
        }

        inCombat = false;
    }

    EnemySpawnPoint FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = Mathf.Infinity;
        EnemySpawnPoint closest = null;

        foreach (GameObject enemy in enemies)
        {
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai == null || ai.spawnPoint == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = ai.spawnPoint;
            }
        }

        return closest;
    }
}
