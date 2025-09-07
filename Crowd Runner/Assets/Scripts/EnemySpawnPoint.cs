using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections.Generic;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("Bu noktada üretilecek düþman sayýsý")]
    public int enemyCount = 3;

    [Tooltip("Spawn edilecek düþman prefabý")]
    public GameObject enemyPrefab;

    [Tooltip("TextMesh nesnesi (opsiyonel: düþman sayýsýný gösterir)")]
    public TextMeshPro enemyCountText;

    [Header("Formasyon Ayarlarý")]
    public float spacing = 0.3f;
    public float goldenAngle = 137.5f;

    [Header("VFX")]
    public GameObject deathEffectPrefab;
    public float deathEffectLifetime = 1f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public int CurrentEnemyCount => spawnedEnemies.Count;

    public int EnemyCount => spawnedEnemies.Count;


    private void Start()
    {
        SpawnEnemies();
        UpdateEnemyCountUI();
    }

    private void Update()
    {
        spawnedEnemies.RemoveAll(e => e == null);
        UpdateEnemyCountUI();

        if (spawnedEnemies.Count == 0)
            Destroy(gameObject);
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            float angle = i * goldenAngle * Mathf.Deg2Rad;
            float radius = spacing * Mathf.Sqrt(i);

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            Vector3 offset = new Vector3(x, 0, z);
            Vector3 targetPos = transform.position + offset;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                GameObject enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
                enemy.GetComponent<EnemyAI>().spawnPoint = this;
                spawnedEnemies.Add(enemy);
            }
        }
    }

    private void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = spawnedEnemies.Count.ToString();
        }
    }

    public void RemoveOneEnemy()
    {
        if (spawnedEnemies.Count == 0) return;

        GameObject playerGroup = GameObject.FindGameObjectWithTag("PlayerGroup");
        if (playerGroup == null) return;

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(enemy.transform.position, playerGroup.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            if (deathEffectPrefab != null)
            {
                GameObject effect = Instantiate(deathEffectPrefab, closestEnemy.transform.position, Quaternion.identity);
                Destroy(effect, deathEffectLifetime);
            }

            spawnedEnemies.Remove(closestEnemy);
            Destroy(closestEnemy);
            UpdateEnemyCountUI();
        }
    }

    public void ForceUpdateEnemies(int newCount)
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                if (deathEffectPrefab != null)
                {
                    GameObject effect = Instantiate(deathEffectPrefab, enemy.transform.position, Quaternion.identity);
                    Destroy(effect, deathEffectLifetime);
                }

                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();
        enemyCount = newCount;
        SpawnEnemies();
        UpdateEnemyCountUI();
    }

}
