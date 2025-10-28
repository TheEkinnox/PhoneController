using UnityEngine;
using UnityEngine.UI;

public class Camera : MonoBehaviour
{
    [SerializeField] private RawImage cameraDisplay;
    private WebCamTexture camTexture;

    void Start()
    {
        StartCamera();
    }

    private void StartCamera()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            WebCamDevice device = WebCamTexture.devices[0];
            camTexture = new WebCamTexture(device.name);
            cameraDisplay.texture = camTexture;
            camTexture.Play();
        }
        else
        {
            Debug.LogError("No camera found on this device!");
        }
    }

    void OnDestroy()
    {
        if (camTexture != null)
            camTexture.Stop();
    }
}
