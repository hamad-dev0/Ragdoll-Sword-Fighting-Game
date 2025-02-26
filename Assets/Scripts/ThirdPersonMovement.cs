using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    //public Ragdoll ragdoll;
    public Camera playerCamera; // Added camera reference

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed;

    [Header("Ground Check")]
    public bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.2f;

    [Header("State")]
    public bool isMoving; // Added to check if player is moving

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb.freezeRotation = true;
    }

    void Update()
    {
        //if (ragdoll.isRagdoll)
            //return;

        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance + 0.1f, groundLayer);

        // Set orientation to match camera direction (only Y-axis)
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0; // Ignore vertical tilt
        orientation.forward = cameraForward.normalized;

        // Movement Input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Check if player is moving
        isMoving = inputDir.magnitude > 0;

        // Rotate Player Object
        if (isMoving)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space)) // isGrounded
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        //if (ragdoll.isRagdoll)
            //return;
        MovePlayer();
    }

    void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;

        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }
}
