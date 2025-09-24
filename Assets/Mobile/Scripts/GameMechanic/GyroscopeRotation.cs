using SocketClient;
using UnityEngine;

public class GyroscopeRotation : MonoBehaviour
{
    private Gyroscope _gyroscope;
    private bool _isGyroSupported;
    private Quaternion _lastRotation;

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
        Debug.Log(_gyroscope.enabled);
    }

    private void FixedUpdate()
    {
        if (!_isGyroSupported)
            return;
        
        Vector3 euler = _gyroscope.attitude.eulerAngles;
        (euler.y, euler.z) = (euler.z, -euler.y);
        

        GyroData data = new(Quaternion.Euler(euler));

        //Debug.Log($"{data.value} text {_lastRotation}");
        if (data.value == _lastRotation)
            return;

        WebSocketClient.Instance.Send(NetworkedObject.MakePayload(data));
        _lastRotation = data.value;
        
        //Debug.Log(data.value);
    }
}