using System;
using UnityEngine;

public class PowerEnabled : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.PowerTrigger += LightTurnOn;
    }

    private void LightTurnOn()
    {
        Debug.Log("yes");
    }
}
