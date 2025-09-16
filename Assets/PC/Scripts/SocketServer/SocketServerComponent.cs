using System.Net.Sockets;
using System.Text;
using Shared.Scripts;
using Shared.WebSocket;
using UnityEngine;

namespace SocketServer
{
    public class SocketServerComponent : MonoBehaviour
    {
        [SerializeField] private WebSocketServer server = new();

        public WebSocketOpenEvent OnOpen => server.onOpen;
        public WebSocketMessageEvent OnMessage => server.onMessage;
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
            OnMessage.AddListener((isText, message) =>
            {
                if (isText)
                    Debug.Log($"Text message received: {Encoding.UTF8.GetString(message)}");
                else
                    Debug.Log($"Binary message received: {message.Flatten()}");
            });
#endif

            server.Start();
            Debug.Log($"Local IP: {WebSocketUtils.GetLocalIPAddress(AddressFamily.InterNetwork)}");
        }

        private void FixedUpdate()
        {
            server.DispatchEvents();
        }

        private void OnApplicationQuit()
        {
            server.Stop();
        }
    }
}