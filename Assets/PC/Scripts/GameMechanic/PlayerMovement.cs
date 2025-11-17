using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")] public float moveSpeed = 6f;

    public float jumpForce = 8f;

    [Header("Gravity Settings")] public Vector3 gravityDirection = Vector3.down;

    public float gravityStrength = 20f;

    [Header("Ground Check")] public float groundCheckDistance = 1.2f;

    public float mouseSensitivity = 150f;
    [SerializeField] private Transform roomParent;
    private Transform cam;
    private bool isGrounded;
    private float mouseX;
    private float mouseY;

    private Vector3 moveInput;

    private Rigidbody rb;
    private float xRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleCamera();
        HandleMovementInput();
        HandleJumpInput();
        FreezeRotation();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyCustomGravity();
    }

    private void FreezeRotation()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, rot.y, 0);
    }

    private void HandleCamera()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 gravityAxis = gravityDirection.normalized;

        Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, -gravityAxis).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.right, -gravityAxis).normalized;

        moveInput = (camRight * x + camForward * z).normalized * moveSpeed;
    }

    private void ApplyMovement()
    {
        Vector3 velocity = rb.linearVelocity;

        Vector3 gravityAxis = gravityDirection.normalized;
        Vector3 verticalVel = Vector3.Project(velocity, gravityAxis);

        Vector3 newVel = moveInput + verticalVel;
        rb.linearVelocity = newVel;
    }

    private void HandleJumpInput()
    {
        Vector3 rayDirection = roomParent.rotation * gravityDirection.normalized;
        isGrounded = Physics.Raycast(transform.position, gravityDirection, groundCheckDistance);

        if (isGrounded)
        {
            transform.parent = roomParent;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 gravityAxis = gravityDirection.normalized;
                Vector3 vel = rb.linearVelocity;

                vel -= Vector3.Project(vel, gravityAxis);
                rb.linearVelocity = vel;

                rb.AddForce(-gravityAxis * jumpForce, ForceMode.VelocityChange);
            }
        }
        else
            transform.parent = null;
        
    }
    
    private void OnDrawGizmos()
    {
        
        Vector3 rayDirection = roomParent.rotation * gravityDirection.normalized;
        if (roomParent == null) return;

        Gizmos.color = Color.red;

        // 1. Normal gravity ray
        Vector3 origin = transform.position;
        Vector3 gravityDir = gravityDirection.normalized;
        Gizmos.DrawLine(origin, origin + gravityDir * groundCheckDistance);
        Gizmos.DrawSphere(origin + gravityDir * groundCheckDistance, 0.05f);

        // 2. Ray using rotated gravity direction
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(origin, origin + rayDirection * groundCheckDistance);
        Gizmos.DrawSphere(origin + rayDirection * groundCheckDistance, 0.05f);
    }

    private void ApplyCustomGravity() => rb.AddForce(gravityDirection.normalized * gravityStrength, ForceMode.Acceleration);
    
}