using UnityEngine;

public class GyroscopeRotation : MonoBehaviour
{
    private Gyroscope _gyroscope;
    private bool _isGyroSupported;
    [SerializeField] private GameObject testObj;
    void Start()
    {
        _isGyroSupported = SystemInfo.supportsGyroscope;

        if (_isGyroSupported)
        {
            _gyroscope = Input.gyro;
            _gyroscope.enabled = true;
        }
    }

    void Update()
    {
        if (!_isGyroSupported)
            return;
        
        Quaternion deviceRotation = Input.gyro.attitude;
        
        testObj.transform.rotation = deviceRotation;
    }
}
