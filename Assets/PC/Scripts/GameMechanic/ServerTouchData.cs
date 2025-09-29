using UnityEngine;

public class ServerTouchData : NetworkedBehaviour
{
    private Renderer _renderer;
    private Color _newColor;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _newColor = _renderer.material.color;
    }

    protected override void HandleMessage(NetworkedObject data)
    {
        if (!data.Is<TouchData>())
            return;

        TouchData touch = data.GetData<TouchData>();

        float r = Mathf.InverseLerp(0, Screen.width, touch.position.x);
        float g = Mathf.InverseLerp(0, Screen.height, touch.position.y);
        float b = (touch.phase == TouchPhase.Ended) ? 1f : 0.5f;

        _newColor = new Color(r, g, b);

        // Apply color immediately
        _renderer.material.color = _newColor;
    }
}
