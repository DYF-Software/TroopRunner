using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class FinalEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerGroup;
    private Animator animator;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "BackrunScene")
        {
            this.enabled = false;
            return;
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject playerGroupObj = GameObject.FindGameObjectWithTag("PlayerGroup");
        if (playerGroupObj != null)
        {
            playerGroup = playerGroupObj.transform;
        }

        if (animator != null)
        {
            animator.SetBool("isRunning", true); 
        }
    }

    void Update()
    {
        if (playerGroup != null && agent != null)
        {
            agent.SetDestination(playerGroup.position);
        }
    }
}
