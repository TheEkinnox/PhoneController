using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Shared.Utility;
using Shared.WebSocket;
using UnityEngine;

namespace SocketServer
{
    /// <summary>
    /// A (partially) RFC6455 compliant socket server
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc6455"/>
    /// </summary>
    [Serializable]
    public class WebSocketServer : WebSocketBase
    {
        private TcpListener _listener;
        private TcpConnection _connection;

        private Thread _serverThread;
        private string _targetOrigin;

        [SerializeField] private int port = WebSocketUtils.DefaultPort;
        [SerializeField] private bool checkOrigin = true;

        public bool IsActive { get; private set; }

        public void Start()
        {
            TrueDebug.Assert(!IsActive);

            _targetOrigin = checkOrigin ? WebSocketUtils.TargetOrigin : null; // Needs to be set on the main thread
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            IsActive = true;

            _serverThread = new Thread(HandleConnection)
            {
                IsBackground = true,
                Name = $"[{nameof(WebSocketServer)}] Receive Thread"
            };
            _serverThread.Start();
        }

        public void Stop()
        {
            IsActive = false;
            _serverThread?.Interrupt();
            _serverThread?.Join();

            DispatchEvents();

            _listener = null;
            _connection = null;
        }

        private void HandleConnection()
        {
            TrueDebug.Log($"Server thread started - Endpoint: {_listener.LocalEndpoint}");

            Thread sendThread = null;
            try
            {
                while (IsActive)
                {
                    TcpClient client = _listener.AcceptTcpClientAsync().GetAwaiter().GetResult();

                    if (!IsActive || !client.Connected)
                        continue;

                    _connection = new TcpConnection(client)
                    {
                        Stream =
                        {
                            ReadTimeout = WebSocketUtils.HandshakeTimeout,
                            WriteTimeout = WebSocketUtils.HandshakeTimeout
                        }
                    };

                    if (!WaitForBytes(_connection, 3, WebSocketUtils.HandshakeTimeout)) // Waiting for GET
                    {
                        QueueError("Connection failed - Missing Handshake");

                        if (_connection.Stream.CanWrite)
                            _connection.Stream.Write(WebSocketClientHandshake.CreateHandshakeFailureResponse(null));

                        _connection.Close();
                        continue;
                    }

                    byte[] buffer = new byte[_connection.Client.Available];
                    int readBytes = _connection.Stream.Read(buffer, 0, buffer.Length);

                    WebSocketClientHandshake handshake = new(Encoding.UTF8.GetString(buffer, 0, readBytes));

                    (bool isSuccess, byte[] response) = handshake.GetResponse(_targetOrigin);
                    _connection.Stream.Write(response);

                    if (!isSuccess)
                    {
                        QueueError("Connection failed - Invalid Handshake");
                        _connection.Close();
                        continue;
                    }

                    QueueOpen();

                    _connection.Stream.ReadTimeout = WebSocketUtils.DefaultTimeout;
                    _connection.Stream.WriteTimeout = WebSocketUtils.DefaultTimeout;

                    sendThread = new Thread(() => SendEvents(_listener.Server, _connection))
                    {
                        IsBackground = true,
                        Name = $"[{nameof(WebSocketServer)}] Send Thread"
                    };
                    sendThread.Start();

                    ReceiveEvents(_connection);
                }
            }
            catch (ThreadInterruptedException)
            {
                // This is fine - We voluntarily killed the thread
                TrueDebug.Log("Server thread stopped");
            }
            catch (ThreadAbortException e)
            {
                // This might be fine - Still log the message just in case
                TrueDebug.LogWarning($"Server thread aborted\n{e.Message}");
            }
            catch (Exception e)
            {
                QueueError($"Server thread crashed\n{e.Message}\n{e.StackTrace}");
            }
            finally
            {
                _connection?.Close();
                _listener?.Stop();
                sendThread?.Interrupt();
            }
        }
    }
}