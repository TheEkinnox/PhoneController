using UnityEngine;
using System.Runtime.InteropServices;

public class QRScannerManager : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void StartQRScanner();

    [DllImport("__Internal")]
    private static extern void StopQRScanner();
#endif

    void Start()
    {
#if UNITY_IOS && !UNITY_EDITOR
        StartQRScanner();
#endif
    }

    public void OnQRDetected(string value)
    {
        Debug.Log("✅ QR Code Detected: " + value);
        // Handle your logic here, for example:
        // SceneManager.LoadScene(value);
    }

    void OnDestroy()
    {
#if UNITY_IOS && !UNITY_EDITOR
        StopQRScanner();
#endif
    }
}