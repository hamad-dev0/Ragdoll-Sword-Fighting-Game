using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;

    [Header("AI Settings")]
    public float followRadius = 10f;
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 2f;

    private void Start()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= followRadius)
        {
            agent.SetDestination(player.position);
            RotateTowardsPlayer();
        }
        else
        {
            agent.ResetPath(); // Stop moving when player is out of range
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep rotation horizontal
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRadius);
    }
}
