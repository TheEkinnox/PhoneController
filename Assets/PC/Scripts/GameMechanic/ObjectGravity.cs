using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectGravity : MonoBehaviour
{
    private Vector3 rayDirection;
    public bool gravityActive;

    private bool isGrounded;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate() => ApplyCustomGravity();

    private void ApplyCustomGravity()
    {
        if (gravityActive)
            rb.AddForce(GameManager.Instance.gravityDirection.normalized * GameManager.Instance.gravityStrength,
                ForceMode.Acceleration);
        
        
    }
}