using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAutoRemover : MonoBehaviour
{
    [Header("Destroy Settings")]
    public float startDelay = 2f;
    public float minInterval = 0.7f;
    public float maxInterval = 1.3f;
    public int enemiesPerInterval = 3;

    [Header("Target Collider")]
    public Collider targetArea;

    [Header("Effect Settings")]
    public GameObject deathEffectPrefab;
    public float effectDuration = 1f;

    void Start()
    {
        StartCoroutine(RemovalLoop());
    }

    IEnumerator RemovalLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            List<GameObject> aliveEnemies = new List<GameObject>();

            foreach (GameObject enemy in allEnemies)
            {
                if (enemy != null && enemy.activeInHierarchy)
                {
                    aliveEnemies.Add(enemy);
                }
            }

            aliveEnemies.Sort((a, b) =>
            {
                float distA = Vector3.Distance(a.transform.position, targetArea.ClosestPoint(a.transform.position));
                float distB = Vector3.Distance(b.transform.position, targetArea.ClosestPoint(b.transform.position));
                return distA.CompareTo(distB);
            });

            int killCount = Mathf.Min(enemiesPerInterval, aliveEnemies.Count);
            for (int i = 0; i < killCount; i++)
            {
                GameObject enemy = aliveEnemies[i];

                if (deathEffectPrefab != null && enemy != null)
                {
                    Instantiate(deathEffectPrefab, enemy.transform.position, Quaternion.identity);
                }

                if (enemy != null)
                    Destroy(enemy, effectDuration);
            }

            float nextInterval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(nextInterval);
        }
    }
}
