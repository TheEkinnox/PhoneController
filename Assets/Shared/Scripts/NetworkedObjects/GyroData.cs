using System;
using UnityEngine;

[Serializable]
public struct GyroData
{
    public Quaternion value;

    public GyroData(Quaternion value)
    {
        this.value = value;
    }
}