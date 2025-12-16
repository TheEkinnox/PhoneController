using SocketClient;
using UnityEngine;

public class GyroscopeRotation : MonoBehaviour
{
    private Gyroscope _gyroscope;
    private bool _isGyroSupported;
    private Quaternion _lastRotation;
    private Quaternion _gyroInitialRotation = Quaternion.identity;
    private bool _canSend;

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
        _canSend = false;
        Debug.Log($"Gyroscope {(_gyroscope.enabled ? "enabled" : "disabled")}");
    }

    private static bool IsValidRotation(Quaternion r)
    {
        return !Mathf.Approximately(r.x, 0)
               || !Mathf.Approximately(r.y, 0)
               || !Mathf.Approximately(r.z, 0)
               || !Mathf.Approximately(r.w, 0);
    }

    public void Recalibrate()
    {
        if (!_isGyroSupported)
            return;

        _gyroInitialRotation = _gyroscope.attitude;
        print("Successfully recalibrated !");
    }

    private Quaternion AdjustGyro(Quaternion q)
    {
        q = Quaternion.Inverse(_gyroInitialRotation) * q;
        (q.y, q.z, q.w) = (0f, q.y, -q.w);
        return q;
    }

    private void FixedUpdate()
    {
        if (!_isGyroSupported)
            return;

        if (!_canSend)
        {
            _canSend = true;
            return; // Skip first frame as gyro is not properly initialized yet
        }

        GyroData data = new(AdjustGyro(_gyroscope.attitude));

        //Debug.Log($"{data.value} text {_lastRotation}");
        if (data.value == _lastRotation)
            return;

        WebSocketClient.Instance.Send(NetworkedObject.MakePayload(data));
        _lastRotation = data.value;

        //Debug.Log(data.value);
    }
}