using System.Net.Sockets;
using Shared.Utility;
using Shared.WebSocket;
using UnityEngine;

namespace SocketServer
{
    [DefaultExecutionOrder(ExecutionOrder)]
    public class SocketServerComponent : MonoBehaviour
    {
        public const int ExecutionOrder = -1500;

        [SerializeField] private WebSocketServer server = new();
        [SerializeField] private WebSocketEventDispatchMode dispatchMode;

        public WebSocketOpenEvent OnOpen => server.onOpen;
        public WebSocketTextMessageEvent OnTextMessage => server.onTextMessage;
        public WebSocketBinaryMessageEvent OnBinaryMessage => server.onBinaryMessage;
        public WebSocketErrorEvent OnError => server.onError;
        public WebSocketCloseEvent OnClose => server.onClose;

        public static SocketServerComponent Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(this);
                return;
            }

            Instance = this;

#if DEBUG
            OnOpen.AddListener(() => { Debug.Log("Client connected"); });
            OnClose.AddListener(() => { Debug.Log("Client disconnected"); });
            OnError.AddListener(Debug.LogError);
            OnTextMessage.AddListener(message => { Debug.Log($"Text message received: {message}"); });
            OnBinaryMessage.AddListener(message => { Debug.Log($"Binary message received: {message.Flatten()}"); });
#endif

            server.Start();
            TrueDebug.Log($"Local IP: {WebSocketUtils.GetLocalIPAddress(AddressFamily.InterNetwork)}");
        }

        private void Update()
        {
            if (dispatchMode == WebSocketEventDispatchMode.Update)
                server.DispatchEvents();
        }

        private void FixedUpdate()
        {
            if (dispatchMode == WebSocketEventDispatchMode.FixedUpdate)
                server.DispatchEvents();
        }

        private void LateUpdate()
        {
            if (dispatchMode == WebSocketEventDispatchMode.LateUpdate)
                server.DispatchEvents();
        }

        private void OnApplicationQuit()
        {
            server.Stop();
        }
    }
}