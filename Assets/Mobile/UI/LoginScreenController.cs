using Shared.Utility;
using Shared.WebSocket;
using SocketClient;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginScreenController : MonoBehaviour
{
    private enum Estate
    {
        Disconnected,
        Connecting,
        Connected
    }
    private CameraQRScanner _qrScannerController;
    private VisualElement _root;

    private Button _connectButton;
    private TextField _addressField;
    private Label _statusLabel;

    private WebSocketClient _client;
    private Estate _state;

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

        _client.onError.AddListener(OnError);

        _root = GetComponent<UIDocument>().rootVisualElement;

        // Load elements
        _connectButton = _root.Q<Button>("btn-connect");
        _addressField = _root.Q<TextField>("txt-address");
        _statusLabel = _root.Q<Label>("lbl-status");
        _qrScannerController = FindFirstObjectByType<CameraQRScanner>();
        _qrScannerController.OnQrAction += QrRead;
        

#if UNITY_EDITOR
        _addressField.value = "localhost";
#endif

        _statusLabel.text = string.Empty;
        _state = Estate.Disconnected;

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
        _qrScannerController.OnQrAction -= QrRead;

    }

    private void Connect()
    {
        string[] tokens = _addressField.text.Split(':');
        TrueDebug.Assert(tokens.Length is 1 or 2);

        int port = tokens.Length == 2 ? int.Parse(tokens[1]) : WebSocketUtils.DefaultPort;
        _client.Connect(tokens[0], port);
        _connectButton.text = "Connecting...";
        _connectButton.enabledSelf = false;
        _statusLabel.text = string.Empty;
        _state = Estate.Connecting;
    }

    private void Disconnect()
    {
        _client.Disconnect();
    }

    private void OnConnect()
    {
        _connectButton.text = "Disconnect";
        _connectButton.enabledSelf = true;
        _connectButton.clicked -= Connect;
        _connectButton.clicked += Disconnect;
        _addressField.isReadOnly = true;
        _statusLabel.text = string.Empty;
        _state = Estate.Connected;
        TrueDebug.Log("Connected to server");

        _client.Send("Plop!");
    }

    private void OnDisconnect()
    {
        _connectButton.text = "Connect";
        _connectButton.enabledSelf = true;

        if (_state == Estate.Connected)
        {
            _connectButton.clicked -= Disconnect;
            _connectButton.clicked += Connect;
            TrueDebug.Log("Disconnected from server");
        }

        _addressField.isReadOnly = false;
        _statusLabel.text = string.Empty;
        _state = Estate.Disconnected;
    }

    private void OnError(string message)
    {
#if DEBUG
        Debug.LogError(message);
#endif

        if (_state == Estate.Connecting && !_client.IsConnected)
            OnDisconnect();

        _statusLabel.text = message;
    }

    private void QrRead(string qrValue)
    {
        _addressField.value = qrValue;
        Connect();
    }
}