using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

using Shared.WebSocket;
using TMPro;

public class ShowIP : MonoBehaviour
{
    private TMP_Text _ipText;

    void Start()
    {
        _ipText = GetComponent<TMP_Text>();
        _ipText.text = $"Local IP: {WebSocketUtils.GetLocalIPAddress(AddressFamily.InterNetwork)}";
    }
}
