using SocketServer;
using UnityEngine;

[DefaultExecutionOrder(SocketServerComponent.ExecutionOrder + 1)]
public abstract class NetworkedBehaviour : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        SocketServerComponent.Instance.OnTextMessage.AddListener(OnMessage);
    }

    protected virtual void OnDisable()
    {
        SocketServerComponent.Instance?.OnTextMessage?.RemoveListener(OnMessage);
    }

    private void OnMessage(string message)
    {
        NetworkedObject networkedObject = new(message);

        if (networkedObject.IsValid())
            HandleMessage(new NetworkedObject(message));
    }

    protected abstract void HandleMessage(NetworkedObject data);
}