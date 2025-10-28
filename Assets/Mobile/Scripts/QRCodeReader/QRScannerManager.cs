using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class QRScannerController : MonoBehaviour
{
    public event Action<string> OnQrAction;
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void StartQRScanner();
    [DllImport("__Internal")] private static extern void StopQRScanner();
#endif

    void Start()
    {
#if UNITY_IOS && !UNITY_EDITOR
        StartQRScanner();
#endif
    }

    void OnDestroy()
    {
#if UNITY_IOS && !UNITY_EDITOR
        StopQRScanner();
#endif
    }

    // Called from native
    public void OnQRScanned(string value)
    {
        OnQrAction?.Invoke(value);
    }
    
}