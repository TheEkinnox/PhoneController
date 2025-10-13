using System.Collections;
using System.Net.Sockets;
using Shared.WebSocket;
using SocketServer;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class QRCode : MonoBehaviour
{
    private RawImage _targetImage;

    private void Start()
    {
        _targetImage = GetComponent<RawImage>();
        StartCoroutine(GenerateQr());
        SocketServerComponent.Instance.OnOpen.AddListener(() => _targetImage.enabled = false);
        SocketServerComponent.Instance.OnClose.AddListener(() => _targetImage.enabled = true);
    }

    private IEnumerator GenerateQr()
    {
        string getIP = WebSocketUtils.GetLocalIPAddress(AddressFamily.InterNetwork).ToString();
        string url =
            $"https://api.qrserver.com/v1/create-qr-code/?data={UnityWebRequest.EscapeURL(getIP)}&size={256}x{256}";
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D qrTexture = DownloadHandlerTexture.GetContent(www);
            if (_targetImage != null)
                _targetImage.texture = qrTexture;
        }
        else
        {
            Debug.LogError("Failed to generate QR: " + www.error);
        }
    }
}