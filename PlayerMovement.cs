using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius;

    [Header("Jump")]
    public float jumpForce;

    [Header("Components")]
    public Rigidbody rigidBody;
    public Camera playerCamera;
    public PhysicMaterial playerPhysicsMaterial;

    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;
    public float rotationSpeed;

    private bool isSprinting;

    private void Start()
    {
        // Check if required components are attached
        CheckRequiredComponents();

        // Apply physics material settings for better movement behavior
        ApplyPhysicsMaterialSettings();
    }

    private void Update()
    {
        // Check if the player is grounded
        bool isGrounded = IsGrounded();

        // Handle jumping based on input and ground status
        HandleJump(isGrounded);

        // Handle sprinting based on input
        HandleSprint();

        // Capture player input for movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction based on camera orientation
        Vector3 moveDirection = CalculateMoveDirection(horizontalInput, verticalInput);

        // Move the player
        MovePlayer(moveDirection);

        // Rotate the player based on movement direction
        RotatePlayer(moveDirection);
    }

    private bool IsGrounded()
    {
        // Check if the player is grounded using a spherecast
        return Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
    }

    private void CheckRequiredComponents()
    {
        // Check if crucial components are assigned; disable the script if not
        if (!rigidBody || !playerCamera || !playerPhysicsMaterial)
        {
            Debug.LogError("Some components are missing. Please assign them in the Unity Editor.");
            enabled = false;
        }
    }

    private void ApplyPhysicsMaterialSettings()
    {
        // Set physics material settings for improved ground interaction
        playerPhysicsMaterial.dynamicFriction = 0f;
        playerPhysicsMaterial.staticFriction = 0f;
        playerPhysicsMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
    }

    private void HandleJump(bool isGrounded)
    {
        // Check for jump input and ground contact before applying the jump force
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidBody.velocity = new Vector3(
                rigidBody.velocity.x,
                Mathf.Sqrt(2f * jumpForce * Mathf.Abs(Physics.gravity.y)),
                rigidBody.velocity.z
            );
        }
    }

    private void HandleSprint()
    {
        // Toggle sprinting based on input
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    private Vector3 CalculateMoveDirection(float horizontalInput, float verticalInput)
    {
        // Calculate the movement direction based on camera orientation
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        return (cameraForward.normalized * verticalInput + cameraRight.normalized * horizontalInput).normalized;
    }

    private void MovePlayer(Vector3 moveDirection)
    {
        // Choose move speed based on whether sprinting or not
        float currentMoveSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Move the player using the calculated move direction and speed
        Vector3 moveVelocity = moveDirection * currentMoveSpeed;
        rigidBody.velocity = new Vector3(moveVelocity.x, rigidBody.velocity.y, moveVelocity.z);
    }

    private void RotatePlayer(Vector3 moveDirection)
    {
        // Rotate the player based on the movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}