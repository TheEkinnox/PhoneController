using UnityEngine;

public class ObjectGravity : MonoBehaviour
{
    [Header("Gravity Settings")] public Vector3 gravityAxis;

    public float gravityStrength = 20f;
    public bool gravityActive = true;
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        gravityAxis = GameManager.Instance.phoneRotation * Vector3.down;
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();
    }

    private void ApplyCustomGravity()
    {
        if (gravityActive)
            rb.AddForce(gravityAxis.normalized * gravityStrength, ForceMode.Acceleration);
    }
}