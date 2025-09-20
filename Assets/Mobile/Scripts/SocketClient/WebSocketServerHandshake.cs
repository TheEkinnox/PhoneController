using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Shared.Utility;
using Shared.WebSocket;

namespace SocketClient
{
    /// <summary>
    /// An RFC6455 compliant WebSocket server handshake validator.
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc6455"/>
    /// </summary>
    public readonly struct WebSocketServerHandshake
    {
        private static readonly Regex header = new(@"^HTTP/([0-9.]+) (\d+) .*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public readonly Version version;
        public readonly string status;
        public readonly Dictionary<string, string> headers;

        public WebSocketServerHandshake(string data)
        {
            MatchCollection matches = header.Matches(data);
            if (matches.Count >= 1)
            {
                // If there is more than one the server sent weird stuff...
                TrueDebug.Assert(matches.Count == 1);
                Match match = matches[0];

                version = new Version(match.Groups[1].Value.Trim());
                status = match.Groups[2].Value;
            }
            else
            {
                version = null;
                status = null;
            }

            headers = WebSocketUtils.ParseHeaders(data);
        }

        // See https://datatracker.ietf.org/doc/html/rfc6455#section-4.2.1
        public (bool, string) IsValid(string socketKey)
        {
            if (version < WebSocketUtils.HttpVersion)
            {
                return (false, $"Invalid HTTP version - Expected {WebSocketUtils.HttpVersion}+, received \"{version}\"");
            }

            if (!string.Equals(status, "101"))
            {
                return (false, $"Invalid status code - Expected 101, received \"{status}\""); // TODO: Process error codes - might be a client error
            }

            if (!headers.TryGetValue("Upgrade", out string up) || !string.Equals(up, "websocket", StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, "Missing websocket upgrade");
            }

            if (!headers.TryGetValue("Connection", out string connection) || !connection.Contains("Upgrade"))
            {
                return (false, "Request does not have Connection: Upgrade");
            }

            string expectedAccept = WebSocketUtils.MakeAcceptHeader(socketKey);
            if (!headers.TryGetValue("Sec-WebSocket-Accept", out string accept) ||
                !string.Equals(accept, expectedAccept, StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, "Invalid Sec-WebSocket-Accept header");
            }

            return (true, null);
        }
    }
}