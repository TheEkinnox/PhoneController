using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Shared.Utility;
using Shared.WebSocket;

namespace SocketServer
{
    /// <summary>
    /// An RFC6455 compliant WebSocket client handshake validator.
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc6455"/>
    /// </summary>
    public readonly struct WebSocketClientHandshake
    {
        private static readonly Regex header = new("^(GET|POST|PUT|DELETE|OPTIONS) (.+) HTTP/([0-9.]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public readonly string method;
        public readonly string uri;
        public readonly Version version;
        public readonly Dictionary<string, string> headers;

        public WebSocketClientHandshake(string data)
        {
            MatchCollection matches = header.Matches(data);
            if (matches.Count >= 1)
            {
                // If there is more than one the client sent weird stuff...
                TrueDebug.Assert(matches.Count == 1);
                Match match = matches[0];

                method = match.Groups[1].Value.Trim();
                uri = match.Groups[2].Value.Trim();
                version = new Version(match.Groups[3].Value.Trim());
            }
            else
            {
                method = null;
                uri = null;
                version = null;
            }

            headers = WebSocketUtils.ParseHeaders(data);
        }

        // See https://datatracker.ietf.org/doc/html/rfc6455#section-4.2.1
        private (bool, string) IsValid(string targetOrigin)
        {
            if (version < WebSocketUtils.HttpVersion)
            {
                return (false, $"Invalid HTTP version - Expected {WebSocketUtils.HttpVersion}+, received \"{version}\"");
            }

            if (!string.Equals(method, "GET", StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, $"Invalid request type - Expected GET, received \"{method}\"");
            }

            if (!headers.ContainsKey("Host"))
            {
                return (false, "Host missing from request header");
            }

            if (!headers.TryGetValue("Upgrade", out string up) || !string.Equals(up, "websocket", StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, "Missing websocket upgrade");
            }

            if (!headers.TryGetValue("Connection", out string connection) || !connection.Contains("Upgrade"))
            {
                return (false, "Request does not have Connection: Upgrade");
            }

            if (!headers.ContainsKey("Sec-WebSocket-Key"))
            {
                return (false, "Sec-WebSocket-Key missing from request header");
            }

            if (!headers.TryGetValue("Sec-WebSocket-Version", out string ver) || !string.Equals(ver, "13"))
            {
                return (false, $"Invalid socket version - Expected 13, received {ver}");
            }

            if (!string.IsNullOrEmpty(targetOrigin))
            {
                if (!headers.TryGetValue("Origin", out string origin))
                {
                    return (false, "Origin missing from request header");
                }

                if (!string.Equals(origin, targetOrigin))
                {
                    return (false, $"Invalid socket origin: {origin}");
                }
            }

            return (true, null);
        }

        public (bool, byte[]) GetResponse(string targetOrigin)
        {
            (bool isSuccess, string message) = IsValid(targetOrigin);

            return (isSuccess, isSuccess ? CreateHandshakeSuccessResponse(headers["Sec-WebSocket-Key"]) : CreateHandshakeFailureResponse(message));
        }

        // See https://datatracker.ietf.org/doc/html/rfc6455#section-4.2.2
        private static byte[] CreateHandshakeSuccessResponse(string socketKey)
        {
            StringBuilder response = new();

            response.Append($"{WebSocketUtils.HttpStr} 101 Switching Protocols{WebSocketUtils.EOL}");
            response.Append($"Connection: Upgrade{WebSocketUtils.EOL}");
            response.Append($"Upgrade: websocket{WebSocketUtils.EOL}");
            response.Append($"Sec-WebSocket-Accept: {WebSocketUtils.MakeAcceptHeader(socketKey)}{WebSocketUtils.EOL}{WebSocketUtils.EOL}");

            return Encoding.UTF8.GetBytes(response.ToString());
        }

        public static byte[] CreateHandshakeFailureResponse(string error)
        {
            string errorMessage = "Invalid WebSocket handshake";
            string status = "400 Bad Request";

            if (!string.IsNullOrEmpty(error))
            {
                errorMessage += $" - {error}";

                if (error.Contains("origin", StringComparison.InvariantCultureIgnoreCase))
                    status = "403 Forbidden";
            }

            TrueDebug.Log(errorMessage);

            StringBuilder response = new();
            response.Append($"{WebSocketUtils.HttpStr} {status}{WebSocketUtils.EOL}");
            response.Append($"Connection: close{WebSocketUtils.EOL}");
            response.Append($"Content-Type: text/plain; charset=utf-8{WebSocketUtils.EOL}");
            response.Append($"Content-Length: {errorMessage.Length + 1}{WebSocketUtils.EOL}{WebSocketUtils.EOL}");
            response.Append($"{errorMessage}");

            return Encoding.UTF8.GetBytes(response.ToString());
        }
    }
}