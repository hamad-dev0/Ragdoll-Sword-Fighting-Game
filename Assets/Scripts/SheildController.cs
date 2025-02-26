using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour
{
    public ConfigurableJoint shieldJoint; // Assign in Inspector
    public Transform elbowTransform; // Assign elbow Transform in Inspector

    public float shieldX = 30f;  // Shield up position (X-axis)
    public float shieldY = 10f;  // Shield up position (Y-axis)
    public float shieldZ = 0f;   // Shield up position (Z-axis)

    public float elbowHoldX = 60f; // Elbow X rotation when holding shield
    public float returnSpeed = 2f; // Speed of smooth reset

    public float defaultSpring = 10f;
    public float holdSpring = 100f;

    private Quaternion defaultRotation;
    private bool isHoldingShield = false;
    public Ragdoll ragdoll;


    void Start()
    {
        defaultRotation = shieldJoint.targetRotation;
        JointDrive slerpDrive = shieldJoint.slerpDrive;
        slerpDrive.positionSpring = defaultSpring;
        shieldJoint.slerpDrive = slerpDrive;
    

        
    }

    void Update()
    {
        JointDrive slerpDrive = shieldJoint.slerpDrive;

        if (Input.GetMouseButton(1)) // Holding Right Click
        {
            isHoldingShield = true;
            ragdoll.enabled = false;
            Quaternion shieldRotation = Quaternion.Euler(shieldX, shieldY, shieldZ);
            shieldJoint.targetRotation = Quaternion.Inverse(defaultRotation) * shieldRotation;
            slerpDrive.positionSpring = holdSpring;
            shieldJoint.slerpDrive = slerpDrive;

            // ✅ Move the elbow to 60° on X-axis
            if (elbowTransform != null)
            {
                elbowTransform.localRotation = Quaternion.Euler(elbowHoldX, elbowTransform.localEulerAngles.y, elbowTransform.localEulerAngles.z);
            }
        }
        else if (isHoldingShield) // Released Right Click
        {
            isHoldingShield = false;
            StartCoroutine(SmoothResetShield());
            StartCoroutine(SmoothResetElbow());
            ragdoll.enabled = true;
        }

        // If Left Click while holding Right Click, reset immediately
        if (Input.GetMouseButtonDown(0) && isHoldingShield)
        {
            isHoldingShield = false;
            StopAllCoroutines(); // Cancel smooth reset if it's running
            ResetShieldInstant();
            ResetElbowInstant();
            ragdoll.enabled = true;
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

        shieldJoint.targetRotation = targetRotation; // Ensure final position is exact

        // ✅ Reset Spring to Default after rotation completes
        JointDrive slerpDrive = shieldJoint.slerpDrive;
        slerpDrive.positionSpring = defaultSpring;
        shieldJoint.slerpDrive = slerpDrive;
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

            elbowTransform.localRotation = Quaternion.Euler(targetX, elbowTransform.localEulerAngles.y, elbowTransform.localEulerAngles.z); // Ensure final position is exact
        }
    }

    void ResetShieldInstant()
    {
        shieldJoint.targetRotation = defaultRotation;

        // ✅ Reset Spring immediately
        JointDrive slerpDrive = shieldJoint.slerpDrive;
        slerpDrive.positionSpring = defaultSpring;
        shieldJoint.slerpDrive = slerpDrive;
    }

    void ResetElbowInstant()
    {
        if (elbowTransform != null)
        {
            elbowTransform.localRotation = Quaternion.Euler(0f, elbowTransform.localEulerAngles.y, elbowTransform.localEulerAngles.z);
        }
    }
}
