using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shared.Utility;
using UnityEngine;

namespace Shared.WebSocket
{
    /// <summary>
    /// A bunch of useful things for RFC6455 compliant WebSockets.
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc6455"/>
    /// </summary>
    public static class WebSocketUtils
    {
        public static readonly Version HttpVersion = new(1, 1);
        public static readonly string HttpStr = $"HTTP/{HttpVersion}";
        public const string EOL = "\r\n"; // The HTTP/1.1 standard defines CR LF as the default line ending

        private static readonly Regex body = new(@"([A-Za-z0-9-]+): ?([^\n^\r]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public const int DefaultPort = 443;
        public const int HandshakeTimeout = 5_000;
        public const int DefaultTimeout = 300_000;

        public static string TargetOrigin => Application.identifier.Replace("-", "");

        public static string MakeAcceptHeader(string key)
        {
            const string socketKeySuffix = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            byte[] socketHash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{key}{socketKeySuffix}"));
            return Convert.ToBase64String(socketHash);
        }

        public static Dictionary<string, string> ParseHeaders(string data)
        {
            MatchCollection matches = body.Matches(data);
            Dictionary<string, string> headers = new();

            foreach (Match match in matches)
                headers.Add(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());

            return headers;
        }

        public static IPAddress GetLocalIPAddress(AddressFamily family)
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == family)
                {
                    return ip;
                }
            }

            TrueDebug.LogError("No network adapters with a valid IP for the given address family found");
            return IPAddress.None;
        }
    }
}