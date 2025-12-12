using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    public Vector3 gravityDirection = Vector3.down;
    public float gravityStrength = 20f;
    
    public bool powerAlreadyTriggered = false;
    public float currentChargeTime;
    public float chargeTime;
    public Quaternion phoneRotation;

    public void TriggerPower()
    {
        if (powerAlreadyTriggered) return;

        powerAlreadyTriggered = true;
        PowerTrigger?.Invoke();
    }

    public event Action PowerTrigger;
    
    
}
