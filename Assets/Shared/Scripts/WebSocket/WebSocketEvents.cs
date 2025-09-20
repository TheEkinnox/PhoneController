using System;
using System.Text;
using Shared.Utility;
using UnityEngine.Events;

namespace Shared.WebSocket
{
    [Serializable]
    public class WebSocketOpenEvent : UnityEvent
    {
    }

    [Serializable]
    public class WebSocketTextMessageEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public class WebSocketBinaryMessageEvent : UnityEvent<byte[]>
    {
    }

    [Serializable]
    public class WebSocketErrorEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public class WebSocketCloseEvent : UnityEvent
    {
    }

    public enum WebSocketEventType
    {
        Open,
        Text,
        Binary,
        Error,
        Close
    }

    public enum WebSocketEventDispatchMode
    {
        Update,
        FixedUpdate,
        LateUpdate
    }

    public readonly struct WebSocketEvent
    {
        public readonly WebSocketEventType type;
        public readonly byte[] data;

        public WebSocketEvent(WebSocketEventType type)
        {
            TrueDebug.Assert(type is WebSocketEventType.Open or WebSocketEventType.Close);
            this.type = type;
            data = null;
        }

        public WebSocketEvent(WebSocketEventType type, byte[] data)
        {
            this.type = type;
            this.data = data;
        }

        public WebSocketEvent(WebSocketEventType type, string data)
        {
            TrueDebug.Assert(type is WebSocketEventType.Text or WebSocketEventType.Error);
            this.type = type;
            this.data = Encoding.UTF8.GetBytes(data);
        }
    }
}