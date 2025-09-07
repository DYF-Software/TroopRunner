using UnityEngine;

public class GateAreaController : MonoBehaviour
{
    [Header("Spawn Point Transforms")]
    [SerializeField] private Transform obstacleSpawnPoint;
    [SerializeField] private Transform leftGateSpawnPoint;
    [SerializeField] private Transform rightGateSpawnPoint;

    [Header("Prefabs")]
    public GameObject obstaclePrefab;
    public GameObject leftGatePrefab;
    public GameObject rightGatePrefab;

    [Header("Gate Operations")]
    public GateOperation leftGateOperation;
    public int leftGateValue;

    public GateOperation rightGateOperation;
    public int rightGateValue;

    void Start()
    {
        if (obstaclePrefab != null)
            Instantiate(obstaclePrefab, obstacleSpawnPoint.position, Quaternion.identity, transform);

        if (leftGatePrefab != null)
        {
            GameObject leftGate = Instantiate(leftGatePrefab, leftGateSpawnPoint.position, Quaternion.identity, transform);
            var gate = leftGate.GetComponent<GateBehavior>();
            if (gate != null)
                gate.Initialize(leftGateOperation, leftGateValue);
        }

        if (rightGatePrefab != null)
        {
            GameObject rightGate = Instantiate(rightGatePrefab, rightGateSpawnPoint.position, Quaternion.identity, transform);
            var gate = rightGate.GetComponent<GateBehavior>();
            if (gate != null)
                gate.Initialize(rightGateOperation, rightGateValue);
        }
    }
}
