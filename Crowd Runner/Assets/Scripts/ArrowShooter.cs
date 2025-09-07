using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ArrowShooter : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab;
    public float fireInterval = 2f;
    public float arrowForce = 10f;
    public string troopTag = "Troop";  

    private float fireTimer;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "BackrunScene") return;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "BackrunScene") return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            FireFromAllTroops();
            fireTimer = 0f;
        }
    }

    void FireFromAllTroops()
    {
        GameObject[] troops = GameObject.FindGameObjectsWithTag(troopTag);

        for (int i = 0; i < troops.Length; i++)
        {
            Transform firePoint = troops[i].transform.Find("FirePoint");

            if (firePoint != null)
            {
                StartCoroutine(FireWithDelay(firePoint, i * 0.2f));
            }
        }
    }

    IEnumerator FireWithDelay(Transform firePoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (firePoint == null) yield break;
        if (firePoint.gameObject == null) yield break;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(Vector3.forward * arrowForce, ForceMode.VelocityChange); 
        }

        Destroy(arrow, 5f);
    }
}
