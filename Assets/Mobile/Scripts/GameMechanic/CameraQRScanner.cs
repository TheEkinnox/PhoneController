using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing; // make sure ZXing.Net is imported (see below)

public class CameraQRScanner : MonoBehaviour
{
    [Header("Camera Feed")]
    [SerializeField] private RawImage cameraDisplay;

    public event Action<string> OnQrAction;

    private WebCamTexture camTexture;
    private IBarcodeReader barcodeReader;
    private float nextScanTime = 0f;
    private const float ScanInterval = 2f; // scan every 0.3s to save performance

    void Start()
    {
        barcodeReader = new BarcodeReader { AutoRotate = true };
        StartCamera();
    }

    private void StartCamera()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            // Prefer back camera if available
            WebCamDevice device = Array.Find(WebCamTexture.devices, d => !d.isFrontFacing);
            if (string.IsNullOrEmpty(device.name))
                device = WebCamTexture.devices[0];

            camTexture = new WebCamTexture(device.name);
            cameraDisplay.texture = camTexture;
            camTexture.Play();

            Debug.Log($"🎥 Using camera: {device.name}");
        }
        else
        {
            Debug.LogError("❌ No camera found on this device!");
        }
    }

    void Update()
    {
        if (camTexture == null || !camTexture.isPlaying) return;

        // Rotate RawImage to match device orientation
        cameraDisplay.rectTransform.localEulerAngles =
            new Vector3(0, 0, -camTexture.videoRotationAngle);

        // Fix mirroring
        cameraDisplay.rectTransform.localScale = camTexture.videoVerticallyMirrored
            ? new Vector3(1, -1, 1)
            : Vector3.one;

        // Decode every few frames
        if (Time.time >= nextScanTime)
        {
            TryDecode();
            nextScanTime = Time.time + ScanInterval;
        }
    }

    private void TryDecode()
    {
        try
        {
            Color32[] pixels = camTexture.GetPixels32();
            if (pixels == null || pixels.Length == 0)
                return;

            var result = barcodeReader.Decode(pixels, camTexture.width, camTexture.height);

            if (result != null)
            {
                Debug.Log($"✅ QR Code Detected: {result.Text}");
                OnQrAction?.Invoke(result.Text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"⚠️ QR decode failed: {ex.Message}");
        }
    }

    void OnDestroy()
    {
        if (camTexture != null)
            camTexture.Stop();
    }
}