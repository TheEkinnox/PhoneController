using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    public bool powerEnabled;

    public bool powerEnabledAction
    {
        get => powerEnabled;
        set
        {
            if (powerEnabled == value) return;
            
            powerEnabled = value;

            if (powerEnabled)
            {
                PowerTrigger?.Invoke();
                powerEnabled = false;
            }
        }
    }

    public event Action PowerTrigger;
}
