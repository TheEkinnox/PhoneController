using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Shared.Utility;
using Shared.WebSocket;
using UnityEngine;

namespace SocketClient
{
    /// <summary>
    ///     A (partially) RFC6455 compliant socket client
    ///     See <see href="https://datatracker.ietf.org/doc/html/rfc6455" />
    /// </summary>
    [Serializable]
    public class WebSocketClient : WebSocketBase
    {
        private static WebSocketClient _instance;

        private Thread _clientThread;
        private TcpConnection _connection;
        private string _targetOrigin;

        [NotNull]
        public static WebSocketClient Instance
        {
            get
            {
                _instance ??= new WebSocketClient();
                return _instance;
            }
        }

        public void Connect(string host, int port)
        {
            Debug.Assert(_connection?.Client is not { Connected: true });
            Debug.Assert(_clientThread is not { IsAlive: true });

            host = host.Trim();

            if (string.IsNullOrEmpty(host))
            {
                onError?.Invoke("Failed to connect to server - Host is empty");
                return;
            }

            _targetOrigin = WebSocketUtils.TargetOrigin; // Needs to be set on the main thread
            _clientThread = new Thread(() => HandleConnection(host, port));
            _clientThread.IsBackground = true;
            _clientThread.Start();
        }

        public void Disconnect()
        {
            if (_connection?.Client?.Connected is not true)
                return;

            onClose?.Invoke();

            _clientThread?.Interrupt();
            _clientThread?.Join();

            _connection = null;
            _clientThread = null;
        }

        private void HandleConnection(string host, int port)
        {
            TrueDebug.Log($"Client thread started - Host: {host}, Port: {port}");

            Thread sendThread = null;
            try
            {
                string socketKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

                _connection = new TcpConnection(new TcpClient(host, port))
                {
                    Stream =
                    {
                        ReadTimeout = WebSocketUtils.HandshakeTimeout,
                        WriteTimeout = WebSocketUtils.HandshakeTimeout
                    }
                };

                _connection.Stream.Write(CreateClientHandshake(host, socketKey, _targetOrigin));

                if (!WaitForBytes(_connection, WebSocketUtils.HttpStr.Length, WebSocketUtils.HandshakeTimeout))
                {
                    QueueError("Failed to connect to server - Missing handshake response");
                    return;
                }

                byte[] buffer = new byte[_connection.Client.Available];
                int readBytes = _connection.Stream.Read(buffer, 0, buffer.Length);

                WebSocketServerHandshake handshake = new(Encoding.UTF8.GetString(buffer, 0, readBytes));
                (bool isSuccess, string message) = handshake.IsValid(socketKey);

                if (!isSuccess)
                {
                    QueueError($"Handshake failed - {message}");
                    _connection.Close();
                    return;
                }

                _connection.Stream.ReadTimeout = WebSocketUtils.DefaultTimeout;
                _connection.Stream.WriteTimeout = WebSocketUtils.DefaultTimeout;

                QueueOpen();

                sendThread = new Thread(() => SendEvents(_connection.Client.Client, _connection));
                sendThread.IsBackground = true;
                sendThread.Start();

                ReceiveEvents(_connection);
            }
            catch (ThreadInterruptedException)
            {
                // This is fine - We just stopped the thread
                TrueDebug.Log("Client thread stopped");
            }
            catch (ThreadAbortException e)
            {
                // This might be fine - Still log the message just in case
                TrueDebug.LogWarning($"Client thread aborted\n{e.Message}");
            }
            catch (Exception e)
            {
                QueueError($"Client thread crashed\n{e.Message}");
#if DEBUG
                throw;
#endif
            }
            finally
            {
                Close(_connection);
                sendThread?.Interrupt();
            }
        }

        // See https://datatracker.ietf.org/doc/html/rfc6455#section-4.2.1
        private static byte[] CreateClientHandshake(string host, string socketKey, string origin)
        {
            StringBuilder request = new();

            request.Append($"GET / {WebSocketUtils.HttpStr}{WebSocketUtils.EOL}");
            request.Append($"Host: {host}{WebSocketUtils.EOL}");
            request.Append($"Connection: Upgrade{WebSocketUtils.EOL}");
            request.Append($"Upgrade: websocket{WebSocketUtils.EOL}");
            request.Append($"Sec-WebSocket-Key: {socketKey}{WebSocketUtils.EOL}");
            request.Append($"Sec-WebSocket-Version: 13{WebSocketUtils.EOL}");
            request.Append($"Origin: {origin}{WebSocketUtils.EOL}");

            TrueDebug.Log($"Created handshake:\n{request}");
            return Encoding.UTF8.GetBytes(request.ToString());
        }
    }
}