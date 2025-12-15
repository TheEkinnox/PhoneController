using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectGravity : MonoBehaviour
{
    public float gravityScale = 1f;
    protected Rigidbody rb;

    protected static Vector3 GravityAxis => GameManager.Instance ? GameManager.Instance.GravityDirection : Vector3.down;
    private float GravityStrength => GameManager.Instance ? GameManager.Instance.gravityStrength * gravityScale : gravityScale;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    protected virtual void FixedUpdate()
    {
        ApplyCustomGravity();
    }

    private void ApplyCustomGravity()
    {
        if (!Mathf.Approximately(GravityStrength, 0f))
            rb.AddForce(GravityAxis * GravityStrength, ForceMode.Acceleration);
    }
}