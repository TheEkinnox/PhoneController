using UnityEngine;
using SocketClient;

public class Accelerometer : MonoBehaviour
{
    private bool _isAccSupported;

#if UNITY_EDITOR
    private void OnApplicationFocus(bool hasFocus)
#else
    private void Start()
#endif
    {
        _isAccSupported = SystemInfo.supportsAccelerometer;
        
    }

    private void FixedUpdate()
    {
        #if !UNITY_EDITOR
        if (!_isAccSupported) return;
        #endif
        
        if (Input.acceleration.sqrMagnitude <= 1.1f) return;

        Vector3 acceleration = Input.acceleration - Vector3.back;
        

        AccData data = new AccData(acceleration);
        WebSocketClient.Instance.Send(NetworkedObject.MakePayload(data));
    }
}
