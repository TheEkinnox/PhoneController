﻿using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Shared.Utility;
using UnityEngine;

namespace Shared.WebSocket
{
    public static class StreamExtensions
    {
        public static void ReadExactly(this NetworkStream stream, byte[] bytes, int offset, int count)
        {
            if (count == 0)
                return;

            int readBytes = stream.Read(bytes, offset, count);
            TrueDebug.Assert(readBytes == count);
        }
    }

    public abstract class WebSocketBase
    {
        private readonly struct WebSocketMessage
        {
            public readonly WebSocketOpCode opCode;
            public readonly byte[] data;

            public WebSocketMessage(WebSocketOpCode opCode, byte[] data)
            {
                this.opCode = opCode;
                this.data = data;
            }
        }

        private ConcurrentQueue<WebSocketEvent> _eventQueue = new();
        private ConcurrentQueue<WebSocketMessage> _messageQueue = new();

        [Space] public WebSocketOpenEvent onOpen = new();
        public WebSocketTextMessageEvent onTextMessage = new();
        public WebSocketBinaryMessageEvent onBinaryMessage = new();
        public WebSocketErrorEvent onError = new();
        public WebSocketCloseEvent onClose = new();

        protected void QueueOpen()
        {
            _eventQueue.Enqueue(new WebSocketEvent(WebSocketEventType.Open));
        }

        protected void QueueClose()
        {
            _eventQueue.Enqueue(new WebSocketEvent(WebSocketEventType.Close));
        }

        protected void QueueMessage(string message)
        {
            _eventQueue.Enqueue(new WebSocketEvent(WebSocketEventType.Text, message));
        }

        protected void QueueMessage(byte[] message, bool isText)
        {
            _eventQueue.Enqueue(new WebSocketEvent(isText ? WebSocketEventType.Text : WebSocketEventType.Binary, message));
        }

        protected void QueueError(string message)
        {
#if DEBUG
            _eventQueue.Enqueue(new WebSocketEvent(WebSocketEventType.Error,
                $"{message}\n\nStackTrace:\n{new System.Diagnostics.StackTrace(1, true)}"));
#else
            _eventQueue.Enqueue(new WebSocketEvent(WebSocketEventType.Error, message));
#endif
        }

        protected void SendEvents(Socket socket, params TcpConnection[] connections)
        {
            _messageQueue.Clear();

            while (socket.Connected)
            {
                while (socket.Connected && _messageQueue.TryDequeue(out WebSocketMessage message))
                {
                    foreach (TcpConnection connection in connections)
                    {
                        if (!socket.Connected)
                            break;

                        if (!connection.Client.Connected)
                            continue;

                        SendImpl(connection.Stream, message.opCode, message.data);
                    }
                }
            }

            _messageQueue.Clear();
        }

        protected void ReceiveEvents(TcpConnection connection)
        {
            while (connection?.Client?.Connected is true)
            {
                WebSocketDataFrame frame = ReadFrame(connection);

                if (frame == null)
                {
                    TrueDebug.LogWarning("Frame dropped");
                    continue;
                }

                if (!frame.IsFinalFragment)
                {
                    TrueDebug.LogWarning("Fragmented frame received");
                    continue;
                }

                switch (frame.OpCode)
                {
                    case WebSocketOpCode.Text:
                        QueueMessage(frame.DecodeData(), true);
                        break;
                    case WebSocketOpCode.Binary:
                        QueueMessage(frame.DecodeData(), false);
                        break;
                    case WebSocketOpCode.Close:
                        connection.Close();
                        break;
                    default:
                        TrueDebug.LogWarning("Unknown opcode: " + frame.OpCode);
                        break;
                }
            }

            QueueClose();
        }

        public void DispatchEvents()
        {
            if (_eventQueue == null)
                return;

            while (_eventQueue.TryDequeue(out WebSocketEvent socketEvent))
            {
                switch (socketEvent.type)
                {
                    case WebSocketEventType.Open:
                        onOpen?.Invoke();
                        break;
                    case WebSocketEventType.Text:
                        onTextMessage?.Invoke(Encoding.UTF8.GetString(socketEvent.data));
                        break;
                    case WebSocketEventType.Binary:
                        onBinaryMessage?.Invoke(socketEvent.data);
                        break;
                    case WebSocketEventType.Error:
                        onError?.Invoke(Encoding.UTF8.GetString(socketEvent.data));
                        break;
                    case WebSocketEventType.Close:
                        onClose?.Invoke();
                        break;
                    default:
                        TrueDebug.LogWarning($"Unhandled event type: {socketEvent.type}");
                        break;
                }
            }
        }

