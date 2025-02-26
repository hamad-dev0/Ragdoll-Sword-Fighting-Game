using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyRagdoll : MonoBehaviour
{
    public Rigidbody[] rigidbodies;
    public ConfigurableJointController[] jointControllers;
    public Animator animator;
    public EnemyAI enemyAI;
    public NavMeshAgent navMeshAgent;
    public EnemyCombat enemyCombat;
    public Collider enemySwordCollider;  
    public Collider playerSwordCollider;

    public Transform enemy;
    
    public bool isRagdoll = false;
    private Quaternion originalRotation;
    public float resetDelay = 1f;  

    public int health = 100;  
    public int damageTaken = 25;  // Adjustable damage per hit
    public float damageCooldown = 0.5f; // Cooldown time between hits
    private float lastDamageTime = 0f; // Tracks last time damage was taken

    void Start()
    {
        originalRotation = enemy.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CanTakeDamage() && playerSwordCollider != null && collision.collider == playerSwordCollider && playerSwordCollider.enabled)
        {
            TakeDamage(damageTaken);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CanTakeDamage() && playerSwordCollider != null && other == playerSwordCollider && playerSwordCollider.enabled)
        {
            TakeDamage(damageTaken);
        }
    }

    bool CanTakeDamage()
    {
        return Time.time >= lastDamageTime + damageCooldown; // Ensure cooldown has passed
    }

    void TakeDamage(int damage)
    {
        lastDamageTime = Time.time; // Update last damage time
        health -= damage;
        Debug.Log("Enemy Health: " + health);

        if (health <= 0 && !isRagdoll)
        {
            EnableRagdoll();
        }
    }

    void EnableRagdoll()
    {
        isRagdoll = true;

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.freezeRotation = false;
        }

        foreach (ConfigurableJointController jointController in jointControllers)
        {
            jointController.ActivateRagdoll();
        }

        animator.enabled = false;
        enemyAI.enabled = false;
        navMeshAgent.enabled = false;
        enemyCombat.enabled = false;
        enemySwordCollider.enabled = false;  // Disable sword collider on ragdoll
    }
}
