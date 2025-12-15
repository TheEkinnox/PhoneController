using SocketClient;
using UnityEngine;

public class GyroscopeRotation : MonoBehaviour
{
    private Quaternion _gyroInitialRotation;
    private Gyroscope _gyroscope;
    private bool _isGyroSupported;
    private Quaternion _lastRotation;
    private Quaternion _offsetRotation;

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
        Recalibrate();
    }
    
    private void FixedUpdate()
    {
        if (!_isGyroSupported)
            return;
        
        _offsetRotation = Quaternion.Inverse(_gyroInitialRotation) * GyroToUnity(Input.gyro.attitude);
        _offsetRotation = new Quaternion(_offsetRotation.x, 0f, _offsetRotation.y, _offsetRotation.w);

        GyroData data = new(_offsetRotation);
        
        //Debug.Log($"{data.value} text {_lastRotation}");
        if (data.value == _lastRotation)
            return;

        WebSocketClient.Instance.Send(NetworkedObject.MakePayload(data));
        _lastRotation = data.value;

        //Debug.Log(data.value);
    }


    private Quaternion GyroToUnity(Quaternion gyro)
    {
        return new Quaternion(gyro.x, gyro.y, -gyro.z, -gyro.w);
    }

    public void Recalibrate()
    {
        Quaternion gyro = GyroToUnity(Input.gyro.attitude);
        _gyroInitialRotation.x = gyro.x;
        _gyroInitialRotation.y = gyro.y; // Fixed Y axis
        _gyroInitialRotation.z = gyro.z; // We rotate object on Y with Z axis gyro
        _gyroInitialRotation.w = gyro.w;
        print("Successfully recalibrated !");
    }
}