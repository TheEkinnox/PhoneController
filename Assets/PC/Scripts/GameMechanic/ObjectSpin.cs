using UnityEngine;

public class ObjectSpin : MonoBehaviour
{
    [System.Serializable]
    public class AxisSettings
    {
        public bool enable = false;
        public float speed = 45f;
        public bool positiveDirection = true;
    }

    [SerializeField] private AxisSettings xAxis;
    [SerializeField] private AxisSettings yAxis;
    [SerializeField] private AxisSettings zAxis;

    [HideInInspector]
    public float powerMultiplier = 1f;

    void Update()
    {
        Vector3 rotation = Vector3.zero;

        if (xAxis.enable)
        {
            float dir = xAxis.positiveDirection ? 1f : -1f;
            rotation.x = xAxis.speed * dir * powerMultiplier * Time.deltaTime;
        }

        if (yAxis.enable)
        {
            float dir = yAxis.positiveDirection ? 1f : -1f;
            rotation.y = yAxis.speed * dir * powerMultiplier * Time.deltaTime;
        }

        if (zAxis.enable)
        {
            float dir = zAxis.positiveDirection ? 1f : -1f;
            rotation.z = zAxis.speed * dir * powerMultiplier * Time.deltaTime;
        }

        transform.Rotate(rotation, Space.Self);
    }
}