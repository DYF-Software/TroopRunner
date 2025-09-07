using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LastCombatController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private bool isBackrunScene;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        isBackrunScene = SceneManager.GetActiveScene().name == "BackrunScene";
        if (!isBackrunScene)
        {
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBackrunScene) return;

        if (other.CompareTag("PlayerGroup"))
        {
            StopAgent(agent);
            if (animator != null)
            {
                animator.SetBool("isRunning", false);
                animator.SetTrigger("Attack");
            }

            var troopAgent = other.GetComponent<NavMeshAgent>();
            var troopAnimator = other.GetComponent<Animator>();

            StopAgent(troopAgent);
            if (troopAnimator != null)
            {
                troopAnimator.SetBool("isRunning", false);
                troopAnimator.SetTrigger("Death"); 
            }

            Destroy(other.gameObject, 0.04f);
        }
    }

    private void StopAgent(NavMeshAgent navAgent)
    {
        if (navAgent == null) return;
        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;
        navAgent.ResetPath();
    }
}
