using UnityEngine;

public class PlayerMovement : ObjectGravity
{
    [Header("Movement Settings")] public float moveSpeed = 6f;

    public float jumpForce = 8f;

    [Header("Ground Check")] public float groundCheckDistance = 1.2f;

    public float mouseSensitivity = 150f;
    [SerializeField] private Transform roomParent;
    private Transform cam;
    private bool isGrounded;
    private float mouseX;
    private float mouseY;

    private Vector3 moveInput;

    private float xRotation;
    private float yRotation;

    protected override void Start()
    {
        base.Start();

        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleCamera();
        HandleMovementInput();
        HandleJumpInput();
    }

    protected override void FixedUpdate()
    {
        ApplyMovement();
        base.FixedUpdate();
    }

    private void HandleCamera()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation - mouseY, -80f, 80f);
        yRotation += mouseX;

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        Quaternion rotAdjust = Quaternion.FromToRotation(Vector3.down, GravityAxis);
        transform.rotation = rotAdjust * Quaternion.Euler(0, yRotation, 0);
    }

    private void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, -GravityAxis);
        Vector3 camRight = Vector3.ProjectOnPlane(cam.right, -GravityAxis);

        moveInput = (camRight * x + camForward * z).normalized * moveSpeed;
    }

    private void ApplyMovement()
    {
        Vector3 verticalVel = Vector3.Project(rb.linearVelocity, GravityAxis); // TODO: Check if this behaves as it should (isolating the local Y)
        Vector3 newVel = moveInput + verticalVel;
        rb.linearVelocity = newVel;
    }

    private void HandleJumpInput()
    {
        isGrounded = Physics.Raycast(transform.position, GravityAxis, groundCheckDistance);

        if (isGrounded)
        {
            transform.parent = roomParent;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 vel = rb.linearVelocity;
                rb.linearVelocity -= Vector3.Project(vel, GravityAxis);
                rb.AddForce(-GravityAxis * jumpForce, ForceMode.VelocityChange);
            }
        }
        else
            transform.parent = null;
    }

    private void OnDrawGizmos()
    {
        Vector3 rayDirection = roomParent.rotation * GravityAxis;
        if (roomParent == null) return;

        Gizmos.color = Color.red;

        // 1. Normal gravity ray
        Vector3 origin = transform.position;
        Gizmos.DrawLine(origin, origin + GravityAxis * groundCheckDistance);
        Gizmos.DrawSphere(origin + GravityAxis * groundCheckDistance, 0.05f);

        // 2. Ray using rotated gravity direction
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(origin, origin + rayDirection * groundCheckDistance);
        Gizmos.DrawSphere(origin + rayDirection * groundCheckDistance, 0.05f);
    }
}