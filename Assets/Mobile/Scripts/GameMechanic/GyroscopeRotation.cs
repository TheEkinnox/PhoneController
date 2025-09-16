using SocketClient;
using UnityEngine;

public class GyroscopeRotation : MonoBehaviour
{
    private Gyroscope _gyroscope;
    private bool _isGyroSupported;
    private Quaternion _lastRotation;

    private void Start()
    {
        _isGyroSupported = SystemInfo.supportsGyroscope;

        if (!_isGyroSupported)
            return;

        _gyroscope = Input.gyro;
        _gyroscope.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!_isGyroSupported)
            return;

        GyroData data = new(_gyroscope.attitude); // send special data

        if (data.value == _lastRotation)
            return;

        WebSocketClient.Instance.Send(NetworkedObject.MakePayload(data)); //NEEDED: send data
        _lastRotation = data.value;
    }
}