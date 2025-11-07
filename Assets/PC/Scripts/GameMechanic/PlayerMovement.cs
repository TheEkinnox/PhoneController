using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
[Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 8f;

    [Header("Gravity Settings")]
    public Vector3 gravityDirection = Vector3.down;
    public float gravityStrength = 20f;

    [Header("Ground Check")]
    public float groundCheckDistance = 1.2f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private bool isGrounded;

    private Transform cam;
    private float mouseX;
    private float mouseY;
    private float xRotation = 0f;
    public float mouseSensitivity = 150f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleCamera();
        HandleMovementInput();
        HandleJumpInput();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplyCustomGravity();
    }

    // -------------------------------
    // CAMERA
    // -------------------------------
    void HandleCamera()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // -------------------------------
    // MOVEMENT
    // -------------------------------
    private Vector3 moveInput;

    void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Get the movement direction relative to custom gravity
        Vector3 localRight = Vector3.Cross(-gravityDirection, transform.forward).normalized;
        Vector3 localForward = Vector3.Cross(localRight, -gravityDirection).normalized;

        moveInput = (localRight * x + localForward * z).normalized * moveSpeed;
    }

    void ApplyMovement()
    {
        Vector3 velocity = rb.linearVelocity;

        // Separate gravity velocity and lateral velocity
        Vector3 gravityAxis = gravityDirection.normalized;
        Vector3 verticalVel = Vector3.Project(velocity, gravityAxis);
        Vector3 lateralVel = Vector3.ProjectOnPlane(velocity, gravityAxis);

        // Directly set the lateral velocity for responsiveness
        Vector3 newVel = moveInput + verticalVel;
        rb.linearVelocity = newVel;
    }

    // -------------------------------
    // JUMP
    // -------------------------------
    void HandleJumpInput()
    {
        isGrounded = Physics.Raycast(transform.position, gravityDirection, groundCheckDistance, groundMask);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // Remove downward velocity and apply upward impulse
            Vector3 gravityAxis = gravityDirection.normalized;
            Vector3 vel = rb.linearVelocity;

            // cancel any downward motion before jumping
            vel -= Vector3.Project(vel, gravityAxis);
            rb.linearVelocity = vel;

            rb.AddForce(-gravityAxis * jumpForce, ForceMode.VelocityChange);
        }
    }

    // -------------------------------
    // CUSTOM GRAVITY
    // -------------------------------
    void ApplyCustomGravity()
    {
        rb.AddForce(gravityDirection.normalized * gravityStrength, ForceMode.Acceleration);
    }
}

