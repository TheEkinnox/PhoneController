using System;
using System.Collections;
using UnityEngine;

public class PowerEnabled : MonoBehaviour
{
    
    private Material _material;
    private ObjectSpin _objectSpin;
    
    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _objectSpin = GetComponent<ObjectSpin>();
    }
    private void OnEnable()
    {
        GameManager.Instance.PowerTrigger += LightTurnOn;
    }

    private void OnDisable()
    {
        if(GameManager.Instance)
            GameManager.Instance.PowerTrigger -= LightTurnOn;
    }

    private void Update()
    {
        float emissiveValue = Mathf.InverseLerp(0f, GameManager.Instance.chargeTime, GameManager.Instance.currentChargeTime);
        _material.SetFloat("_EmissiveOn", emissiveValue);
    }

    private void LightTurnOn()
    {
        if (_objectSpin != null)
            _objectSpin.enabled = true;
    }
}
