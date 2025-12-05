using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Book : MonoBehaviour
{

    private Vector3 rayDirection;
    [Header("Gravity Settings")] public Vector3 gravityDirection = Vector3.down;

    public float gravityStrength = 20f;

    [Header("Ground Check")] public float groundCheckDistance = 1.2f;

    [SerializeField] private Transform roomParent;
    private bool isGrounded;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        HandleJumpInput();
        rayDirection = roomParent.rotation * gravityDirection.normalized;
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();
    }

    private void HandleJumpInput()
    {
        isGrounded = Physics.Raycast(transform.position, rayDirection, groundCheckDistance);

        if (isGrounded)
        {
            transform.parent = roomParent;
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