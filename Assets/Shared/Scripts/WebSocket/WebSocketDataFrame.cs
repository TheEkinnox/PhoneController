using System;
using Shared.Scripts;
using UnityEngine;

namespace Shared.WebSocket
{
    public enum WebSocketOpCode
    {
        Continuation = 0x0,
        Text = 0x1,
        Binary = 0x2,
        Close = 0x8,
        Ping = 0x9,
        Pong = 0xA
    }

    /// <summary>
    /// An RFC6455 compliant data frame.
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc6455"/>
    /// </summary>
    public class WebSocketDataFrame
    {
        private const int ShortPayloadHint = 126;
        private const int LongPayloadHint = 127;

        public bool IsFinalFragment { get; private set; }
        public bool HasMask { get; private set; }
        public WebSocketOpCode OpCode { get; private set; }
        private byte LengthHint { get; set; }
        public int PayloadLength { get; private set; } // TODO: Support 64bit payload length ?
        public int Offset { get; private set; }
        public byte[] Data { get; set; }

        public bool IsShortPayload => LengthHint == ShortPayloadHint;
        public bool IsLongPayload => LengthHint == LongPayloadHint;

        public WebSocketDataFrame()
        {
        }

        public WebSocketDataFrame(WebSocketOpCode opCode, byte[] payload, bool isFinalFragment = true)
        {
            IsFinalFragment = isFinalFragment;
            HasMask = false; // TODO: Support masks ?
            OpCode = opCode;
            PayloadLength = payload.Length;
            byte[] lengthBytes;

            switch (PayloadLength)
            {
                case < ShortPayloadHint:
                    LengthHint = (byte)PayloadLength;
                    lengthBytes = Array.Empty<byte>();
                    break;
                case < ushort.MaxValue:
                    LengthHint = ShortPayloadHint;
                    lengthBytes = BitConverter.GetBytes((ushort)PayloadLength);
                    Offset += 2;
                    break;
                default:
                    LengthHint = LongPayloadHint;
                    lengthBytes = BitConverter.GetBytes((ulong)PayloadLength);
                    Offset += 8;
                    break;
            }

            Offset = 2 + lengthBytes.Length;

            int payloadOffset = Offset + (HasMask ? 4 : 0);
            Data = new byte[payloadOffset + PayloadLength];
            Data[0] = (byte)((IsFinalFragment ? 1 << 7 : 0) | ((byte)OpCode & ((1 << 4) - 1)));
            Data[1] = (byte)((HasMask ? 1 << 7 : 0) | (LengthHint & ((1 << 7) - 1)));

            for (int i = 0; i < lengthBytes.Length; ++i)
                Data[2 + i] = lengthBytes[BitConverter.IsLittleEndian ? lengthBytes.Length - 1 - i : i];

            if (HasMask)
            {
                // TODO: Support masks ?
                for (int i = Offset; i < 4; ++i)
                    Data[i] = 0;

                for (int i = 0; i < PayloadLength; i++)
                {
                    Data[payloadOffset + i] = (byte)(payload[i] ^ Data[Offset + i % 4]);
                }
            }
            else
            {
                Array.Copy(payload, 0, Data, payloadOffset, PayloadLength);
            }

#if DEBUG
            Debug.Log($"Created data frame\n{Data.Flatten()}");
#endif
        }

        public void ParseHeader(byte[] bytes)
        {
            IsFinalFragment = (bytes[0] & (1 << 7)) != 0;
            OpCode = (WebSocketOpCode)(bytes[0] & ((1 << 4) - 1));

            HasMask = (bytes[1] & (1 << 7)) != 0;
            LengthHint = (byte)(bytes[1] & ((1 << 7) - 1));

            Offset = 2;
            Data = bytes;
        }

        public void ParseLength()
        {
            if (IsShortPayload)
            {
                if (BitConverter.IsLittleEndian)
                {
                    PayloadLength = BitConverter.ToUInt16(new[] { Data[Offset + 1], Data[Offset] }, 0);
                }
                else
                {
                    PayloadLength = BitConverter.ToUInt16(Data, Offset);
                }

                Offset += 2;
            }
            else if (IsLongPayload)
            {
                if (BitConverter.IsLittleEndian)
                {
                    PayloadLength = (int)BitConverter.ToUInt64(new[]
                    {
                        Data[Offset + 7], Data[Offset + 6], Data[Offset + 5], Data[Offset + 4],
                        Data[Offset + 3], Data[Offset + 2], Data[Offset + 1], Data[Offset]
                    }, 0);
                }
                else
                {
                    PayloadLength = (int)BitConverter.ToUInt64(Data, Offset);
                }

                Offset += 8;
            }
            else
            {
                PayloadLength = LengthHint;
            }
        }

        public byte[] DecodeData()
        {
            if (PayloadLength <= 0)
                return null;

            byte[] buffer = new byte[PayloadLength];

            if (HasMask)
            {
                byte[] masks = { Data[Offset], Data[Offset + 1], Data[Offset + 2], Data[Offset + 3] };

                int payloadOffset = Offset + 4;

                for (int i = 0; i < PayloadLength; i++)
                {
                    buffer[i] = (byte)(Data[payloadOffset + i] ^ masks[i % 4]);
                }
            }
            else
            {
                Array.Copy(Data, Offset, buffer, 0, PayloadLength);
            }

            return buffer;
        }
    }
}