using UnityEngine;
using System.Collections;

public class EnemyCombat : MonoBehaviour
{
    // Attack variables
    public ConfigurableJoint leftArmJoint;
    public Transform handTransform;
    public TrailRenderer swordTrail;
    public Collider swordCollider;
    public float attackX = -30f, attackY = 10f, attackZ = 0f, handRotX;
    public float attackTime = 0.2f, handResetDelay = 0.3f, handResetSpeed = 2f;
    public float defaultSpring = 10f, holdSpring = 100f;
    public float attackDelay = 1f;
    public float attackRadius = 5f;
    public Transform player;
    private bool isAttacking = false;
    private int attackCount = 0;
    public int maxAttacksBeforeShield = 3;
    public bool canShield = true; // Added bool to control shield usage

    // Shield variables
    public ConfigurableJoint shieldJoint;
    public EnemyRagdoll enemyRagdoll;
    public Transform elbowTransform;
    public float shieldX = 30f, shieldY = 10f, shieldZ = 0f;
    public float elbowHoldX = 60f, returnSpeed = 2f;
    public float shieldDuration = 2f;
    private bool isShielding = false;
    private Quaternion defaultRotation;

    void Start()
    {
        defaultRotation = leftArmJoint.targetRotation;
        JointDrive slerpDrive = leftArmJoint.slerpDrive;
        slerpDrive.positionSpring = defaultSpring;
        leftArmJoint.slerpDrive = slerpDrive;
        swordTrail.emitting = false;
        swordCollider.enabled = false;
    }

    void Update()
    {
        if (!isAttacking && !isShielding && Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            StartCoroutine(EnemyCombatCycle());
        }
    }

    IEnumerator EnemyCombatCycle()
    {
        if (attackCount < maxAttacksBeforeShield)
        {
            yield return StartCoroutine(EnemyAttack());
            attackCount++;
        }
        else
        {
            if (canShield)
            {
                yield return StartCoroutine(ShieldUp());
            }
            attackCount = 0;
        }
    }

    IEnumerator EnemyAttack()
    {
        isAttacking = true;
        MoveHandToAttackPosition();
        swordTrail.emitting = true;
        swordCollider.enabled = true;
        JointDrive slerpDrive = leftArmJoint.slerpDrive;
        leftArmJoint.targetRotation = Quaternion.Inverse(defaultRotation) * Quaternion.Euler(attackX, attackY, attackZ);
        slerpDrive.positionSpring = holdSpring;
        leftArmJoint.slerpDrive = slerpDrive;

        yield return new WaitForSeconds(attackTime);
        ResetArm();
        swordTrail.emitting = false;
        swordCollider.enabled = false;
        yield return new WaitForSeconds(handResetDelay);
        StartCoroutine(SmoothResetHandRotation());
        yield return new WaitForSeconds(attackDelay);

        isAttacking = false;
    }

    IEnumerator ShieldUp()
    {
        isShielding = true;
        enemyRagdoll.enabled = false;
        JointDrive slerpDrive = shieldJoint.slerpDrive;
        shieldJoint.targetRotation = Quaternion.Inverse(defaultRotation) * Quaternion.Euler(shieldX, shieldY, shieldZ);
        slerpDrive.positionSpring = holdSpring;
        shieldJoint.slerpDrive = slerpDrive;

        if (elbowTransform != null)
        {
            elbowTransform.localRotation = Quaternion.Euler(elbowHoldX, elbowTransform.localEulerAngles.y, elbowTransform.localEulerAngles.z);
        }

        yield return new WaitForSeconds(shieldDuration);

        StartCoroutine(SmoothResetShield());
        StartCoroutine(SmoothResetElbow());
        yield return new WaitForSeconds(attackDelay);

        isShielding = false;
    }

    void MoveHandToAttackPosition()
    {
        if (handTransform != null)
        {
            handTransform.localRotation = Quaternion.Euler(handRotX, handTransform.localRotation.y, handTransform.localRotation.z);
        }
    }

    void ResetArm()
    {
        JointDrive slerpDrive = leftArmJoint.slerpDrive;
        leftArmJoint.targetRotation = defaultRotation;
        slerpDrive.positionSpring = defaultSpring;
        leftArmJoint.slerpDrive = slerpDrive;
    }

    IEnumerator SmoothResetHandRotation()
    {
        if (handTransform != null)
        {
            Quaternion startRotation = handTransform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(0f, handTransform.localRotation.eulerAngles.y, handTransform.localRotation.eulerAngles.z);
            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                handTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
                elapsedTime += Time.deltaTime * handResetSpeed;
                yield return null;
            }
            handTransform.localRotation = targetRotation;
        }
    }

    IEnumerator SmoothResetShield()
    {
        Quaternion startRotation = shieldJoint.targetRotation;
        Quaternion targetRotation = defaultRotation;
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            shieldJoint.targetRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * returnSpeed;
            yield return null;
        }
        shieldJoint.targetRotation = targetRotation;
        JointDrive slerpDrive = shieldJoint.slerpDrive;
        slerpDrive.positionSpring = defaultSpring;
        shieldJoint.slerpDrive = slerpDrive;
        enemyRagdoll.enabled = true;
    }

    IEnumerator SmoothResetElbow()
    {
        if (elbowTransform != null)
        {
            float elapsedTime = 0f;
            float startX = elbowTransform.localEulerAngles.x;
            float targetX = 0f;
            while (elapsedTime < 1f)
            {
                float newX = Mathf.Lerp(startX, targetX, elapsedTime);
                elbowTransform.localRotation = Quaternion.Euler(newX, elbowTransform.localEulerAngles.y, elbowTransform.localEulerAngles.z);
                elapsedTime += Time.deltaTime * returnSpeed;
                yield return null;
            }
            elbowTransform.localRotation = Quaternion.Euler(targetX, elbowTransform.localEulerAngles.y, elbowTransform.localEulerAngles.z);
        }
    }
}
