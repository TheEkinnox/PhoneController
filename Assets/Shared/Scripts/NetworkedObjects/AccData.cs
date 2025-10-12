using System;
using UnityEngine;

[Serializable]
public struct AccData
{
    public Vector3 value;

    public AccData(Vector3 value)
    {
        this.value = value;
    }
}
