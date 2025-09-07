using UnityEngine;

public class TroopForwardMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private bool isStopped = true;
    private Transform currentTarget;
    private Transform firstGateTarget;

    void Start()
    {
        GameObject firstGate = GameObject.FindGameObjectWithTag("FirstGate");
        if (firstGate != null)
        {
            firstGateTarget = firstGate.transform;
        }
    }

    void Update()
    {
        if (firstGateTarget != null)
        {
            MoveTowards(firstGateTarget);
            return;
        }

        if (isStopped || currentTarget == null) return;

        MoveTowards(currentTarget);
    }

    private void MoveTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        isStopped = (target == null);
    }
}
