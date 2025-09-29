using System;
using UnityEngine;

[Serializable]
public struct TouchData
{
    public Vector2 position;
    public TouchPhase phase;

    public TouchData(Vector2 position, TouchPhase phase)
    {
        this.position = position;
        this.phase = phase;
    }
}
