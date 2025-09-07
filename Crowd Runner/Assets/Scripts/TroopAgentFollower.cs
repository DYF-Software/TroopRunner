using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TroopAgentFollower : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform followTarget;
    private Vector3 localOffset;
    private Transform firstGateTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        followTarget = transform.parent;
        localOffset = transform.localPosition;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = 3f;
        agent.angularSpeed = 500f;
        agent.acceleration = 10f;
        agent.autoBraking = false;
        agent.stoppingDistance = 0f;

        GameObject firstGate = GameObject.FindGameObjectWithTag("FirstGate");
        if (firstGate != null)
        {
            firstGateTarget = firstGate.transform;
        }
    }

    void Update()
    {
        if (followTarget == null || !agent.isOnNavMesh) return; 

        Vector3 targetPos = followTarget.TransformPoint(localOffset);
        agent.SetDestination(targetPos);

        if (firstGateTarget != null)
        {
            Vector3 lookDirection = firstGateTarget.position - transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }


    public void StopMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
    }


}
