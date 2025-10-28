using SocketClient;
using UnityEngine;

public class TouchScreen : MonoBehaviour
{
    private bool _isTouchSupported;
    private Vector2 _lastPosition;
    private TouchPhase _lastPhase;

#if UNITY_EDITOR
    private void OnApplicationFocus(bool hasFocus)
#else
    private void Start()
#endif
    {
        _isTouchSupported = Input.touchSupported;
        if (!_isTouchSupported) return;
    }

    private void FixedUpdate()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector2 position = touch.position;
        TouchPhase phase = touch.phase;

        if (position == _lastPosition && phase == _lastPhase) return;

        TouchData data = new TouchData(position, phase);
        WebSocketClient.Instance.Send(NetworkedObject.MakePayload(data));
        //Debug.Log("Touch:" + position);

        _lastPosition = position;
        _lastPhase = phase;
    }
}
