using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Text Objects (assign in current visual order)")]
    public TextMeshPro currentText;
    public TextMeshPro topText;
    public TextMeshPro bottomText;

    [Header("Animation")]
    public float slideSpeed = 10f;
    
    [Header("Scale")]
    public Vector3 currentScale = Vector3.one;
    public Vector3 sideScale = Vector3.one * 0.8f;

    private float time;
    private int lastSecond = -1;

    Vector3 currentSlot;
    Vector3 middleSlot;
    Vector3 bottomSlot;

    void Start()
    {
        currentSlot = currentText.transform.localPosition;
        middleSlot  = topText.transform.localPosition;
        bottomSlot  = bottomText.transform.localPosition;

        currentText.alpha = 1f;
        topText.alpha = 0.1f;
        bottomText.alpha = 0.1f;
        
        currentText.transform.localScale = currentScale;
        topText.transform.localScale = sideScale;
        bottomText.transform.localScale = sideScale;
    }

    void Update()
    {
        time += Time.deltaTime;

        int totalSeconds = Mathf.FloorToInt(time);
        int seconds = totalSeconds % 60;
        int minutes = totalSeconds / 60;

        if (seconds != lastSecond)
        {
            Rotate(minutes, seconds);
            lastSecond = seconds;
        }

        AnimateToSlots();
        AnimateScale();
    }

    void Rotate(int minutes, int seconds)
    {
        // Top → Current → Bottom → Top
        TextMeshPro temp = topText;
        topText = currentText;
        currentText = bottomText;
        bottomText = temp;

        // Current shows THIS second
        currentText.text = $"{minutes:00}:{seconds:00}";

        // Top = current + 1
        int nextSeconds = (seconds + 1) % 60;

        // Bottom = current - 1 (wrap-safe)
        int prevSeconds = (seconds - 1 + 60) % 60;

        bottomText.text = $"{minutes:00}:{nextSeconds:00}";
        topText.text = $"{minutes:00}:{prevSeconds:00}";
        

        // Visual emphasis
        currentText.alpha = 1f;
        topText.alpha = 0.4f;
        bottomText.alpha = 0.4f;
    }

    void AnimateToSlots()
    {
        currentText.transform.localPosition =
            Vector3.Lerp(currentText.transform.localPosition, currentSlot, Time.deltaTime * slideSpeed);

        topText.transform.localPosition =
            Vector3.Lerp(topText.transform.localPosition, middleSlot, Time.deltaTime * slideSpeed);

        bottomText.transform.localPosition =
            Vector3.Lerp(bottomText.transform.localPosition, bottomSlot, Time.deltaTime * slideSpeed);
    }
    
    void AnimateScale()
    {
        currentText.transform.localScale =
            Vector3.Lerp(currentText.transform.localScale, currentScale, Time.deltaTime * slideSpeed);

        topText.transform.localScale =
            Vector3.Lerp(topText.transform.localScale, sideScale, Time.deltaTime * slideSpeed);

        bottomText.transform.localScale =
            Vector3.Lerp(bottomText.transform.localScale, sideScale, Time.deltaTime * slideSpeed);
    }
}
