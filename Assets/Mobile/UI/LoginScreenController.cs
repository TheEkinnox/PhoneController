using Shared.Utility;
using Shared.WebSocket;
using SocketClient;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginScreenController : MonoBehaviour
{
    private VisualElement _root;

    private Button _connectButton;
    private TextField _addressField;
    private Label _statusLabel;

    private WebSocketClient _client;
    [SerializeField] private WebSocketEventDispatchMode dispatchMode;

#if UNITY_EDITOR
    private void OnEnable()
#else
    private void Awake()
#endif
    {
        _client = WebSocketClient.Instance;
        _client.onOpen.AddListener(OnConnect);
        _client.onClose.AddListener(OnDisconnect);

#if DEBUG
        _client.onError.AddListener(Debug.LogError);
#endif

        _root = GetComponent<UIDocument>().rootVisualElement;

        // Load elements
        _connectButton = _root.Q<Button>("btn-connect");
        _addressField = _root.Q<TextField>("txt-address");
        _statusLabel = _root.Q<Label>("lbl-status");

        // TODO: Setup status label (Data source ? Manual text overriding ?)

        // Setup button
        _connectButton.clicked += Connect;
    }

    private void Update()
    {
        if (dispatchMode == WebSocketEventDispatchMode.Update)
            _client.DispatchEvents();
    }

    private void FixedUpdate()
    {
        if (dispatchMode == WebSocketEventDispatchMode.FixedUpdate)
            _client.DispatchEvents();
    }

    private void LateUpdate()
    {
        if (dispatchMode == WebSocketEventDispatchMode.LateUpdate)
            _client.DispatchEvents();
    }

#if UNITY_EDITOR
    private void OnDisable()
#else
    private void OnApplicationQuit()
#endif
    {
        Disconnect();
        _connectButton.clicked -= Connect;
    }

    private void Connect()
    {
        string[] tokens = _addressField.text.Split(':');
        TrueDebug.Assert(tokens.Length is 1 or 2);

        int port = tokens.Length == 2 ? int.Parse(tokens[1]) : WebSocketUtils.DefaultPort;
        _client.Connect(tokens[0], port);
    }

    private void Disconnect()
    {
        _client.Disconnect();
    }

    private void OnConnect()
    {
        _connectButton.text = "Disconnect";
        _connectButton.clicked -= Connect;
        _connectButton.clicked += Disconnect;
        _addressField.isReadOnly = true;
        TrueDebug.Log("Connected to server");

        _client.Send("Plop!");
    }

    private void OnDisconnect()
    {
        _connectButton.text = "Connect";
        _connectButton.clicked -= Disconnect;
        _connectButton.clicked += Connect;
        _addressField.isReadOnly = false;
        TrueDebug.Log("Disconnected from server");
    }
}