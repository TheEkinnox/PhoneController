using SocketClient;
using UnityEngine;

public class GyroscopeRotation : MonoBehaviour
{
    private Gyroscope _gyroscope;
    private bool _isGyroSupported;
    private Quaternion _lastRotation;
    private Quaternion _gyroInitialRotation;

#if UNITY_EDITOR
    private void OnApplicationFocus(bool hasFocus)
#else
    private void Start()
#endif
    {
        _isGyroSupported = SystemInfo.supportsGyroscope;

        if (!_isGyroSupported)
            return;

        _gyroscope = Input.gyro;
        _gyroscope.enabled = true;
        Debug.Log($"Gyroscope {(_gyroscope.enabled ? "enabled" : "disabled")}");
        Recalibrate();
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    private void Recalibrate()
    {
        if (!_isGyroSupported)
            return;

        _gyroInitialRotation = GyroToUnity(_gyroscope.attitude);
        print("Successfully recalibrated !");
    }

    private Quaternion AdjustGyro(Quaternion q)
    {
        q = Quaternion.Inverse(_gyroInitialRotation) * GyroToUnity(q);
        (q.y, q.z) = (0f, q.y);
        return q;
    }

    private void FixedUpdate()
    {
        if (!_isGyroSupported)
            return;

        GyroData data = new(AdjustGyro(_gyroscope.attitude));

        //Debug.Log($"{data.value} text {_lastRotation}");
        if (data.value == _lastRotation)
            return;

        WebSocketClient.Instance.Send(NetworkedObject.MakePayload(data));
        _lastRotation = data.value;

        //Debug.Log(data.value);
    }
}