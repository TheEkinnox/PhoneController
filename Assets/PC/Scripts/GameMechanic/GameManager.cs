using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    
    public bool powerAlreadyTriggered = false;
    public float currentChargeTime;
    public float chargeTime;

    public void TriggerPower()
    {
        if (powerAlreadyTriggered) return;

        powerAlreadyTriggered = true;
        PowerTrigger?.Invoke();
    }

    public event Action PowerTrigger;
}
