using UnityEngine;

public class ConfigurableJointController : MonoBehaviour
{
    public ConfigurableJoint joint;           
    private float defaultPositionSpring;      

    void Start()
    {
        // Initialize default positionSpring from the joint
        if (joint != null)
        {
            defaultPositionSpring = joint.slerpDrive.positionSpring;
        }
    }

    // Call this to activate ragdoll (reduce slerp position spring to 0)
    public void ActivateRagdoll()
    {
        if (joint != null)
        {
            JointDrive slerpDrive = joint.slerpDrive;
            slerpDrive.positionSpring = 10;  // Loosen the joint
            joint.slerpDrive = slerpDrive;
        }
    }

    // Call this to deactivate ragdoll (reset slerp position spring to default)
    public void DeactivateRagdoll()
    {
        if (joint != null)
        {
            JointDrive slerpDrive = joint.slerpDrive;
            slerpDrive.positionSpring = defaultPositionSpring;  // Restore the default value
            joint.slerpDrive = slerpDrive;
        }
    }
}