        public void Send(string message)
        {
            _messageQueue.Enqueue(new WebSocketMessage(WebSocketOpCode.Text, Encoding.UTF8.GetBytes(message)));
        }

        public void Send(byte[] data, bool isText)
        {
            _messageQueue.Enqueue(new WebSocketMessage(isText ? WebSocketOpCode.Text : WebSocketOpCode.Binary, data));
        }

        protected static void Close(TcpConnection connection)
        {
            if (connection?.Stream?.CanWrite is true)
                SendImpl(connection.Stream, WebSocketOpCode.Close, Array.Empty<byte>());

            connection?.Close();
        }

        private static void SendImpl(NetworkStream stream, WebSocketOpCode opCode, byte[] payload)
        {
            TrueDebug.Assert(stream.CanWrite);

            WebSocketDataFrame frame = new(opCode, payload);
            stream.Write(frame.Data);

            //TrueDebug.Log($"{opCode} frame sent\n{(opCode == WebSocketOpCode.Text ? Encoding.UTF8.GetString(payload) : payload.Flatten())}");
        }

        protected bool WaitForBytes(TcpConnection connection, int count, int timeout = WebSocketUtils.DefaultTimeout)
        {
            if (count == 0)
                return true;

            DateTime endTime = timeout > 0 ? DateTime.Now + TimeSpan.FromMilliseconds(timeout) : DateTime.MaxValue;
            DateTime currentTime = DateTime.Now;

            while (currentTime < endTime && connection?.Client?.Connected is true &&
                   !(connection.Stream?.DataAvailable is true && connection.Client.Available >= count))
            {
                Thread.Sleep(0);
                currentTime = DateTime.Now;
            }

            if (currentTime >= endTime)
            {
                QueueError("Read Timeout");
                return false;
            }

            if (connection?.Client?.Connected is not true)
            {
                QueueError("Connection lost");
                return false;
            }

            if (connection.Client.Available < count)
            {
                QueueError("Not enough data");
                return false;
            }

            return true;
        }

        private WebSocketDataFrame ReadFrame(TcpConnection connection)
        {
            const int dataFrameHeader = 2;
            const int shortPayloadSizeLength = 2;
            const int longPayloadSizeLength = 8;
            const int maskLength = 4;

            if (!WaitForBytes(connection, dataFrameHeader))
                return null;

            byte[] buffer = new byte[dataFrameHeader + longPayloadSizeLength + maskLength];
            connection.Stream.ReadExactly(buffer, 0, dataFrameHeader);

            WebSocketDataFrame frame = new();
            frame.ParseHeader(buffer);

            int offset = dataFrameHeader;

            if (frame.IsShortPayload)
            {
                if (!WaitForBytes(connection, shortPayloadSizeLength))
                    return null;

                connection.Stream.ReadExactly(buffer, offset, shortPayloadSizeLength);
                offset += shortPayloadSizeLength;
            }
            else if (frame.IsLongPayload)
            {
                if (!WaitForBytes(connection, longPayloadSizeLength))
                    return null;

                connection.Stream.ReadExactly(buffer, offset, longPayloadSizeLength);
                offset += longPayloadSizeLength;
            }

            frame.ParseLength();

            if (frame.HasMask)
            {
                if (!WaitForBytes(connection, maskLength))
                    return null;

                connection.Stream.ReadExactly(buffer, offset, maskLength);
                offset += maskLength;
            }

            Array.Resize(ref buffer, offset + frame.PayloadLength);
            connection.Stream.ReadExactly(buffer, offset, frame.PayloadLength);
            frame.Data = buffer;

            // TrueDebug.Log($"Received data frame\n{buffer.Flatten()}");
            return frame;
        }
    }
}