using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector3 GravityDirection { get; private set; } = Vector3.down;
    public float gravityStrength = 20f;

    public bool powerAlreadyTriggered = false;
    public float currentChargeTime;
    public float chargeTime;
    public Quaternion phoneRotation;

    public void TriggerPower()
    {
        if (powerAlreadyTriggered)
            return;

        powerAlreadyTriggered = true;
        PowerTrigger?.Invoke();
    }

    public void Update()
    {
        GravityDirection = phoneRotation * Vector3.down;
    }

    public event Action PowerTrigger;
}