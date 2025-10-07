using UnityEngine;
using SocketClient;

public class Compass : MonoBehaviour
{

    private float _lastDirection;
#if UNITY_EDITOR
    private void OnApplicationFocus(bool hasFocus)
#else
    private void Start()
#endif
    {
        Input.location.Start();
        Input.compass.enabled = true;
        
    }

    private void FixedUpdate()
    {
        // Read the compass heading in degrees (0–360)
        float heading = Input.compass.trueHeading;

        // Print it to console
        Debug.Log("Compass Heading: " + heading);

        // Example: rotate an object to match compass
        //transform.rotation = Quaternion.Euler(0, -heading, 0);
        
        
        if (heading == _lastDirection)
            return;

        CompassData data = new CompassData(heading);
        WebSocketClient.Instance.Send(NetworkedObject.MakePayload(data));
        _lastDirection = heading;
        
        //Debug.Log(data.value);
    }
}
