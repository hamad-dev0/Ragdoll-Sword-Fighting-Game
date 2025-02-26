using UnityEngine;
using System.Collections;

public class Ragdoll : MonoBehaviour
{
    public Rigidbody[] rigidbodies;  // List of rigidbodies that are part of the ragdoll
    public Transform player;  // The player's transform (for resetting rotation)
    public ConfigurableJointController[] jointControllers;  // Array of ConfigurableJointController scripts for each limb
    public SwordAttackController swordAttackController;
    public ShieldController shieldController;
    public Animator animator;
    public ThirdPersonMovement thirdPersonMovement;
    public EnemyCombat[] enemyCombats;  // Array of EnemyCombat for multiple enemies
    
    public bool isRagdoll = false;  // Whether the ragdoll is enabled or not
    private Quaternion originalRotation;  // Store original rotation
    public float resetDelay = 1f;   
    public Collider[] hitColliders;  // Array of hit colliders (for different enemies)
    public Collider userCollider;    // Collider for this character

    private float collisionDelayTimer = 0f;  // Timer to handle the collision delay
    private float collisionDelay = 2f;  // Delay before another collision can be counted

    public int health = 100;  // Health of the enemy, 100% to start with

    void Start()
    {
        originalRotation = player.rotation;  // Store initial rotation
        
        // Automatically find all EnemyCombat scripts in the scene
        enemyCombats = FindObjectsOfType<EnemyCombat>();
        
        // Automatically find all EnemySword colliders in the scene
        GameObject[] enemySwords = GameObject.FindGameObjectsWithTag("EnemySword");
        hitColliders = new Collider[enemySwords.Length];
        for (int i = 0; i < enemySwords.Length; i++)
        {
            hitColliders[i] = enemySwords[i].GetComponent<CapsuleCollider>();
        }
    }

    void Update()
    {
        // Handle collision delay
        if (collisionDelayTimer > 0)
        {
            collisionDelayTimer -= Time.deltaTime;  // Countdown the delay
        }

        // Check collision with each hitCollider
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider != null && userCollider != null)
            {
                if (hitCollider.bounds.Intersects(userCollider.bounds))  // Check for collision
                {
                    if (collisionDelayTimer <= 0)  // If delay is over, process the collision
                    {
                        collisionDelayTimer = collisionDelay;  // Reset the collision delay timer

                        // Decrease health by 25% on each collision
                        health -= 25;
                        Debug.Log("Health: " + health);

                        // Check if health has dropped to 0 or below and enable ragdoll
                        if (health <= 0 && !isRagdoll)
                        {
                            EnableRagdoll();
                        }
                    }
                }
            }
        }
    }

    void EnableRagdoll()
    {
        isRagdoll = true;

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.freezeRotation = false;
        }

        // Activate ragdoll on joints (limbs)
        foreach (ConfigurableJointController jointController in jointControllers)
        {
            jointController.ActivateRagdoll();
        }

        thirdPersonMovement.enabled = false;
        swordAttackController.enabled = false;
        shieldController.enabled = false;
        animator.enabled = false;
        
        // Disable all enemy combat scripts
        foreach (EnemyCombat enemyCombat in enemyCombats)
        {
            enemyCombat.enabled = false;
        }

        userCollider.enabled = false;
    }
}