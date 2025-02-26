using UnityEngine;
using System.Collections;

public class SwordAttackController : MonoBehaviour
{
    public ConfigurableJoint leftArmJoint; // Assign in Inspector
    public Transform handTransform; // Assign the hand object in Inspector
    public Collider swordCollider;
    public TrailRenderer trailRenderer;

    // Attack rotation (plays when releasing or quick click)
    public float attackX = -30f;
    public float attackY = 10f;
    public float attackZ = 0f;
    public float handRotX;

    // Hold rotation (stays while holding)
    public float holdX = -60f;
    public float holdY = 0f;
    public float holdZ = 0f;

    public float attackTime = 0.2f; // Duration of attack motion
    public float holdThreshold = 0.3f; // How long to hold before switching to hold position
    public float handResetDelay = 0.3f; // Delay before hand resets
    public float handResetSpeed = 2f; // Speed of smooth reset
    public float defaultSpring = 10f;
    public float holdSpring = 100f;

    private Quaternion defaultRotation;
    private bool isHolding = false;
    private bool isAttacking = false;
    private float holdTimer = 0f;

    void Start()
    {
        defaultRotation = leftArmJoint.targetRotation;
        JointDrive slerpDrive = leftArmJoint.slerpDrive;
        slerpDrive.positionSpring = defaultSpring;
        leftArmJoint.slerpDrive = slerpDrive;
        swordCollider.enabled = false;
        trailRenderer.emitting = false;
    }

    void Update()
    {
        JointDrive slerpDrive = leftArmJoint.slerpDrive;

        if (Input.GetMouseButtonDown(0)) // Start timer when mouse is pressed
        {
            holdTimer = 0f;
        }

        if (Input.GetMouseButton(0)) // Holding Mouse Button
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdThreshold && !isHolding) // If held long enough, go to hold position
            {
                isHolding = true;
                MoveHandToAttackPosition();
                Quaternion holdRotation = Quaternion.Euler(holdX, holdY, holdZ);
                leftArmJoint.targetRotation = Quaternion.Inverse(defaultRotation) * holdRotation;
                slerpDrive.positionSpring = holdSpring;
                leftArmJoint.slerpDrive = slerpDrive;
                swordCollider.enabled = true;

            }
        }

        if (Input.GetMouseButtonUp(0)) // Released Mouse Button
        {
            if (isHolding) // If it was holding, play attack first before resetting
            {
                isHolding = false;
                StartCoroutine(PerformAttack());
            }
            else // If it was just a quick click, do a quick attack transition
            {
                StartCoroutine(PerformQuickAttack());
            }
        }
    }

    IEnumerator PerformAttack()
    {
        if (isAttacking) yield break; // Prevent multiple attack calls
        isAttacking = true;
        swordCollider.enabled = true;
        trailRenderer.enabled = true;

        MoveHandToAttackPosition(); // Move hand rotation
        JointDrive slerpDrive = leftArmJoint.slerpDrive;
        Quaternion attackRotation = Quaternion.Euler(attackX, attackY, attackZ);
        leftArmJoint.targetRotation = Quaternion.Inverse(defaultRotation) * attackRotation;
        slerpDrive.positionSpring = holdSpring;
        leftArmJoint.slerpDrive = slerpDrive;

        yield return new WaitForSeconds(attackTime);

        ResetArm();
        swordCollider.enabled = false;
        trailRenderer.emitting = false;
        yield return new WaitForSeconds(handResetDelay); // Delay before resetting hand
        StartCoroutine(SmoothResetHandRotation());

        isAttacking = false;
    }

    IEnumerator PerformQuickAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        swordCollider.enabled = true;
        trailRenderer.emitting = true;

        MoveHandToAttackPosition(); // Move hand rotation
        JointDrive slerpDrive = leftArmJoint.slerpDrive;
        Quaternion attackRotation = Quaternion.Euler(attackX, attackY, attackZ);
        leftArmJoint.targetRotation = Quaternion.Inverse(defaultRotation) * attackRotation;
        slerpDrive.positionSpring = holdSpring;
        leftArmJoint.slerpDrive = slerpDrive;

        yield return new WaitForSeconds(attackTime * 0.5f); // Shorter attack time for quick click

        ResetArm();
        swordCollider.enabled = false;
        trailRenderer.emitting = false;
        yield return new WaitForSeconds(handResetDelay); // Delay before resetting hand
        StartCoroutine(SmoothResetHandRotation());

        isAttacking = false;
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

            handTransform.localRotation = targetRotation; // Ensure final position is exact
        }
    }
}
