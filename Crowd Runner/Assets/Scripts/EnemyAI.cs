using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    public float detectionDistance = 20f;
    public EnemySpawnPoint spawnPoint;
    private Animator animator;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "ForwardrunScene")
        {
            this.enabled = false;
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("PlayerGroup")?.transform;

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
        }
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance < detectionDistance)
        {
            agent.SetDestination(target.position);

            if (animator != null)
            {
                animator.SetBool("isRunning", agent.velocity.magnitude > 0.1f);
            }
        }
        else
        {
            agent.ResetPath();

            if (animator != null)
            {
                animator.SetBool("isRunning", false);
            }
        }
    }
}
