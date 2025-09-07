using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;

public class TroopSpawner : MonoBehaviour
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private GameObject troopPrefab;

    [Header("Formasyon Ayarlar�")]
    [SerializeField] private float spacing = 0.3f;
    [SerializeField] private float goldenAngle = 137.5f;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float spawnOffsetZ = -1f;

    [Header("UI")]
    [SerializeField] private TextMeshPro troopCountText;
    [SerializeField] private ArrowShooter arrowShooter;

    private List<GameObject> spawnedTroops = new List<GameObject>();
    private CapsuleCollider capsuleCollider;
    private bool isGateEffectActive = false;

    public int TroopCount => spawnedTroops.Count;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        SpawnTroops(gameSettings.initialTroopCount);
        UpdateGroupCollider(gameSettings.initialTroopCount);
        UpdateTroopCountUI();
    }

    private void SpawnTroops(int totalCount)
{
    bool isBackrun = SceneManager.GetActiveScene().name == "BackrunScene";

    // Mevcut askerleri temizle
    foreach (var troop in spawnedTroops)
        if (troop != null) Destroy(troop);
    spawnedTroops.Clear();

    for (int i = 0; i < totalCount; i++)
    {
        float angle = i * goldenAngle * Mathf.Deg2Rad;
        float radius = spacing * Mathf.Sqrt(i + 1); // +1 ile merkezde boşluk engellenir
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        Vector3 localOffset = new Vector3(x, 0f, z);
        Vector3 worldPosition = transform.position + (isBackrun ? localOffset : new Vector3(x, 0f, spawnOffsetZ));

        // Backrun sahnesinde askerler direkt geriye dönsün
        Quaternion rotation = isBackrun ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity;

        GameObject troop = Instantiate(troopPrefab, worldPosition, rotation, transform);

        // Animasyon
        Animator animator = troop.GetComponent<Animator>();
        if (animator != null)
            animator.SetBool("isRunning", true);

        // NavMesh Ayarları
        NavMeshAgent agent = troop.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (isBackrun)
                agent.enabled = false;
            else
            {
                agent.speed = gameSettings.forwardSpeed;
                agent.angularSpeed = 500f;
                agent.acceleration = 10f;
                agent.autoBraking = false;
                agent.updateRotation = false;
                agent.updateUpAxis = false;
            }
        }

        spawnedTroops.Add(troop);

        if (!isBackrun)
            StartCoroutine(MoveTroopToPosition(troop.transform, transform.position + localOffset, slideDuration));
        else
            troop.transform.localPosition = localOffset;
    }

    UpdateTroopCountUI();
}

    IEnumerator MoveTroopToPosition(Transform troop, Vector3 targetPos, float duration)
    {
        Vector3 start = troop.position;
        float time = 0f;

        while (time < duration)
        {
            if (troop == null) yield break;
            troop.position = Vector3.Lerp(start, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        if (troop != null)
            troop.position = targetPos;
    }

    private void UpdateGroupCollider(int troopCount)
    {
        float dynamicRadius = Mathf.Sqrt(troopCount) * 0.25f;
        capsuleCollider.radius = dynamicRadius;
        capsuleCollider.height = 2f;
        capsuleCollider.center = new Vector3(0f, 1f, 0f);
        capsuleCollider.isTrigger = false;
    }

    private void UpdateTroopCountUI()
    {
        if (troopCountText != null)
        {
            troopCountText.text = spawnedTroops.Count.ToString();
        }
    }

    public void ApplyGateEffect(GateOperation operation, int value)
    {
        if (isGateEffectActive) return;
        StartCoroutine(ApplyGateEffectWithCooldown(operation, value));
    }

    private IEnumerator ApplyGateEffectWithCooldown(GateOperation operation, int value)
    {
        isGateEffectActive = true;

        int currentCount = spawnedTroops.Count;
        int newCount = currentCount;

        switch (operation)
        {
            case GateOperation.Add:
                newCount += value;
                break;
            case GateOperation.Subtract:
                newCount = Mathf.Max(1, currentCount - value);
                break;
            case GateOperation.Multiply:
                newCount = currentCount * value;
                break;
            case GateOperation.Divide:
                if (value <= 0) value = 1;
                newCount = Mathf.Max(1, Mathf.RoundToInt((float)currentCount / value));
                break;
        }

        SpawnTroops(newCount);
        UpdateGroupCollider(newCount);
        UpdateTroopCountUI();

        yield return new WaitForSeconds(2f);
        isGateEffectActive = false;
    }

    public void RemoveOneTroop()
    {
        if (spawnedTroops.Count == 0) return;

        GameObject troop = spawnedTroops[spawnedTroops.Count - 1];
        if (troop != null)
        {
            Destroy(troop);
            spawnedTroops.RemoveAt(spawnedTroops.Count - 1);
            UpdateTroopCountUI();
        }
    }

    public void AddTroopsWithoutReset(int additionalCount)
    {
        int currentCount = spawnedTroops.Count;
        int newTotal = currentCount + additionalCount;
        SpawnTroops(newTotal);
        UpdateGroupCollider(newTotal);
        UpdateTroopCountUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SceneTrigger"))
        {
            SceneManager.LoadScene("BackrunScene");
        }

        if (other.CompareTag("StopCollider"))
        {
            StopMovement();

            foreach (var troop in spawnedTroops)
            {
                if (troop == null) continue;

                var follower = troop.GetComponent<TroopAgentFollower>();
                if (follower != null) follower.StopMovement();

                var anim = troop.GetComponent<Animator>();
                if (anim != null) anim.SetBool("isRunning", false);

                var agent = troop.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.ResetPath();
                }
            }

            var myAgent = GetComponent<NavMeshAgent>();
            if (myAgent != null)
            {
                myAgent.isStopped = true;
                myAgent.velocity = Vector3.zero;
                myAgent.ResetPath();
            }
        }
    }

    private void StopMovement()
    {
        var myAgent = GetComponent<NavMeshAgent>();
        if (myAgent != null)
            myAgent.isStopped = true;
    }
}
